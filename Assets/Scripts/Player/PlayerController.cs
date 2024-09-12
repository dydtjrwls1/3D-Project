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

    // ���� �Լ���
    IPlayerState currentState;
    IdleState idleState = new IdleState();
    FireState fireState = new FireState();
    ChargingState chargingState = new ChargingState();

    // Fire ���¿��� Idle ���·� ���ư��� ������ �ణ�� ������ �ð�
    WaitForSeconds returnIdleWaitTimeDelay = new WaitForSeconds(0.5f);
    // Fire ���¿��� Ground�� ���� �� Idle ���·� ���ư��� �ڷ�ƾ �Լ��� ���� ����
    Coroutine returnIdleCoroutine;

    // �����ð� Charging �ø��� �ʱ�ȭ �Ѵ�.
    float elapsedTime = 0.0f;

    // �ٴڿ� ����� ����
    bool isGrounded = true;

    // Charging �� ���� ���� �κ� �޽�
    public Transform body;

    // ī�޶� �ٶ� ����Ʈ
    public Transform cameraPoint;

    // ��� Mesh�� �ڽ����� ������ �ִ� Ʈ������
    public Transform root;

    

    [Header("��ü ȸ��")]
    public float minRotateSpeed = 10.0f;
    public float maxRotateSpeed = 360.0f;
    public float rotateDuration = 5.0f;
    public AnimationCurve chargeAnimationCurve;

    [Header("ī�޶�")]
    public float defaultCameraDistance = 6.0f;
    public float zoomDistance = 4.0f;

    public float minFireForce = 3.0f;
    public float maxFireForce = 5.0f;
    public float cameraSpeed = 30.0f;

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
        // ī�޶� ���� ����
        cameraPoint.localRotation = Quaternion.Slerp(cameraPoint.localRotation, nextCameraRotation, Time.deltaTime * cameraSpeed);
        Debug.Log(cameraPoint.localRotation.x);
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
