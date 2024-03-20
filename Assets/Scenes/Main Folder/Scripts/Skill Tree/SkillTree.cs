using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillTree : MonoBehaviour, IDataPersistence
{
    [SerializeField] List<Skill> skills;
    [SerializeField] GameObject upgradesScreenObj;
    [SerializeField] Button exitButton;
    [SerializeField] Button tablesUpgradeButton;

    public void EnterUI()
    {
        Debug.Log("EnterUI() pressed");
        gameObject.SetActive(true);
        SaveSystem.inst.LoadSkillTree(this);
        foreach (Skill skill in skills)
        {
            skill.Select();
        }
        EventSystem.current.firstSelectedGameObject = exitButton.gameObject;
        EventSystem.current.SetSelectedGameObject(exitButton.gameObject);
        upgradesScreenObj.SetActive(false);
    }
    public void ExitUI()
    {
        SaveSystem.inst.SaveSkillTree(this);
        gameObject.SetActive(false);
        EventSystem.current.firstSelectedGameObject = tablesUpgradeButton.gameObject;
        EventSystem.current.SetSelectedGameObject(tablesUpgradeButton.gameObject);
        upgradesScreenObj.SetActive(true);
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
                skills[i].CompleteSkill();
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

