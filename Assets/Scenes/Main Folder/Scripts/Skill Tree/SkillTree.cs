using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour, IDataPersistence
{
    [SerializeField] List<Skill> skills;

    public void EnterUI()
    {
        Debug.Log("EnterUI() pressed");
        gameObject.SetActive(true);
        SaveSystem.inst.LoadSkillTree(this);
        foreach (Skill skill in skills)
        {
            skill.Select();
        }
    }
    public void ExitUI()
    {
        SaveSystem.inst.SaveSkillTree(this);
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

    public void LoadData(GameData data)
    {
        Debug.Log("loading skill tree");
        for (int i = 0; i < skills.Count; i++)
        {
            skills[i].isAcquired = data.skills[i];
            if (skills[i].isAcquired)
            {
                skills[i].Confirm();
            }
        }
        
    }

    public void SaveData(GameData data)
    {
        Debug.Log("saving skill tree");
        for (int i = 0; i < skills.Count; i++)
        {
            data.skills[i] = skills[i].isAcquired;
        }
        
    }
}

