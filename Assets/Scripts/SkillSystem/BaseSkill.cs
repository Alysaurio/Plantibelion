using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseSkill : MonoBehaviour
{
    public SkillData skillData;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void SetSkill(SkillData data,BaseEntity Target)
    {
        skillData = data;
        BaseSkill skill = Target.gameObject.AddComponent<BaseSkill>() ;
        skill.SetSkill(data, Target);
        
    }
    public virtual void ActiveSkill()

    {
        Debug.LogWarning("DEBES SOBREESCRIBIR ESTE METODO");
    }
}
