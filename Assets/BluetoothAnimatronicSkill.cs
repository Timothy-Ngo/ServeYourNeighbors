using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluetoothAnimatronicSkill : Skill
{
    // Needs to be assigned in the inspector-------------
    [SerializeField] Skill preReqSkill;
    [SerializeField] PlayerInteraction playerInteraction;
    //---------------------------------------------------

    public void Update()
    {
        
    }
    
    public override bool CheckRequirements()
    {
        return preReqSkill.isAcquired &&
            DistractionSystem.inst.animatronicDistraction != null &&
            Currency.inst.AbleToWithdraw(skillCost);
            
    }

    public override void MissingRequirements()
    {
        int missingGold = skillCost - Currency.inst.gold;
        if (!preReqSkill.isAcquired)
        {
            SkillInformation.inst.missingRequirementsText.text = $"Missing {preReqSkill.skillName} skill.";
        }
        if (DistractionSystem.inst.animatronicDistraction == null)
        {
            SkillInformation.inst.missingRequirementsText.text += $"Missing animatronic.";
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
        playerInteraction.bluetoothSkill = true;
    }
}
