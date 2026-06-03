using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEntity : MonoBehaviour, IDamageable
{
    [Header("Entity info")]
    public int entityID;
    public string entityName;
    public string entityDescription;
    [Header("Stats")]
    public BaseStats stats;
    protected int currentHealth;   
    
    protected virtual void Awake()
    {
        currentHealth = stats.Health;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{entityName} lo hirieron señor poleceaaaa!");
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void TakeDamage(DamageData damageData)
    {
        TakeDamage(damageData.damage);
    }

    protected virtual void Die()
    {
        Debug.Log($"{entityName} lo mataron doctor, mataron un inoceeeentee");
        Destroy(gameObject);
    }

    public int CurrentHealth => currentHealth;
    public BaseStats Stats => stats;
}
