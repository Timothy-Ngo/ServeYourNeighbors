// Author: Timothy Ngo
// Date: 3/9/24
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedWaitTime : Skill
{
    // Needs to be assigned in the inspector-------------
    [SerializeField] Skill preReqSkill;
    [Tooltip("Value is a percentage from 0-1")]
    [SerializeField] float timeIncreasePercentage = .15f;
    //---------------------------------------------------
    public override bool CheckRequirements()
    {
        return preReqSkill.isAcquired &&
            Currency.inst.AbleToWithdraw(skillCost);
            
    }

    public override void MissingRequirements()
    {
        int missingGold = skillCost - Currency.inst.gold;
        if (!preReqSkill.isAcquired)
        {
            SkillInformation.inst.missingRequirementsText.text = $"Missing {preReqSkill.skillName} skill.";
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
            Currency.inst.Withdraw(skillCost);
            foreach (GameObject go in FoodieSpawner.inst.foodiePrefabs)
            {
                go.GetComponent<Foodie>().orderTime *= (1 + timeIncreasePercentage);
            }
            CompleteSkill();
        }
        else
        {
            Debug.LogError("There is absolutely no way this should be displayed in the console. The player has pressed confirm without achieving the requirements");
        }
    }
}
