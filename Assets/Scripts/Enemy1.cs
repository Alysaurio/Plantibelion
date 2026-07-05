using UnityEngine;

public class Enemy1 : BaseEntity
{
    private AirSteering airSteering;

    protected override void Awake()
    {
        stats = new BaseStats(health: 100, power: 10, speed: 5, knockback: 2);
        base.Awake();

        airSteering = GetComponent<AirSteering>();
        if (airSteering != null)
            airSteering.maxSpeed = stats.Speed;
    }

    protected override void Die()
    {
        Debug.Log($"{entityName} lo mataron que bueno jajaj xDDDDD");
        // Colocar aquí luego el sistema de drop
        base.Die();
    }

    public void Attack()
    {
        // Lógica de ataque futura
    }
}