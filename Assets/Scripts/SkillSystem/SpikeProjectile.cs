using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class SpikeProjectile : MonoBehaviour
{
    private SkillData skillData;
    private BaseEntity owner;
    private Vector2 startPos;
    private Rigidbody2D rb;
    public void Initialize(SkillData data, BaseEntity skillOwner, Vector2 dir)
    {
        skillData = data;
        owner = skillOwner;
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = dir.normalized * skillData.Speed;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
    private void Update()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        if (Vector2.Distance(startPos, transform.position) >= skillData.Range)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<IDamageable>(out var target)) return;
        if (target == (IDamageable)owner) return;
        target.TakeDamage(new DamageData((int)skillData.Damage, owner));
        Destroy(gameObject);
    }
}