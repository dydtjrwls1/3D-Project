using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
    public float moveSpeed = 5.0f;              // 이동 속도
    public float rotateSpeed = 180.0f;          // 플레이어 메쉬 회전 속도
    float moved = 0.0f;                         // 움직임 여부 결정 변수

    public float jumpForce = 5.0f;              // 점프 세기
    public float jumpCoolDown = 3.0f;           // 점프 쿨타임
    float jumpCoolRemains = 0.0f;               // 남아있는 점프 쿨타임

    public float cameraSpeed = 2.0f;            // 카메라 이동 속도

    Rigidbody rb;
    Animator animator;
    PlayerInputActions inputActions;

    Transform mesh;                             // 캐릭터의 Mesh 의 부모 트랜스폼

    Transform cameraPoint;                      // 카메라가 바라보고 중심이되어서 회전할 포인트

    Vector3 deltaCameraVector;                  // 카메라의 다음 벡터
     
    Vector2 moveInput;                          // 키보드 이동 인풋값

    Quaternion rotationDelta;                   // 키보드 입력에 따른 플레이어 회전 위치
    Quaternion deltaPointRotation;              // 카메라 포인트의 다음 회전

    bool isGrounded = true;                     // 현재 발이 바닥에 닿았는지 확인하는 변수.


#if UNITY_EDITOR
    public List<Vector3> points = new List<Vector3>(); // 선을 그릴 포인트들
    public Color gizmoColor = Color.green; // Gizmo 색상
    public bool gizmoOn = true;
#endif



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

        Camera.main.transform.forward = cameraPoint.forward;
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
    
        // 오브젝트의 현재 위치를 포인트 목록에 추가
        if (gizmoOn && points.Count == 0 || (points.Count > 0 && points[points.Count - 1] != transform.position))
        {
            points.Add(Camera.main.transform.position);
        }
    

    }

    private void LateUpdate()
    {
        CameraRotation();
        Quaternion nextRotation = moved > 0.1f ? rotationDelta : mesh.rotation;                                       // 움직였으면 다음 회전이고 움직이지 않았으면 기존 회전을 유지하는 변수
        mesh.rotation = Quaternion.Slerp(mesh.rotation, nextRotation, Time.deltaTime * rotateSpeed);                  // player mesh 를 카메라 방향에 맞춰서 회전
    }

    /// 이동 및 회전처리
    /// </summary>
    private void Movement(float deltaTime)
    {
        float moveAngle = Vector3.SignedAngle(Vector3.forward, new Vector3(moveInput.x, 0, moveInput.y), Vector3.up); // 입력값에 따른 이동 방향
        float directionY = (Camera.main.transform.rotation.eulerAngles.y) + moveAngle;                                // 현재 카메라의 방향 + 입력값에 따른 이동 방향

        rotationDelta = Quaternion.Euler(0, directionY, 0);                                                           // 최종 이동할 방향의 각도

        Vector3 direction = rotationDelta * transform.forward;                                                        // 캐릭터가 다음 프레임에 이동하게 될 방향

        // 새 이동할 위치 : 현재위치 + 초당 moveSpeed의 속도로, 오브젝트의 앞 쪽 방향을 기준으로 전진/후진/정지
        Vector3 position = rb.position + deltaTime * moveSpeed * moved * (direction);

        rb.MovePosition(position);
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

    }

    void CameraRotation()
    {
        cameraPoint.transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        
        
        Camera.main.transform.localPosition = Vector3.Slerp(Camera.main.transform.localPosition, deltaCameraVector, Time.deltaTime * cameraSpeed);
        
        
        
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

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;


        if (points.Count > 1)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
    }
}
