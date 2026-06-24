using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : BaseEntity
{
    [Header("Input")]
    public InputSystem inputs;

    [Header("Movimiento")]
    public Rigidbody2D rigibody;
    public bool IsGrounded;
    public float MoveInput;
    public float Speed;
    public float JumpForce;
    public float NMaxJump;
    public float CurrentNJump;

    [Header("Skills")]
    private const int MaxSkills = 2;
    public List<SkillData> skills = new();

    protected override void Awake()
    {
        base.Awake();
        inputs = new();
        CurrentNJump = NMaxJump;
    }

    private void OnEnable()
    {
        inputs.Player.Skill1.performed += OnSkill1;
        inputs.Player.Skill2.performed += OnSkill2;

        inputs.Player.Movement.performed += OnMovementStart;
        inputs.Player.Movement.canceled += OnMovementFinish;
        inputs.Player.Jump.performed += OnJumpStart;

        inputs.Enable();
    }

    private void OnDisable()
    {
        inputs.Player.Skill1.performed -= OnSkill1;
        inputs.Player.Skill2.performed -= OnSkill2;

        inputs.Player.Movement.performed -= OnMovementStart;
        inputs.Player.Movement.canceled -= OnMovementFinish;
        inputs.Player.Jump.performed -= OnJumpStart;

        inputs.Disable();
    }

    private void FixedUpdate()
    {
        rigibody.velocity = new Vector2(MoveInput * Speed, rigibody.velocity.y);
    }

    public void AcquireSkill(SkillData newSkill)
    {
        if (skills.Count >= MaxSkills)
            skills.RemoveAt(0);

        skills.Add(newSkill);
        Debug.Log("[Player] Skill adquirido: " + newSkill.skillName + " | Total: " + skills.Count);
    }

    // Instancia y activa el skill del slot indicado (0 = slot 1, 1 = slot 2).
    private void UseSkill(int index)
    {
        if (index < 0 || index >= skills.Count)
        {
            Debug.Log("[Player] No hay skill en el slot " + (index + 1));
            return;
        }

        SkillData data = skills[index];

        if (data.skillPrefab == null)
        {
            Debug.LogWarning("[Player] El skill '" + data.skillName + "' no tiene prefab asignado.");
            return;
        }

        GameObject obj = Instantiate(data.skillPrefab, transform.position, Quaternion.identity);

        if (obj.TryGetComponent<BaseSkill>(out var skill))
        {
            skill.Initialize(data, this);
            skill.Activate();
        }
        else
        {
            Debug.LogWarning("[Player] El prefab de '" + data.skillName + "' no tiene un componente BaseSkill.");
        }
    }

    // -----------------------------------------------------------------
    // Callbacks de input
    // -----------------------------------------------------------------

    private void OnSkill1(InputAction.CallbackContext ctx) => UseSkill(0);
    private void OnSkill2(InputAction.CallbackContext ctx) => UseSkill(1);

    private void OnMovementStart(InputAction.CallbackContext ctx)
        => MoveInput = ctx.ReadValue<Vector2>().x;

    private void OnMovementFinish(InputAction.CallbackContext ctx)
        => MoveInput = 0;

    private void OnJumpStart(InputAction.CallbackContext ctx)
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
        if (collision.gameObject.CompareTag("Ground"))
        {
            rigibody.drag = 30;
            IsGrounded = true;
            CurrentNJump = NMaxJump;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            rigibody.drag = 1;
            IsGrounded = false;
        }
    }
}