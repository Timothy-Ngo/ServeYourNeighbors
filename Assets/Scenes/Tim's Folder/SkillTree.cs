using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    [SerializeField] List<Skill> skills;
    public void Start()
    {
        ExitUI();
    }
    public void EnterUI()
    {
        gameObject.SetActive(true);
        foreach(Skill skill in skills)
        {
            skill.Select();
        }
    }
    public void ExitUI()
    {
        gameObject.SetActive(false);
    }
}
