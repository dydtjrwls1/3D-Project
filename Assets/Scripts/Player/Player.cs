using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    public float rotateSpeed = 180.0f;

    public float jumpForce = 5.0f;

    // 점프 쿨타임
    public float jumpCoolDown = 3.0f;

    // 카메라 이동 속도
    public float cameraSpeed = 2.0f;

    Rigidbody rb;

    Animator animator;

    PlayerInputActions inputActions;

    // 캐릭터의 Mesh 의 부모 트랜스폼
    Transform mesh;

    // 카메라가 바라보고 중심이되어서 회전할 포인트
    Transform cameraPoint;

    // 카메라가 바라보는 월드 상의 앞 방향 (X, Z 회전 무시하고)
    Vector3 cameraForward;

    Vector3 deltaCameraVector;

    Vector2 moveInput;

    

    // 마우스 움직임에 따른 카메라의 변동 위치
    Quaternion cameraDelta;

    // 키보드 입력에 따른 플레이어 회전 위치
    Quaternion rotationDelta; 
    
    // 움직임 여부 결정 변수
    private float moved = 0.0f;

    // 현재 발이 바닥에 닿았는지 확인하는 변수.
    bool isGrounded = true;

    // 남아있는 점프 쿨타임
    float jumpCoolRemains = 0.0f;

    // 점프가 가능한지 확인하는 프로퍼티
    bool IsJumpAvailabe => (isGrounded && (JumpCoolRemains < 0.0f));

    // 점프 쿨타임을 확인하고 설정하기 위한 프로퍼티
    float JumpCoolRemains
    {
        get => jumpCoolRemains;
        set
        {
            jumpCoolRemains = value;
            onJumpCoolDownChange?.Invoke(jumpCoolRemains / jumpCoolDown);
        }
    }

    // 점프 쿨타임에 변화가 있었음을 알리는 델리게이트
    public Action<float> onJumpCoolDownChange;

   

    readonly int IsMove_Hash = Animator.StringToHash("IsMove");
    readonly int IsUse_Hash = Animator.StringToHash("Use");
    readonly int Jump_Hash = Animator.StringToHash("Jump");

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mesh = transform.GetChild(1);
        cameraPoint = transform.GetChild(2);

        GroundSensor groundSensor = GetComponentInChildren<GroundSensor>();
        groundSensor.onGround += (isGround) => isGrounded = isGround;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += On_MoveInput;
        inputActions.Player.Move.canceled += On_MoveInput;
        inputActions.Player.Jump.performed += On_JumpInput;
        inputActions.Player.Use.performed += On_UseInput;
        inputActions.Player.Camera.performed += On_CameraInput;
        inputActions.Player.MousePoint.performed += On_MousePointInput;
    }

    private void OnDisable()
    {
        inputActions.Player.MousePoint.performed -= On_MousePointInput;
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
    }

    private void Update()
    {
        JumpCoolRemains -= Time.deltaTime; // 점프 쿨타임 줄이기
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    /// 이동 및 회전처리
    /// </summary>
    private void Movement(float deltaTime)
    {
        float moveAngle = Vector3.SignedAngle(Vector3.forward, new Vector3(moveInput.x, 0, moveInput.y), Vector3.up); // 입력값에 따른 이동 방향
        float directionY = (Camera.main.transform.rotation.eulerAngles.y) + moveAngle;                                          // 현재 카메라의 방향 + 입력값에 따른 이동 방향

        rotationDelta = Quaternion.Euler(0, directionY, 0);                                                           // 최종 이동할 방향의 각도

        Vector3 direction = rotationDelta * transform.forward;                                                        // 캐릭터가 다음 프레임에 이동하게 될 방향

        // 새 이동할 위치 : 현재위치 + 초당 moveSpeed의 속도로, 오브젝트의 앞 쪽 방향을 기준으로 전진/후진/정지
        Vector3 position = rb.position + deltaTime * moveSpeed * moved * (direction);

        Quaternion nextRotation = moved > 0.1f ? rotationDelta : mesh.rotation;                                       // 움직였으면 다음 회전이고 움직이지 않았으면 기존 회전을 유지하는 변수

        mesh.rotation = Quaternion.Slerp(mesh.rotation, nextRotation, deltaTime * rotateSpeed);

        rb.MovePosition(position);

        // transform.rotation = Quaternion.Slerp(transform.rotation, rotationDelta, Time.deltaTime * 50.0f);
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
        Camera.main.GetComponent<MainCamera>().Switching();
    }

    private void On_MousePointInput(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();
        Debug.Log(delta);

        // float yDelta = MathF.Abs(delta.x) < 1.0f ? delta.y : 0.0f;
        // float xDelta = MathF.Abs(delta.y) < 1.0f ? delta.x : 0.0f;

        // cameraDelta = cameraPoint.rotation * Quaternion.Euler(-delta.y, delta.x, 0);
        Quaternion rotation = Quaternion.AngleAxis(delta.x, transform.up) * Quaternion.AngleAxis(-delta.y, Camera.main.transform.right);
        deltaCameraVector = rotation * (Camera.main.transform.localPosition);
    }

    void CameraRotation()
    {
        // cameraPoint.rotation = Quaternion.Slerp(cameraPoint.rotation, cameraDelta, Time.deltaTime * cameraSpeed);
        Camera.main.transform.localPosition = Vector3.Slerp(Camera.main.transform.localPosition, deltaCameraVector, Time.deltaTime * rotateSpeed);
        Camera.main.transform.LookAt(cameraPoint.position);
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
}
