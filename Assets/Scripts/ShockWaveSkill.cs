using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveSkill : MonoBehaviour
{
    [Header("ShockWave Settings")]
    [SerializeField] private float maxRadius = 5f;
    [SerializeField] private float expansionSpeed = 10f;
    [SerializeField] private int damage = 10;

    private float currentRadius;
    private BaseEntity owner;
    private readonly HashSet<IDamageable> hitTargets = new();
    private void Initialize(BaseEntity owner, int damage)
    {
        this.owner = owner;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
