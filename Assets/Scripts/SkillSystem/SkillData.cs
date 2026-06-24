using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillMode
{
    None,
    UniqueAttack,
    ContinuousAttack,
}

[CreateAssetMenu(fileName = "SkillData", menuName = "Plantibellion/Skills/SkillData", order = 100)]

public class SkillData : ScriptableObject
{
    [Header("Visual")]
    public Sprite Icon;
    public string skillName;

    [Header("Stats")]
    public float Range;
    public float Speed;
    public float Damage;
    public float Knockback;
    public float Cooldown;

    [Header("Behavior")]
    public SkillMode modeAttack;

    [Header("Prefab")]
    public GameObject skillPrefab;
}
