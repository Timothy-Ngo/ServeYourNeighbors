// Author: Timothy Ngo
// Date 2/20/24
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedSpeedSkill : Skill
{

    [SerializeField] PlayerMovement pm; // Needs to be assigned in the inspector
    [SerializeField] int requiredNumTables = 3;
    [SerializeField] float newSpeed = 10f;
    public void Start()
    {
        Select();
    }

    public override bool CheckRequirements()
    {
        return Currency.inst.AbleToWithdraw(skillCost) && Upgrades.inst.numTables >= requiredNumTables;
    }

    public override void MissingRequirements()
    {
        int missingTables = requiredNumTables - Upgrades.inst.numTables;
        int missingGold = skillCost - Currency.inst.gold;
        if (missingTables > 0)
        {
            SkillInformation.inst.missingRequirementsText.text = $"Missing {missingTables} tables.\n";
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
            Debug.Log("Making player go vroom vroom");
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
        pm.SetSpeed(newSpeed);
    }
}
