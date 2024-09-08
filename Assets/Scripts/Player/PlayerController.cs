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
    public Transform body;

    IPlayerState currentState;

    float elapsedTime = 0.0f;

    [Header("��ü ȸ��")]
    public float minRotateSpeed = 10.0f;
    public float maxRotateSpeed = 360.0f;
    public float rotateDuration = 5.0f;
    public AnimationCurve chargeAnimationCurve;

    [Header("ī�޶�")]
    public float defaultCameraDistance = 6.0f;
    public float zoomDistance = 4.0f;

    public float fireForce = 5.0f;

    // Changing State ���� ������Ƽ
    public CinemachineVirtualCamera PlayerMainCam => vcam;
    public Transform PlayerBody => body;

    // BodyAnimationCurve�� ���� ��
    public float CurrentChargeDelta => chargeAnimationCurve.Evaluate(elapsedTime / rotateDuration);

    // BodyAnimationCurve�� ���� ���� ���� ȸ�� �ӵ�
    public float CurrentRotateSpeed => minRotateSpeed + (maxRotateSpeed - minRotateSpeed) * CurrentChargeDelta;

    // BodyAnimationCurve�� ���� ���� ���� ī�޶� �� �Ÿ�
    public float CurrentZoomDistance => defaultCameraDistance - zoomDistance * CurrentChargeDelta;

    private void Awake()
    {
        inputAction = new PlayerInputActions();
        vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        rb = GetComponentInChildren<Rigidbody>();
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

    }

    private void OnDisable()
    {
        inputAction.Player.Click.canceled -= Off_Click;
        inputAction.Player.Click.performed -= On_Click;
        inputAction.Player.Disable();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        currentState.UpdateState(this);
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
        elapsedTime = 0.0f;
        SetState(new ChargingState());
    }

    private void Off_Click(InputAction.CallbackContext context)
    {
        SetState(new IdleState());
        rb.AddForce((transform.up + transform.forward) * CurrentChargeDelta * fireForce, ForceMode.Impulse);
    }
}
