// Author: Timothy Ngo
// Date 2/20/24
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FasterCookingSkill : Skill
{
    [SerializeField] int requiredNumCookStations = 3;
    [SerializeField] int newCookTime = 4; // Base cook time is 5 seconds.

    public override bool CheckRequirements()
    {
        return Currency.inst.AbleToWithdraw(skillCost) && Upgrades.inst.numCookStations >= requiredNumCookStations;
    }
    public override void MissingRequirements()
    {
        int missingCookStations = requiredNumCookStations - Upgrades.inst.numCookStations;
        int missingGold = skillCost - Currency.inst.gold;
        if (missingCookStations > 0)
        {
            SkillInformation.inst.missingRequirementsText.text = $"Missing {missingCookStations} cook stations.\n";
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
            Debug.Log("Faster cooking skill activated");
            Currency.inst.Withdraw(skillCost);
            foreach (GameObject obj in Upgrades.inst.cookStations)
            {
                Cooking cooking = obj.GetComponent<Cooking>();
                cooking.SetCookTime(newCookTime);
            }
            CompleteSkill();

        }
        else
        {
            Debug.LogError("There is absolutely no way this should be displayed in the console. The player has pressed confirm without achieving the requirements");
        }
    }
}
