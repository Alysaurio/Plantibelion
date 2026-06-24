using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerController player;

    [Header("Skills iniciales")]   
    public SkillData[] startingSkills; // aun no le asigno nada

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("[GameManager] No hay PlayerController asignado.");
            return;
        }

        foreach (SkillData skill in startingSkills)
        {
            if (skill != null)
                player.AcquireSkill(skill);
        }
    }
}