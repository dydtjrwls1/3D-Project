using Cinemachine;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    FireState fireState;
    ChargingState chargingState;

    Coroutine resetPlayerCoroutine;

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

    // Charging �� ��ȭ�� �̹���
    Image chargingImage;

    Vector3 spawnPoint;

    // Reset �ǰ� �ٽ� ���ƿ� ī�޶� ����Ʈ�� ��ġ
    Vector3 cameraPointOrigin;

    float epsilon = 1.0f;

    bool isGround = true;

    bool isHit = false;
    //bool wasHit = false;

    int maxHp = 3;
    int hp;

    float defaultCameraDistance;

    [Header("��ü ȸ��")]
    public float minRotateSpeed = 10.0f;
    public float maxRotateSpeed = 360.0f;
    public float rotateDuration = 5.0f;
    public AnimationCurve chargeAnimationCurve;

    [Header("ī�޶�")]
    public float zoomDistance = 4.0f;
    public float cameraSpeed = 30.0f;

    [Header("�߻� �ӵ�")]
    public float minFireForce = 3.0f;
    public float maxFireForce = 5.0f;

    // Hit �� ����� �߷��� ũ��
    public float gravity = 10.0f;
    
    public float DefaultCameraDistance => defaultCameraDistance;

    public bool IsHit => isHit;

    // Changing State ���� ������Ƽ
    public CinemachineVirtualCamera PlayerMainCam => vcam;
    public Transform PlayerBody => body;
    public Rigidbody PlayerRb => rb;
    public Animator Animator => animator;
    public Image ChargingImage => chargingImage;
    public IdleState IdleState => idleState;
    

    public Action<float> onCharging = null;

    public Action<int> onHealthChange = null;

    public Action onDie = null;

    public int HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, 3);
            onHealthChange?.Invoke(hp);
            if (hp < 1)
            {
                OnDie();
            }
            else
            {
                ResetPlayer();
            }
        }
    }

    public float ElapsedTime => elapsedTime;
    // BodyAnimationCurve�� ���� ��
    public float CurrentChargeDelta => chargeAnimationCurve.Evaluate(elapsedTime / rotateDuration);

    // BodyAnimationCurve�� ���� ���� ���� ȸ�� �ӵ�
    public float CurrentRotateSpeed => minRotateSpeed + (maxRotateSpeed - minRotateSpeed) * CurrentChargeDelta;

    // BodyAnimationCurve�� ���� ���� ���� ī�޶� �� �Ÿ�
    public float CurrentZoomDistance => defaultCameraDistance - zoomDistance * CurrentChargeDelta;

    public float CurrentFireForce => minFireForce + (maxFireForce - minFireForce) * rb.mass * CurrentChargeDelta;

    Quaternion nextCameraRotation;

    public readonly int Fire_Hash = Animator.StringToHash("Fire");

    private void Awake()
    {
        inputAction = new PlayerInputActions();
        vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        rb = GetComponentInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        groundSensor = GetComponentInChildren<GroundSensor>();

        Transform canvas = GetComponentInChildren<Canvas>().transform;
        chargingImage = canvas.GetChild(0).GetComponent<Image>();

        chargingState = new ChargingState(this);
        fireState = new FireState(this);

        spawnPoint = transform.position;
        hp = maxHp;

        cameraPointOrigin = cameraPoint.localPosition;
    }

    private void Start()
    {
        defaultCameraDistance = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance;

        SetState(idleState);
        groundSensor.onGround += (onGround) =>
        {
            isGround = onGround;
        };

        chargingImage.fillAmount = 0.0f;

        Goal goal = FindAnyObjectByType<Goal>();
        goal.onClear += Clear;
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
        inputAction.Player.MousePoint.performed -= On_MouseMove;
        inputAction.Player.Click.canceled -= Off_Click;
        inputAction.Player.Click.performed -= On_Click;
        inputAction.Player.Disable();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("DeathZone"))
    //    {
    //        OnDie();
    //    }
    //}

    
    private void OnCollisionStay(Collision collision)
    {
        // �߻� �� ���� �������� �� velocity �� ���� ���ϸ� Idle ���·� ���ư���
        if (currentState is FireState && rb.velocity.sqrMagnitude < epsilon)
        {
            //if (returnIdleCoroutine != null)
            //{
            //    StopCoroutine(returnIdleCoroutine);
            //}

            //returnIdleCoroutine = StartCoroutine(ReturnIdleRoutine());
            //animator.SetBool(Fire_Hash, false);
            SetState(idleState);
        }
    }

    private void FixedUpdate()
    {
        if (isGround)
        {
            if (isHit)
            {
                rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
            } 
            else
            {
                rb.velocity *= 0.95f;
            }
        }

        rb.angularVelocity = Vector3.zero;
        rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
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
            //animator.SetBool(Fire_Hash, true);
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
        onDie?.Invoke();

        Transform camTransform = vcam.transform;

        vcam.Follow = null;
        camTransform.position = transform.position + Vector3.up * 2.0f;
        camTransform.parent = null;
        camTransform.LookAt(transform.position);

        inputAction.Player.Disable();
    }

    public void HitPlayer(Vector3 hitVelocity, float hitTime)
    {
        rb.velocity = hitVelocity;
        // StartCoroutine(DecreaseHitForce(hitTime));
    }

    //IEnumerator DecreaseHitForce(float hitTIme)
    //{
    //    if (isHit) wasHit = true;
    //    isHit = true;

    //    for(float t = 0f; t < hitTIme; t += Time.deltaTime)
    //    {
    //        yield return null;
    //    }

    //    if(wasHit) wasHit = false;
    //    else
    //    {
    //        isHit = false;
    //    }
    //}

    public void ResetPlayer()
    {
        if(resetPlayerCoroutine != null)
        {
            StopCoroutine(resetPlayerCoroutine);
        }

        resetPlayerCoroutine = StartCoroutine(ResetPlayerCoroutine());
    }

    IEnumerator ResetPlayerCoroutine()
    {
        inputAction.Player.MousePoint.performed -= On_MouseMove;
        cameraPoint.parent = null;

        SetState(idleState);

        float elapsedTime = 0.0f;
        while (elapsedTime < 1.0f)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        cameraPoint.parent = gameObject.transform;
        cameraPoint.localPosition = cameraPointOrigin;
        inputAction.Player.MousePoint.performed += On_MouseMove;

        

        transform.position = spawnPoint;
        transform.rotation = Quaternion.identity;
        cameraPoint.localRotation = Quaternion.identity;

        resetPlayerCoroutine = null;
    }

    void Clear()
    {
        inputAction.Player.Disable();
        SetState(idleState);
        rb.velocity = Vector3.zero;
    }
}
