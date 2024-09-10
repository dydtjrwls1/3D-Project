using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CinemachineVirtualCamera vcam;
    PlayerInputActions inputAction;
    Rigidbody rb;
    Animator animator;

    public Transform body;
    public Transform cameraPoint;
    public Transform root;

    IPlayerState currentState;

    float elapsedTime = 0.0f;

    bool isGrounded = true;

    [Header("객체 회전")]
    public float minRotateSpeed = 10.0f;
    public float maxRotateSpeed = 360.0f;
    public float rotateDuration = 5.0f;
    public AnimationCurve chargeAnimationCurve;

    [Header("카메라")]
    public float defaultCameraDistance = 6.0f;
    public float zoomDistance = 4.0f;

    public float fireForce = 5.0f;
    public float cameraSpeed = 30.0f;

    // Changing State 관련 프로퍼티
    public CinemachineVirtualCamera PlayerMainCam => vcam;
    public Transform PlayerBody => body;
    public Rigidbody PlayerRb => rb;
    public Animator Animator => animator;

    public float ElapsedTime => elapsedTime;
    // BodyAnimationCurve의 현재 값
    public float CurrentChargeDelta => chargeAnimationCurve.Evaluate(elapsedTime / rotateDuration);

    // BodyAnimationCurve의 현재 값에 따른 회전 속도
    public float CurrentRotateSpeed => minRotateSpeed + (maxRotateSpeed - minRotateSpeed) * CurrentChargeDelta;

    // BodyAnimationCurve의 현재 값에 따른 카메라 줌 거리
    public float CurrentZoomDistance => defaultCameraDistance - zoomDistance * CurrentChargeDelta;

    Vector3 nextCameraRotation;

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
        SetState(new IdleState());
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

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGrounded && collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            StartCoroutine(ReturnIdleRoutine());
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
    }

    private void LateUpdate()
    {
        // 카메라 각도 변경
        cameraPoint.localEulerAngles = Vector3.Slerp(cameraPoint.localEulerAngles, nextCameraRotation, Time.deltaTime * cameraSpeed);
        //cameraPoint.localEulerAngles = new Vector3(cameraPoint.localEulerAngles.x, cameraPoint.localEulerAngles.y, 0);
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
            SetState(new ChargingState());
            animator.enabled = false;
        }
    }

    private void Off_Click(InputAction.CallbackContext context)
    {
        if(currentState is ChargingState)
        {
            animator.enabled = true;
            animator.SetBool(Fire_Hash, true);
            SetState(new FireState());
        }
    }

    private void On_MouseMove(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();

        float deltaX = Mathf.Clamp(delta.x, -30.0f, 30.0f);
        float deltaY = Mathf.Clamp(delta.y, -30.0f, 30.0f);

        float nextXRotation = Mathf.Clamp(cameraPoint.localEulerAngles.x + -deltaY, 0.0f, 90.0f);
        float nextYRotation = cameraPoint.localEulerAngles.y + deltaX;
        nextCameraRotation = new Vector3(nextXRotation, nextYRotation, 0);
    }

    IEnumerator ReturnIdleRoutine()
    {
        while(MathF.Abs(rb.velocity.x) > 1.0f)
        {
            yield return null;
        }

        animator.SetBool(Fire_Hash, false);

        yield return new WaitForSeconds(0.5f);

        SetState(new IdleState());
    }
}
