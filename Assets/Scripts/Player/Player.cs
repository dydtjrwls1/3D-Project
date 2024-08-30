using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 이동
    public float moveSpeed = 5.0f;              // 이동 속도
    public float rotateSpeed = 180.0f;          // 플레이어 메쉬 회전 속도
    float moved = 0.0f;                         // 움직임 여부 결정 변수
    float nextRotate;                           // 방향키 눌렸을 때 다음 회전의 회전 값

    // 점프
    public float jumpForce = 5.0f;              // 점프 세기
    public float jumpCoolDown = 3.0f;           // 점프 쿨타임
    float jumpCoolRemains = 0.0f;               // 남아있는 점프 쿨타임
    
    bool IsJumpAvailabe => (isGrounded && (JumpCoolRemains < 0.0f)); // 점프가 가능한지 확인하는 프로퍼티
    
    float JumpCoolRemains                       // 점프 쿨타임을 확인하고 설정하기 위한 프로퍼티
    {
        get => jumpCoolRemains;
        set
        {
            jumpCoolRemains = value;
            onJumpCoolDownChange?.Invoke(jumpCoolRemains / jumpCoolDown);
        }
    }

    // 델리게이트
    public Action<float> onJumpCoolDownChange;  // 점프 쿨타임에 변화가 있었음을 알리는 델리게이트
    public Action<Vector2> OnMouseInput = null; // 마우스 변동이 있을 때 마다 실행되는 이벤트
    public Action<float, bool> OnScrollInput = null;// 마우스 스크롤 변동이 있을 때 마다 실행되는 이벤트

    Rigidbody rb;
    Animator animator;
    public PlayerInputActions inputActions;
    

    // 메쉬
    Transform mesh;                             // 캐릭터의 Mesh 의 부모 트랜스폼
    Vector2 moveInput;                          // 키보드 이동 인풋값
    bool isGrounded = true;                     // 현재 발이 바닥에 닿았는지 확인하는 변수.

    Transform cameraPoint;                      // 카메라 포인트
    float inputAngle;

    Quaternion NextRotation              // 다음 프레임에서의 회전 (CameraPoint 의 앞쪽방향 + 마우스 변동량)
    {
        get
        {
            if(moved > 0.1f)
            {

                Vector3 inputToVector3 = new Vector3(moveInput.x, 0, moveInput.y);
                Quaternion inputDirection = Quaternion.LookRotation(inputToVector3);
                Quaternion cameraDirection = Quaternion.LookRotation(cameraPoint.forward);

                Quaternion delta = inputDirection * cameraDirection;

                return Quaternion.Euler(0, delta.eulerAngles.y, 0);
            }
            else
            {
                return Quaternion.identity;
            }
            
        }
    }

    float InputAngle
    {
        get
        {
            float angle;

            if (moveInput.x == 0 && moveInput.y == 1)
                angle = 0f;       // 위쪽
            else if (moveInput.x == 1 && moveInput.y == 1)
                angle = 45f;      // 오른쪽 위
            else if (moveInput.x == 1 && moveInput.y == 0)
                angle = 90f;      // 오른쪽
            else if (moveInput.x == 1 && moveInput.y == -1)
                angle = 135f;     // 오른쪽 아래
            else if (moveInput.x == 0 && moveInput.y == -1)
                angle = 180f;     // 아래쪽
            else if (moveInput.x == -1 && moveInput.y == -1)
                angle = 225f;     // 왼쪽 아래
            else if (moveInput.x == -1 && moveInput.y == 0)
                angle = 270f;     // 왼쪽
            else if (moveInput.x == -1 && moveInput.y == 1)
                angle = 315f;     // 왼쪽 위
            else
                angle = 0f;       // 기본값, 필요 시 수정 가능

            return angle;
        }
    }
    
    // 세이브 포인트 관련
    ParticleSystem respawnEffeect;              // 리스폰 시 재생될 이펙트
    Vector3 savePoint = Vector3.zero;           // 리스폰 시 되돌아갈 위치
    
    public Vector3 CurrentSavePoint             // 현재 세이브 포인트 위치
    {
        get => savePoint;
        set
        {
            savePoint = value;
        }
    }
   
    // 애니메이션 해쉬 값
    readonly int IsMove_Hash = Animator.StringToHash("IsMove");
    readonly int IsUse_Hash = Animator.StringToHash("Use");
    readonly int Jump_Hash = Animator.StringToHash("Jump");



    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mesh = transform.GetChild(0);
        cameraPoint = transform.GetChild(1);
        respawnEffeect = GetComponentInChildren<ParticleSystem>();

        GroundSensor groundSensor = GetComponentInChildren<GroundSensor>();
        groundSensor.onGround += (isGround) => isGrounded = isGround;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // 커서를 화면 중앙에 고정
        Cursor.visible = false;  // 커서를 화면에서 숨김
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += On_MoveInput;
        inputActions.Player.Move.canceled += On_MoveInput;
        inputActions.Player.Jump.performed += On_JumpInput;
        inputActions.Player.Use.performed += On_UseInput;
        inputActions.Player.Camera.performed += On_CameraInput;
        inputActions.Player.MousePoint.performed += On_MouseDeltaInput;
        inputActions.Player.Zoom.performed += On_ZoomInput;
    }

    

    private void OnDisable()
    {
        inputActions.Player.Zoom.performed -= On_ZoomInput;
        inputActions.Player.MousePoint.performed -= On_MouseDeltaInput;
        inputActions.Player.Camera.performed -= On_CameraInput;
        inputActions.Player.Use.performed -= On_UseInput;
        inputActions.Player.Jump.performed -= On_JumpInput;
        inputActions.Player.Move.canceled -= On_MoveInput;
        inputActions.Player.Move.performed -= On_MoveInput;
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        Movement(Time.fixedDeltaTime);
        RotateMesh();
    }

    private void Update()
    {
        
        JumpCoolRemains -= Time.deltaTime; // 점프 쿨타임 줄이기
    }

    private void LateUpdate()
    {
        
    }

    /// 이동 및 회전처리
    /// </summary>
    private void Movement(float deltaTime)
    {
        Vector3 direction = NextRotation * transform.forward; // 캐릭터가 다음 프레임에 이동하게 될 방향

        // 새 이동할 위치 : 현재위치 + 초당 moveSpeed의 속도로, 오브젝트의 앞 쪽 방향을 기준으로 전진/후진/정지
        Vector3 position = rb.position + deltaTime * moveSpeed * moved * (direction);

        rb.MovePosition(position);
    }

    void RotateMesh()
    {
        Quaternion nextRotation = moved > 0.1f ? NextRotation : mesh.rotation;
        mesh.rotation = Quaternion.Slerp(mesh.rotation,  nextRotation, Time.fixedDeltaTime * rotateSpeed); // player 이동 시 Nextrotation 의 Y축으로 회전, 정지 시 현재 Rotate 유지 
    }

    private void On_MoveInput(InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), !context.canceled);
    }

    private void On_JumpInput(InputAction.CallbackContext _)
    {
        Jump();
    }

    private void On_UseInput(InputAction.CallbackContext _)
    {
        Use();
    }

    private void On_CameraInput(InputAction.CallbackContext _)
    {
        Respawn();
    }

    

    private void On_MouseDeltaInput(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        OnMouseInput?.Invoke(delta);
    }

    private void On_ZoomInput(InputAction.CallbackContext context)
    {
        bool canceled = context.canceled;
        Vector2 delta = context.ReadValue<Vector2>();
        OnScrollInput?.Invoke(delta.y, canceled);
    }

    

    /// <summary>
    /// 이동 입력 처리용 함수
    /// </summary>
    /// <param name="input">입력된 방향</param>
    /// <param name="isMove">이동 중이면 true, 아니면 false</param>
    void SetInput(Vector2 input, bool isMove)
    {
        moved = isMove ? 1.0f : 0.0f;
        moveInput = input;
        // nextRotate = Vector3.SignedAngle(Vector3.forward, new Vector3(moveInput.x, 0, moveInput.y), Vector3.up);

        if (input.x == 1 && input.y == 0)
            inputAngle = 0f;       // 오른쪽
        else if (input.x == -1 &&input.y == 0)
            inputAngle = 180f;     // 왼쪽
        else if (input.x == 0 && input.y == 1)
            inputAngle = 90f;      // 위쪽
        else if (input.x == 0 && input.y == -1)
            inputAngle = 270f;     // 아래쪽
        else
            inputAngle = 0f;       // 기본값 (0, 0)인 경우에 해당, 필요 시 처리 방법에 따라 수정 가능

        animator.SetBool(IsMove_Hash, isMove);
    }

    /// <summary>
    /// 플레이어 점프 처리용 함수
    /// </summary>
    void Jump()
    {
        // 점프 키를 누르면 실행된다(space 키)
        // 2단 점프 금지
        // 쿨타임 존재
        if (IsJumpAvailabe)
        {
            animator.SetTrigger(Jump_Hash);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // 위로 점프
            JumpCoolRemains = jumpCoolDown;                         // 쿨타임 초기화
        }
    }

    /// <summary>
    /// 상호작용 관련 처리용 함수
    /// </summary>
    void Use()
    {
        animator.SetTrigger(IsUse_Hash);
    }

    /// <summary>
    /// 플레이어 사망 처리 함수
    /// </summary>
    public void Die()
    {
        Debug.Log("사망");
    }

    private void Respawn()
    {
        transform.position = CurrentSavePoint;
        respawnEffeect.Play();
    }
}
