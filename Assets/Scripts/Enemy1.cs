using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : BaseEntity
{
    private AirSteering airSteering;

    [Header("Segmentos")]
    public GameObject segmentPrefab;
    public int segmentCount = 6;
    public float segmentSpacing = 0.5f;

    protected override void Awake()
    {
        stats = new BaseStats(health: 100, power: 10, speed: 5, knockback: 2);
        base.Awake();

        airSteering = GetComponent<AirSteering>();
        if (airSteering != null)
            airSteering.maxSpeed = stats.Speed;

        SpawnSegments();
    }

    void SpawnSegments()
    {
        if (segmentPrefab == null) return;

        IKChain ikChain = GetComponent<IKChain>();
        if (ikChain == null) return;

        // Limpiar joints anteriores y agregar la cabeza (este mismo GameObject)
        ikChain.joints.Clear();
        ikChain.joints.Add(transform);

        // Instanciar segmentos uno tras otro hacia abajo
        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 pos = transform.position + Vector3.down * segmentSpacing * (i + 1);
            GameObject seg = Instantiate(segmentPrefab, pos, Quaternion.identity);
            seg.transform.SetParent(transform.parent); // mismo nivel que Enemy en jerarquía

            // Registrar en IKChain
            ikChain.joints.Add(seg.transform);

            // Asignar cabeza al segmento para el sistema de daño
            if (seg.TryGetComponent<EnemySegment>(out var segment))
                segment.head = this;
        }

        // Reinicializar IKChain con los nuevos joints
        ikChain.Initialize();
    }

    protected override void Die()
    {
        Debug.Log($"{entityName} lo mataron que bueno jajaj xDDDDD");
        base.Die();
    }

    public void Attack() { }
}