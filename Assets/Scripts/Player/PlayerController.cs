using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // 필요 컴포넌트
    CinemachineVirtualCamera vcam;
    PlayerInputActions inputAction;
    Rigidbody rb;
    Animator animator;

    // 상태 함수들
    IPlayerState currentState;
    IdleState idleState = new IdleState();
    FireState fireState = new FireState();
    ChargingState chargingState = new ChargingState();

    // Fire 상태에서 Idle 상태로 돌아가기 까지의 약간의 딜레이 시간
    WaitForSeconds returnIdleWaitTimeDelay = new WaitForSeconds(0.5f);
    // Fire 상태에서 Ground에 닿을 시 Idle 상태로 돌아가는 코루틴 함수를 담을 변수
    Coroutine returnIdleCoroutine;

    // 누적시간 Charging 시마다 초기화 한다.
    float elapsedTime = 0.0f;

    // 바닥에 닿았음 여부
    bool isGrounded = true;

    // Charging 시 돌릴 몸통 부분 메쉬
    public Transform body;

    // 카메라가 바라볼 포인트
    public Transform cameraPoint;

    // 모든 Mesh를 자식으로 가지고 있는 트랜스폼
    public Transform root;

    

    [Header("객체 회전")]
    public float minRotateSpeed = 10.0f;
    public float maxRotateSpeed = 360.0f;
    public float rotateDuration = 5.0f;
    public AnimationCurve chargeAnimationCurve;

    [Header("카메라")]
    public float defaultCameraDistance = 6.0f;
    public float zoomDistance = 4.0f;

    public float minFireForce = 3.0f;
    public float maxFireForce = 5.0f;
    public float cameraSpeed = 30.0f;

    // Changing State 관련 프로퍼티
    public CinemachineVirtualCamera PlayerMainCam => vcam;
    public Transform PlayerBody => body;
    public Rigidbody PlayerRb => rb;
    public Animator Animator => animator;

    public Action<float> onCharging = null;

    public float ElapsedTime => elapsedTime;
    // BodyAnimationCurve의 현재 값
    public float CurrentChargeDelta => chargeAnimationCurve.Evaluate(elapsedTime / rotateDuration);

    // BodyAnimationCurve의 현재 값에 따른 회전 속도
    public float CurrentRotateSpeed => minRotateSpeed + (maxRotateSpeed - minRotateSpeed) * CurrentChargeDelta;

    // BodyAnimationCurve의 현재 값에 따른 카메라 줌 거리
    public float CurrentZoomDistance => defaultCameraDistance - zoomDistance * CurrentChargeDelta;

    public float CurrentFireForce => minFireForce + (maxFireForce - minFireForce) * CurrentChargeDelta;

    Quaternion nextCameraRotation;

    readonly int Fire_Hash = Animator.StringToHash("Fire");

    private void Awake()
    {
        inputAction = new PlayerInputActions();
        vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        rb = GetComponentInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        SetState(idleState);
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Click.performed += On_Click;
        inputAction.Player.Click.canceled += Off_Click;
        inputAction.Player.MousePoint.performed += On_MouseMove;
    }

    private void OnDisable()
    {
        inputAction.Player.Click.canceled -= Off_Click;
        inputAction.Player.Click.performed -= On_Click;
        inputAction.Player.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnDie();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded && collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            if(returnIdleCoroutine != null)
            {
                StopCoroutine(returnIdleCoroutine);
            }

            returnIdleCoroutine = StartCoroutine(ReturnIdleRoutine());      
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isGrounded && collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        currentState.UpdateState(this);
        //Debug.Log(cameraPoint.local);
    }

    private void LateUpdate()
    {
        // 카메라 각도 변경
        cameraPoint.localRotation = Quaternion.Slerp(cameraPoint.localRotation, nextCameraRotation, Time.deltaTime * cameraSpeed);
        Debug.Log(cameraPoint.localRotation.x);
    }

    public void SetState(IPlayerState newState)
    {
        // 상태 전환 처리
        if (currentState != null)
        {
            currentState.ExitState(this);
        }
        currentState = newState;
        currentState.EnterState(this);
    }

    private void On_Click(InputAction.CallbackContext context)
    {
        if(currentState is IdleState)
        {
            elapsedTime = 0.0f;
            SetState(chargingState);
            animator.enabled = false;
        }
    }

    private void Off_Click(InputAction.CallbackContext context)
    {
        if(currentState is ChargingState)
        {
            animator.enabled = true;
            animator.SetBool(Fire_Hash, true);
            SetState(fireState);
        }
    }

    private void On_MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();

        float deltaX = Mathf.Clamp(delta.x, -30.0f, 30.0f);
        float deltaY = Mathf.Clamp(delta.y, -30.0f, 30.0f);

        float nextXRotation = cameraPoint.localEulerAngles.x - deltaY;
        float nextYRotation = cameraPoint.localEulerAngles.y + deltaX;

        nextCameraRotation = Quaternion.Euler(nextXRotation, nextYRotation, 0);
    }

    IEnumerator ReturnIdleRoutine()
    {
        while(MathF.Abs(rb.velocity.x) > 1.0f)
        {
            yield return null;
        }

        yield return returnIdleWaitTimeDelay;

        animator.SetBool(Fire_Hash, false);
        SetState(idleState);

        returnIdleCoroutine = null;
    }

    void OnDie()
    {
        Transform camTransform = vcam.transform;

        vcam.Follow = null;
        camTransform.position = transform.position + Vector3.up * 2.0f;
        camTransform.parent = null;
        camTransform.LookAt(transform.position);

        inputAction.Player.Disable();
    }
}
