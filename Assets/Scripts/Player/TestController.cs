using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestController : MonoBehaviour
{
    CinemachineVirtualCamera vcam;
    PlayerInputActions inputAction;
    public Transform body;

    bool rotate = false;

    float elapsedTime = 0.0f;

    [Header("객체 회전")]
    public float minRotateSpeed = 10.0f;
    public float maxRotateSpeed = 360.0f;
    public float rotateDuration = 5.0f;
    public AnimationCurve bodyAnimationCurve;

    [Header("카메라")]
    public float defaultCameraDistance = 6.0f;
    public float zoomDistance = 4.0f;
    
    private void Awake()
    {
        inputAction = new PlayerInputActions();
        vcam = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        inputAction.Player.Enable();
        inputAction.Player.Click.performed += On_Click;
        inputAction.Player.Click.canceled += Off_Click;

    }

    

    private void OnDisable()
    {
        inputAction.Player.Click.performed -= On_Click;
        inputAction.Player.Disable();
    }

    private void Update()
    {
        
        if (rotate)
        {
            elapsedTime += Time.deltaTime;
            float delta = bodyAnimationCurve.Evaluate(elapsedTime / rotateDuration);
            float rotateSpeed = minRotateSpeed + (maxRotateSpeed - minRotateSpeed) * delta;
            body.Rotate(Time.deltaTime * rotateSpeed * Vector3.up);

            var framingTransposer = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            // Check if the component exists and then modify the camera distance
            if (framingTransposer != null)
            {
                framingTransposer.CameraDistance = defaultCameraDistance - zoomDistance * delta;
            }
            else
            {
                Debug.LogError("CinemachineFramingTransposer component not found.");
            }
        }
        
    }

    private void On_Click(InputAction.CallbackContext context)
    {
        rotate = true;
    }

    private void Off_Click(InputAction.CallbackContext context)
    {
        rotate = false;
    }
}
