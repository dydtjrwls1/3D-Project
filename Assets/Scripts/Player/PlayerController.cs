using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // �ʿ� ������Ʈ
    CinemachineVirtualCamera vcam;
    PlayerInputActions inputAction;
    Rigidbody rb;
    Animator animator;
    GroundSensor groundSensor;

    // ���� �Լ���
    IPlayerState currentState;
    IdleState idleState = new IdleState();
    FireState fireState = new FireState();
    ChargingState chargingState = new ChargingState();

    // Fire ���¿��� Idle ���·� ���ư��� ������ �ణ�� ������ �ð�
    //WaitForSeconds returnIdleWaitTimeDelay = new WaitForSeconds(0.5f);
    // Fire ���¿��� Ground�� ���� �� Idle ���·� ���ư��� �ڷ�ƾ �Լ��� ���� ����
    //Coroutine returnIdleCoroutine;

    // �����ð� Charging �ø��� �ʱ�ȭ �Ѵ�.
    float elapsedTime = 0.0f;

    // Charging �� ���� ���� �κ� �޽�
    public Transform body;

    // ī�޶� �ٶ� ����Ʈ
    public Transform cameraPoint;

    // ��� Mesh�� �ڽ����� ������ �ִ� Ʈ������
    public Transform root;

    float epsilon = 1.0f;

    bool isGround = true;
    
    [Header("��ü ȸ��")]
    public float minRotateSpeed = 10.0f;
    public float maxRotateSpeed = 360.0f;
    public float rotateDuration = 5.0f;
    public AnimationCurve chargeAnimationCurve;

    [Header("ī�޶�")]
    float defaultCameraDistance;
    public float zoomDistance = 4.0f;

    public float minFireForce = 3.0f;
    public float maxFireForce = 5.0f;
    public float cameraSpeed = 30.0f;

    public float DefaultCameraDistance => defaultCameraDistance;

    // Changing State ���� ������Ƽ
    public CinemachineVirtualCamera PlayerMainCam => vcam;
    public Transform PlayerBody => body;
    public Rigidbody PlayerRb => rb;
    public Animator Animator => animator;

    public Action<float> onCharging = null;

    public float ElapsedTime => elapsedTime;
    // BodyAnimationCurve�� ���� ��
    public float CurrentChargeDelta => chargeAnimationCurve.Evaluate(elapsedTime / rotateDuration);

    // BodyAnimationCurve�� ���� ���� ���� ȸ�� �ӵ�
    public float CurrentRotateSpeed => minRotateSpeed + (maxRotateSpeed - minRotateSpeed) * CurrentChargeDelta;

    // BodyAnimationCurve�� ���� ���� ���� ī�޶� �� �Ÿ�
    public float CurrentZoomDistance => defaultCameraDistance - zoomDistance * CurrentChargeDelta;

    public float CurrentFireForce => minFireForce + (maxFireForce - minFireForce) * rb.mass * CurrentChargeDelta;

    Quaternion nextCameraRotation;

    readonly int Fire_Hash = Animator.StringToHash("Fire");

    private void Awake()
    {
        inputAction = new PlayerInputActions();
        vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        rb = GetComponentInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        groundSensor = GetComponentInChildren<GroundSensor>();
    }

    private void Start()
    {
        defaultCameraDistance = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance;

        SetState(idleState);
        groundSensor.onGround += (onGround) =>
        {
            isGround = onGround;
        };
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
        if (other.CompareTag("DeathZone"))
        {
            OnDie();
        }
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (currentState is FireState && isGround && rb.velocity.sqrMagnitude < epsilon)
        {
            //if (returnIdleCoroutine != null)
            //{
            //    StopCoroutine(returnIdleCoroutine);
            //}

            //returnIdleCoroutine = StartCoroutine(ReturnIdleRoutine());
            animator.SetBool(Fire_Hash, false);
            SetState(idleState);
        }
    }

    private void FixedUpdate()
    {
        if(isGround) rb.velocity *= 0.9f;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        currentState.UpdateState(this);
        //Debug.Log(cameraPoint.local);
    }

    private void LateUpdate()
    {
        // ī�޶� ���� ����
        cameraPoint.localRotation = Quaternion.Slerp(cameraPoint.localRotation, nextCameraRotation, Time.deltaTime * cameraSpeed);
        //Debug.Log(cameraPoint.localRotation.x);
    }

    public void SetState(IPlayerState newState)
    {
        // ���� ��ȯ ó��
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

    //IEnumerator ReturnIdleRoutine()
    //{
    //    yield return returnIdleWaitTimeDelay;

    //    animator.SetBool(Fire_Hash, false);
    //    SetState(idleState);

    //    returnIdleCoroutine = null;
    //}

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
