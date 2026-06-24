using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    public SkillData skillData { get; private set; }
    protected BaseEntity owner { get; private set; }
    public virtual void Initialize(SkillData data, BaseEntity skillOwner)
    {
        skillData = data;
        owner = skillOwner;
    }

    public virtual void Activate()
    {
        Debug.LogWarning("Activate() no está sobreescrito en " + GetType().Name);
    }
}
