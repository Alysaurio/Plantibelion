using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : BaseEntity
{
    protected override void Die()
    {
        Debug.Log($"{entityName} lo mataron que bueno jajaj xDDDDD");
        // Colocar aquí luego el sistema de drop
        base.Die();
    }
}
