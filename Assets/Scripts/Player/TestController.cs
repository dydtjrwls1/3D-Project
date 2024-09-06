using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestController : MonoBehaviour
{
    PlayerInputActions inputAction;

    private void Awake()
    {
        inputAction = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputAction.Player.Click.performed += On_Click;
    }

    private void On_Click(InputAction.CallbackContext context)
    {
        Debug.Log("Click");
    }
}
