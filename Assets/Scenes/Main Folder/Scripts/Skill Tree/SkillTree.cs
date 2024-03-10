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

    // return number of active skills
    public int GetNumActive()
    {
        int numActive = 0;

        foreach (Skill skill in skills)
        {
            if (skill.isAcquired)
            {
                numActive++;
            }
        }

        return numActive;
    }
}
