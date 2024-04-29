// Author: Timothy Ngo
// Date: 2/20/24

using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] string _skillName; // Keep at 19 characters max

    public string skillName
    {
        get { return _skillName; }
    }
    [SerializeField] protected string skillDesc;
    [SerializeField] protected int skillCost;

    public bool isAcquired = false;
    public abstract bool CheckRequirements(); // Determines if skill can be acquired

    public abstract void MissingRequirements(); // Determines what requirements are missing
    public abstract void Confirm();

    public abstract void ActivateMechanic();
    public void AchievedRequirements()
    {
        SkillInformation.inst.confirmButton.onClick.RemoveAllListeners();
        SkillInformation.inst.confirmButton.onClick.AddListener(Confirm);
    }
    public void CompleteSkill()
    {
        ActivateMechanic();
        isAcquired = true;
        SkillInformation.inst.confirmButton.gameObject.SetActive(false);
        SkillInformation.inst.skillCostText.text = "Skill Acquired";
        SkillInformation.inst.missingRequirementsText.text = "";
        SkillInformation.inst.exitButton.Select();
        

    }
    public void Select()
    {
        SkillInformation.inst.skillNameText.text = skillName;
        SkillInformation.inst.skillDescText.text = skillDesc;
        SkillInformation.inst.skillCostText.text = $"{skillCost}";
        if (isAcquired)
        {
            SkillInformation.inst.missingRequirementsObj.SetActive(false);
            CompleteSkill();
        }
        else
        {
            SkillInformation.inst.missingRequirementsObj.SetActive(true);
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
                SkillInformation.inst.missingRequirementsText.text = "";
                MissingRequirements();
            }
        }
    }

}
