// Author: Timothy Ngo
// Date: 3/9/24
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedDistractionTimeSkill : Skill
{
    // Needs to be assigned in the inspector-------------
    [SerializeField] Skill preReqSkill;
    [Tooltip("Value is a percentage from 0-1")]
    [SerializeField] float timeIncreasePercentage = .5f;
    //---------------------------------------------------

    public override bool CheckRequirements()
    {
        return preReqSkill.isAcquired &&
            DistractionSystem.inst.animatronicDistraction != null &&
            Currency.inst.AbleToWithdraw(skillCost);  
    }

    public override void MissingRequirements()
    {
        int missingGold = skillCost - Currency.inst.gold;
        SkillInformation.inst.missingRequirementsText.text = "";
        if (!preReqSkill.isAcquired)
        {
            SkillInformation.inst.missingRequirementsText.text = $"Missing {preReqSkill.skillName}.";
        }
        if (DistractionSystem.inst.animatronicDistraction == null)
        {
            SkillInformation.inst.missingRequirementsText.text += $"\r\nMissing animatronic.";
        }
        if (missingGold > 0)
        {
            SkillInformation.inst.missingRequirementsText.text += $"\r\nMissing {missingGold} gold.";
        }
    }

    public override void Confirm()
    {
        if (CheckRequirements())
        {
            Currency.inst.Withdraw(skillCost);
            CompleteSkill();
        }
        else
        {
            Debug.LogError("There is absolutely no way this should be displayed in the console. The player has pressed confirm without achieving the requirements");
        }
    }
    public override void ActivateMechanic()
    {
        DistractionSystem.inst.distractedTime  *= (1 + timeIncreasePercentage);
    }
}
