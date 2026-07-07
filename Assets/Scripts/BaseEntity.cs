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
    [SerializeField] protected int currentHealth;

    [Header("Invencibilidad")]
    public float invincibilityDuration = 0.3f;
    private float invincibilityTimer = 0f;

    protected virtual void Awake()
    {
        currentHealth = stats.Health;
    }

    protected virtual void Update()
    {
        if (invincibilityTimer > 0f)
            invincibilityTimer -= Time.deltaTime;
    }

    public virtual void TakeDamage(int damage)
    {
        if (invincibilityTimer > 0f) return;

        currentHealth -= damage;
        invincibilityTimer = invincibilityDuration;
        Debug.Log($"{entityName} lo hirieron señor poleceaaaa!");

        if (currentHealth <= 0)
            Die();
    }

    public virtual void TakeDamage(DamageData damageData)
    {
        TakeDamage(damageData.damage);
    }

    protected virtual void Die()
    {        
        Destroy(gameObject);
    }

    public int CurrentHealth => currentHealth;
    public BaseStats Stats => stats;
}