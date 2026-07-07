using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseEntity
{
    protected override void Die()
    {
        Debug.Log($"{entityName} lo mataron doctor, mataron un inoceeeentee");
        // Colocar lo que falta, game over, reiniciar nivel, etc.
        base.Die();
    }
}
