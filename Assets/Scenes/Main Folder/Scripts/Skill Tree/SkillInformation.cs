// Author: Timothy Ngo
// Date 2/20/24
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillInformation : MonoBehaviour
{
    public static SkillInformation inst;
    public void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // These fields need to be dragged and dropped using the Skill Information Prefab------------------------------------------
    public GameObject skillInfoObj;
    public TextMeshProUGUI skillNameText; 
    public TextMeshProUGUI skillDescText;
    public TextMeshProUGUI skillCostText;
    public Button confirmButton;

    public TextMeshProUGUI missingRequirementsText;    
    public GameObject missingRequirementsObj;


   
}
