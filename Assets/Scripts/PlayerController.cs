using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : BaseEntity
{
    public InputSystem inputs;
    public Rigidbody2D rigibody;

    public bool IsGrounded;
    public float MoveInput;
    public float Speed;
    public float JumpForce;
    public float NMaxJump;
    public float CurrentNJump;

    [Header("Skills")]
    public GameObject shockWavePrefab;
    public int shockWaveDamage = 10;

    protected override void Awake()
    {
        base.Awake();
        inputs = new();
        CurrentNJump = NMaxJump;
    }

    private void OnEnable()
    {
        inputs.Player.ShockWave.performed += OnShockWave;

        inputs.Enable();
        inputs.Player.Movement.performed += OnMovementStart;
        inputs.Player.Movement.canceled += OnMovementFinish;

        inputs.Player.Jump.performed += OnJumpStart;
    }

    private void OnDisable()
    {
        inputs.Player.Movement.performed -= OnMovementStart;
        inputs.Player.Movement.canceled -= OnMovementFinish;
        inputs.Player.Jump.performed -= OnJumpStart;
        inputs.Disable();
    }

    private void Update()
    {
        if (MoveInput != 0)
        {
            Vector2 dir = new Vector2(MoveInput, rigibody.velocity.y);
            rigibody.velocity = new Vector2(MoveInput * Speed, rigibody.velocity.y);
        }

    }

    private void OnShockWave(InputAction.CallbackContext context)
    {
        GameObject obj = Instantiate(shockWavePrefab, transform.position, Quaternion.identity);
        ShockWaveSkill skill = obj.GetComponent<ShockWaveSkill>();
        skill.Initialize(this, shockWaveDamage);
    }



    private void OnMovementStart(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>().x;
    }
    private void OnMovementFinish(InputAction.CallbackContext context)
    {
        MoveInput = 0;
    }
    private void OnJumpStart(InputAction.CallbackContext context)
    {
        if (IsGrounded || CurrentNJump > 0)
        {
            rigibody.velocity = Vector2.zero;
            rigibody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            CurrentNJump--;
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            rigibody.drag = 30;
            IsGrounded = true;
            CurrentNJump = NMaxJump;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            rigibody.drag = 1;
            IsGrounded = false;
        }
    }
    


    }
