using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class TestController : MonoBehaviour
{
    Animator animator;

    PlayerInputActions inputActions;

    Rigidbody rb;

    CharacterController controller;

    Vector3 direction;

    float speed = 5.0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += On_MoveInput;
        inputActions.Player.Move.canceled += On_MoveInput;
        inputActions.Player.Jump.performed += On_JumpInput;
        inputActions.Player.Use.performed += On_UseInput;
        inputActions.Player.Camera.performed += On_CameraInput;
    }

    private void On_CameraInput(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void On_UseInput(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void On_JumpInput(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        inputActions.Player.Use.performed -= On_UseInput;
        inputActions.Player.Jump.performed -= On_JumpInput;
        inputActions.Player.Move.canceled -= On_MoveInput;
        inputActions.Player.Move.performed -= On_MoveInput;
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void On_MoveInput(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        direction = new Vector3(value.x, 0, value.y).normalized;
    }

    void Movement()
    {
        Vector3 nextPosition = transform.position + Time.fixedDeltaTime * speed * direction;
        rb.MovePosition(nextPosition);
    }
}
