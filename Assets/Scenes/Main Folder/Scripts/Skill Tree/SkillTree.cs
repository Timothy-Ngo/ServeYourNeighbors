using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    [SerializeField] List<Skill> skills;

    public void EnterUI()
    {
        Debug.Log("EnterUI() pressed");
        gameObject.SetActive(true);
        foreach (Skill skill in skills)
        {
            skill.Select();
        }
    }
    public void ExitUI()
    {
        gameObject.SetActive(false);
    }
}
