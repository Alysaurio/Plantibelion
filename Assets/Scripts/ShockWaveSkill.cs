using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveSkill : MonoBehaviour
{
    [Header("ShockWave Settings")]
    public float maxRadius = 5f;
    public float expansionSpeed = 10f;
    public int damage = 10;

    private float currentRadius;
    private BaseEntity owner;
    private readonly HashSet<IDamageable> hitTargets = new();

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentRadius += expansionSpeed * Time.deltaTime;
        Collider[] hits = Physics.OverlapSphere(transform.position, currentRadius);

        foreach (Collider hit in hits)
        {
            if (!hit.TryGetComponent<IDamageable>(out var target))
                continue;
            if (target == owner)
                continue;
            if (hitTargets.Contains(target))
                continue;

            hitTargets.Add(target);
            target.TakeDamage(new DamageData(damage, owner));
        }
        if (currentRadius >= maxRadius)
        {
            Destroy(gameObject);
        }

    }

    public void Initialize(BaseEntity owner, int damage)
    {
        this.owner = owner;
        this.damage = damage;
        currentRadius = 0f;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, currentRadius);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRadius);
    }

}
