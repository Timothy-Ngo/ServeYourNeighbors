// Author: Timothy Ngo
// Date: 3/9/24
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedWaitTimeSkill : Skill
{
    // Needs to be assigned in the inspector-------------
    [SerializeField] Skill preReqSkill1;
    [SerializeField] Skill preReqSkill2;
    [Tooltip("Value is a percentage from 0-1")]
    [SerializeField] float timeIncreasePercentage = .15f;
    //---------------------------------------------------
    public override bool CheckRequirements()
    {
        return preReqSkill1.isAcquired && 
            preReqSkill2.isAcquired &&
            Currency.inst.AbleToWithdraw(skillCost);
            
    }

    public override void MissingRequirements()
    {
        int missingGold = skillCost - Currency.inst.gold;
        SkillInformation.inst.missingRequirementsText.text = "";
        if (!preReqSkill1.isAcquired)
        {
            SkillInformation.inst.missingRequirementsText.text = $"Missing {preReqSkill1.skillName}.\r\n";
        }
        if (!preReqSkill2.isAcquired)
        {
            SkillInformation.inst.missingRequirementsText.text += $"Missing {preReqSkill2.skillName}.\r\n";
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
            CompleteSkill();
        }
        else
        {
            Debug.LogError("There is absolutely no way this should be displayed in the console. The player has pressed confirm without achieving the requirements");
        }
    }

    public override void ActivateMechanic()
    {

        foreach (GameObject go in FoodieSpawner.inst.foodiePrefabs)
        {
            go.GetComponent<Foodie>().orderTime *= (1 + timeIncreasePercentage);
        }
    }
}
