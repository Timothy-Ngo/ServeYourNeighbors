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
        SkillInformation.inst.missingRequirementsText.text = "";
        if (!preReqSkill.isAcquired)
        {
            SkillInformation.inst.missingRequirementsText.text = $"Missing {preReqSkill.skillName}";
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

    [ContextMenu("Activate Mechanic")]
    public override void ActivateMechanic()
    {
        playerInteraction.bluetoothSkill = true;
        DistractionSystem.inst.animatronicDistraction.gameObject.GetComponent<BoxCollider2D>().size = new Vector2(30, 30);
    }
}
