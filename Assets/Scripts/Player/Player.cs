using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    public float rotateSpeed = 180.0f;

    public float jumpForce = 5.0f;

    // 점프 쿨타임
    public float jumpCoolDown = 3.0f;

    PlayerInputActions inputActions;

    // 회전방향(음수면 좌회전, 양수면 우회전)
    private float rotateDirection = 0.0f;
    // 이동방향(음수면 후진, 양수면 전진)
    private float moveDirection = 0.0f;

    // 현재 발이 바닥에 닿았는지 확인하는 변수.
    bool isGrounded = true;

    // 남아있는 점프 쿨타임
    float jumpCoolRemains = 0.0f;

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

    Rigidbody rb;

    Animator animator;

    readonly int IsMove_Hash = Animator.StringToHash("IsMove");
    readonly int IsUse_Hash = Animator.StringToHash("Use");

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        GroundSensor groundSensor = GetComponentInChildren<GroundSensor>();
        groundSensor.onGround += (isGround) => isGrounded = isGround;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += On_MoveInput;
        inputActions.Player.Move.canceled += On_MoveInput;
        inputActions.Player.Jump.performed += On_JumpInput;
        inputActions.Player.Use.performed += On_UseInput;
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
        Movement(Time.fixedDeltaTime);
    }

    private void Update()
    {
        JumpCoolRemains -= Time.deltaTime; // 점프 쿨타임 줄이기
    }

    /// 이동 및 회전처리
    /// </summary>
    private void Movement(float deltaTime)
    {
        // 새 이동할 위치 : 현재위치 + 초당 moveSpeed의 속도로, 오브젝트의 앞 쪽 방향을 기준으로 전진/후진/정지
        Vector3 position = rb.position + deltaTime * moveSpeed * moveDirection * transform.forward;

        // 새 회전 : 현재회전 + 초당 rotateSpeed의 속도로, 좌회전/우회전/정지하는 회전
        Quaternion rotation = rb.rotation * Quaternion.AngleAxis(deltaTime * rotateSpeed * rotateDirection, transform.up);

        rb.Move(position, rotation);
    }

    private void On_MoveInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        SetInput(context.ReadValue<Vector2>(), !context.canceled);
    }

    private void On_JumpInput(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        Jump();
    }

    private void On_UseInput(InputAction.CallbackContext _)
    {
        Use();
    }

    /// <summary>
    /// 이동 입력 처리용 함수
    /// </summary>
    /// <param name="input">입력된 방향</param>
    /// <param name="isMove">이동 중이면 true, 아니면 false</param>
    void SetInput(Vector2 input, bool isMove)
    {
        rotateDirection = input.x;
        moveDirection = input.y;

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
}
