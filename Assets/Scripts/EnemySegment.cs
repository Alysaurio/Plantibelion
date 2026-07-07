using UnityEngine;

public class EnemySegment : MonoBehaviour, IDamageable
{
    public BaseEntity head;

    void Update()
    {        
        if (head == null)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if (head == null) return;
        head.TakeDamage(damage);
    }

    public void TakeDamage(DamageData damageData)
    {
        if (head == null) return;
        head.TakeDamage(damageData);
    }
}