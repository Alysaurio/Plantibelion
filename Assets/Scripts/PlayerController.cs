using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : BaseEntity
{
    [Header("Input")]
    public InputSystem inputs;

    [Header("Movimiento")]
    public Rigidbody2D rigibody;
    public float MoveInput;
    public float Speed;
    public float JumpForce;
    public float NMaxJump;
    public float CurrentNJump;

    [Header("Efectos")]
    [SerializeField] private UnityEvent Attack;

    [Header("Detección de suelo")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;      // punto vacío en los pies del player
    [SerializeField] private float groundCheckRadius = 0.1f;
    public bool IsGrounded;

    [Header("Skills")]
    private const int MaxSkills = 2;
    public List<SkillData> skills = new();

    [Header("Efecto de salto (squash de la Capsule)")]
    [SerializeField] private Transform capsuleVisual;
    [SerializeField] private float jumpSquashScale = 0.3f;
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float squashRecoverySpeed = 2f;

    private float currentSquash = 1f;

    [Header("Inclinación al moverse (BodyVisual)")]
    [SerializeField] private Transform bodyVisual;
    [SerializeField] private float tiltAngle = 15f;
    [SerializeField] private float tiltSpeed = 90f;

    private float currentTilt = 0f;

    protected override void Awake()
    {
        base.Awake();
        inputs = new();
        CurrentNJump = NMaxJump;
        currentSquash = normalScale;
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

    protected override void Update()
    {
        base.Update();

        if (capsuleVisual != null)
        {
            currentSquash = Mathf.MoveTowards(currentSquash, normalScale, squashRecoverySpeed * Time.deltaTime);
            capsuleVisual.localScale = new Vector3(currentSquash, currentSquash, capsuleVisual.localScale.z);
        }

        if (bodyVisual != null)
        {
            float targetTilt = MoveInput > 0 ? -tiltAngle : (MoveInput < 0 ? tiltAngle : 0f);
            currentTilt = Mathf.MoveTowards(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
            bodyVisual.localRotation = Quaternion.Euler(0f, 0f, currentTilt);
        }
    }

    private void FixedUpdate()
    {
        // Detectar suelo con OverlapCircle en cada frame
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (IsGrounded)
            CurrentNJump = NMaxJump;

        rigibody.velocity = new Vector2(MoveInput * Speed, rigibody.velocity.y);
    }

    public void AcquireSkill(SkillData newSkill)
    {
        if (skills.Count >= MaxSkills)
            skills.RemoveAt(0);

        skills.Add(newSkill);
        Debug.Log("[Player] Skill adquirido: " + newSkill.skillName + " | Total: " + skills.Count);
    }

    private void UseSkill(int index)
    {
        Attack?.Invoke();
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

    private void OnSkill1(InputAction.CallbackContext ctx) => UseSkill(0);
    private void OnSkill2(InputAction.CallbackContext ctx) => UseSkill(1);

    private void OnMovementStart(InputAction.CallbackContext ctx) => MoveInput = ctx.ReadValue<Vector2>().x;
    private void OnMovementFinish(InputAction.CallbackContext ctx) => MoveInput = 0;

    private void OnJumpStart(InputAction.CallbackContext ctx)
    {
        if (IsGrounded || CurrentNJump > 0)
        {
            rigibody.velocity = new Vector2(rigibody.velocity.x, 0);
            rigibody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            IsGrounded = false;
            CurrentNJump--;

            currentSquash = jumpSquashScale;
        }
    }

    // Visualizar el punto de detección de suelo en el editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}