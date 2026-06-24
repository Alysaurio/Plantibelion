using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveSkill : BaseSkill
{
   
    private float currentRadius;
    private readonly HashSet<IDamageable> hitTargets = new();
 
    public override void Activate()
    {
        // Añadir un sonido o efecto visual
    }
 
    private void Update()
    {
        if (owner == null) return;
 
        transform.position = owner.transform.position;
        currentRadius += skillData.Speed * Time.deltaTime;
 
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, currentRadius);
 
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent<IDamageable>(out var target)) continue;
            if (target == (IDamageable)owner) continue;
            if (hitTargets.Contains(target)) continue;
 
            hitTargets.Add(target);
            target.TakeDamage(new DamageData((int)skillData.Damage, owner));
        }
 
        if (currentRadius >= skillData.Range)
        {
            Destroy(gameObject);
        }
    }
 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, currentRadius);
 
        if (skillData != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, skillData.Range);
        }
    }
    

}
