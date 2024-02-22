// Author: Timothy Ngo
// Date: 2/21/24
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasterGrindingSkill : Skill
{
    // Needs to be assigned in the inspector-------------
    [SerializeField] Skill preReqSkill;
    [SerializeField] Grinder grinder;
    //---------------------------------------------------
    [SerializeField] int newGrindTime = 1; // Base time is 2 seconds
    public override bool CheckRequirements()
    {
        return preReqSkill.isAcquired 
        && Currency.inst.AbleToWithdraw(skillCost);  
    }

    public override void MissingRequirements()
    {
        int missingGold = skillCost - Currency.inst.gold;
        if (!preReqSkill.isAcquired)
        {
            SkillInformation.inst.missingRequirementsText.text = $"Missing {skillName} skill.";
        }
        if (missingGold > 0)
        {
            SkillInformation.inst.missingRequirementsText.text += $"Missing {missingGold} gold.";
        }
    }

    public override void Confirm()
    {
         if (CheckRequirements())
        {
            Debug.Log("Making grinding go vroom vroom");
            Currency.inst.Withdraw(skillCost);
            grinder.grindTime = newGrindTime;
            CompleteSkill();
        }
        else
        {
            Debug.LogError("There is absolutely no way this should be displayed in the console. The player has pressed confirm without achieving the requirements");
        }
    }

}
