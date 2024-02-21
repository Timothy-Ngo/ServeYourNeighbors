// Author: Timothy Ngo
// Date: 2/20/24
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] protected string skillName; // Keep at 19 characters max
    [SerializeField] protected string skillDesc;
    [SerializeField] protected int skillCost;

    public abstract bool CheckRequirements(); // Determines if skill can be acquired

    public abstract void MissingRequirements(); // Determines what requirements are missing
    public abstract void Confirm();
    public void AchievedRequirements()
    {
        SkillInformation.inst.confirmButton.onClick.RemoveAllListeners();
        SkillInformation.inst.confirmButton.onClick.AddListener(Confirm);
    }
    public void Select()
    {
        SkillInformation.inst.skillNameText.text = skillName;
        SkillInformation.inst.skillDescText.text = skillDesc;
        SkillInformation.inst.skillCostText.text = $"{skillCost}";
        if (CheckRequirements())
        {
            SkillInformation.inst.missingRequirementsText.transform.parent.gameObject.SetActive(false);
            SkillInformation.inst.confirmButton.gameObject.SetActive(true);
            AchievedRequirements();
        }
        else
        {
            SkillInformation.inst.confirmButton.gameObject.SetActive(false);
            SkillInformation.inst.missingRequirementsText.transform.parent.gameObject.SetActive(true);
            MissingRequirements();
        }
    }

}
