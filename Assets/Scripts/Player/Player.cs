using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // 이동
    public float moveSpeed = 5.0f;              // 이동 속도
    public float speedThreshold = 10.0f;
    public float rotateSpeed = 180.0f;          // 플레이어 메쉬 회전 속도
    float moved = 0.0f;                         // 움직임 여부 결정 변수

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
    bool isGrounded = true;                     // 현재 발이 바닥에 닿았는지 확인하는 변수.

    Transform cameraPoint;                      // 카메라 포인트

    int moveX;                                  // 키보드 방향키의 X 입력 값 (-1, 0, 1)
    int moveY;                                  // 키보드 방향키의 Y 입력 값 (-1, 0, 1)

    float InputAngle                            // 키보드 방향키의 입력 각도 (8방향)
    {
        get
        {
            if (moveX == 0 && moveY == 1)
                return 0f;       // 위쪽
            else if (moveX == 1 && moveY == 1)
                return 45f;      // 오른쪽 위
            else if (moveX == 1 && moveY == 0)
                return 90f;      // 오른쪽
            else if (moveX == 1 && moveY == -1)
                return 135f;     // 오른쪽 아래
            else if (moveX == 0 && moveY == -1)
                return 180f;     // 아래쪽
            else if (moveX == -1 && moveY == -1)
                return 225f;     // 왼쪽 아래
            else if (moveX == -1 && moveY == 0)
                return 270f;     // 왼쪽
            else if (moveX == -1 && moveY == 1)
                return 315f;     // 왼쪽 위
            else
                return 0f;       // 기본값, 필요 시 수정 가능
        }
    }

    Quaternion NextRotation              // 다음 프레임에서의 회전 (CameraPoint 의 앞쪽방향 + 마우스 변동량)
    {
        get
        {
            if(moved > 0.1f)
            {
                float nextAngle = InputAngle + cameraPoint.eulerAngles.y; // 키보드 입력 각 + 현재 카메라의 Y 축 회전

                return Quaternion.Euler(0, nextAngle, 0);
            }
            else
            {
                return Quaternion.identity;
            }
            
        }
    }

    
    
    // 세이브 포인트 관련
    ParticleSystem respawnEffect;              // 리스폰 시 재생될 이펙트
    Vector3 savePoint = Vector3.zero;           // 리스폰 시 되돌아갈 위치
    
    public Vector3 CurrentSavePoint             // 현재 세이브 포인트 위치
    {
        get => savePoint;
        set
        {
            savePoint = value;
        }
    }

    // 캐리거 피격 관련
    bool isHit = false;                         // 피격 당했는지 여부

    float hitCoolDown = 1.0f;                   // 피격 쿨타임 (초당 최대 1회 피격 가능)
    float remainsHitCoolDown;                   // 남은 피격 쿨타임

    bool CanHit => remainsHitCoolDown < 0f;     // 피격 가능 여부

    // 애니메이션 해쉬 값
    readonly int IsMove_Hash = Animator.StringToHash("IsMove");
    readonly int IsUse_Hash = Animator.StringToHash("Use");
    readonly int Jump_Hash = Animator.StringToHash("Jump");
    readonly int Hit_Hash = Animator.StringToHash("Hit");



    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mesh = transform.GetChild(0);
        cameraPoint = transform.GetChild(1);
        respawnEffect = GetComponentInChildren<ParticleSystem>();

        GroundSensor groundSensor = GetComponentInChildren<GroundSensor>();
        groundSensor.onGround += (isGround) => 
        { 
            isGrounded = isGround;
            Debug.Log(isGround);
        };
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
        inputActions.Player.Camera.performed += On_RespawnInput;
        inputActions.Player.MousePoint.performed += On_MouseDeltaInput;
        inputActions.Player.Zoom.performed += On_ZoomInput;
    }

    

    private void OnDisable()
    {
        inputActions.Player.Zoom.performed -= On_ZoomInput;
        inputActions.Player.MousePoint.performed -= On_MouseDeltaInput;
        inputActions.Player.Camera.performed -= On_RespawnInput;
        inputActions.Player.Use.performed -= On_UseInput;
        inputActions.Player.Jump.performed -= On_JumpInput;
        inputActions.Player.Move.canceled -= On_MoveInput;
        inputActions.Player.Move.performed -= On_MoveInput;
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        if (!isHit)
        {
            Movement(Time.fixedDeltaTime);
            RotateMesh();
        }
    }

    private void Update()
    {
        remainsHitCoolDown -= Time.deltaTime;
        JumpCoolRemains -= Time.deltaTime; // 점프 쿨타임 줄이기
    }

    private void LateUpdate()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground") && isHit)
        {
            StopAllCoroutines();
            StartCoroutine(RecoverFromHit());
        }

        Debug.Log(collision.gameObject.name);

        IHit hitInterface = Util.FindInterface<IHit>(collision.gameObject);
        if (hitInterface != null && !isHit)
        {
            // Vector3 hitPoint = collision.GetContact(0).point; // 접촉한 지점의 벡터
            Hit();
            hitInterface.OnHit(rb, collision);
        }
    }

    /// 이동 및 회전처리
    /// </summary>
    private void Movement(float deltaTime)
    {
        Vector3 forceDirection = NextRotation * transform.forward * moved;  // 캐릭터가 입력값에 대응하는 힘을 받을 방향

        //rb.AddForce(forceDirection * moveSpeed * 2.0f, ForceMode.Force);

        //rb.MovePosition(rb.position + forceDirection * moveSpeed * deltaTime);

        rb.velocity = forceDirection  * moveSpeed + Vector3.up * rb.velocity.y;                      // 점프할 때 velocity가 덮어 씌워지는것을 방지하기 위해 현재 velocity의 y값은 유지해준다.
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

    private void On_RespawnInput(InputAction.CallbackContext _)
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
        moveX = (int)input.x;
        moveY = (int)input.y;

        // Debug.Log(isHit);
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

    // 리스폰 시 실행할 함수
    private void Respawn()
    {
        rb.velocity = Vector3.zero;
        transform.position = CurrentSavePoint;
        respawnEffect.Play();
    }

    // 맞았을 때 실행될 함수
    void Hit()
    {
        if (CanHit)
        {
            remainsHitCoolDown = hitCoolDown;
            isHit = true;

            // Quaternion lookHitPos = Quaternion.LookRotation(hitPoint);
            // mesh.rotation = lookHitPos; // 플레이어의 mesh 가 접촉한 지점을 바라본다

            animator.SetTrigger(Hit_Hash);
        }
    }

    // 맞고 땅에 떨어졌을때 원래 상태로 돌아오는 코루틴
    IEnumerator RecoverFromHit()
    {
        float elapsedTime = 0.0f;
            
        while(elapsedTime < 1.0f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
            
        }

        isHit = false;
    }
}
