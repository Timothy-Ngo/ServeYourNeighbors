using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class UpgradeConfirmation : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI cost;
    
    [SerializeField] string tablesDesc;
    [SerializeField] string stationsDesc;
    [SerializeField] string animatronicDesc;
    [SerializeField] string changeLayoutDesc;

    [SerializeField] string maxedTablesDesc;
    [SerializeField] string maxedStationsDesc;
    [SerializeField] string maxedAnimatronicDesc;

    int tablesCost;
    int stationsCost;
    int animatronicCost;
    int changeLayoutCost;

    [SerializeField] Button buyButton;
    Button originalButton;
    UnityAction[] oldListeners;

    public void Start()
    {
        tablesCost = Upgrades.inst.tablesUpgradeCost;
        stationsCost = Upgrades.inst.cookStationsUpgradeCost;
        animatronicCost = Upgrades.inst.animatronicUpgradeCost;
        changeLayoutCost = Upgrades.inst.changeLayoutCost;
        // TODO: Save old listeners for sfx
        ShowTablesInfo();
    }
    public void ShowTablesInfo()
    {
        if (Upgrades.inst.MaxTablesReached())
        {
            buyButton.gameObject.SetActive(false);
            description.text = maxedTablesDesc;
            cost.text = "";
            buyButton.onClick.RemoveAllListeners();
        }
        else
        {
            description.text = tablesDesc;
            buyButton.gameObject.SetActive(true);
            cost.text = $"{tablesCost}";
            if (Currency.inst.AbleToWithdraw(Upgrades.inst.tablesUpgradeCost))
            {
                buyButton.interactable = true;
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(Upgrades.inst.TablesPlacementMode);
            }
            else
            {
                buyButton.interactable = false;
                cost.text += " - Not Enough";
            }

        }
    }

    public void ShowStationsInfo()
    {
        if (Upgrades.inst.MaxCookStationsReached())
        {
            buyButton.gameObject.SetActive(false);
            description.text = maxedStationsDesc;
            cost.text = "";
            buyButton.onClick.RemoveAllListeners();
        }
        else
        {
            description.text = stationsDesc;
            buyButton.gameObject.SetActive(true);
            cost.text = $"{stationsCost}";
            if (Currency.inst.AbleToWithdraw(Upgrades.inst.cookStationsUpgradeCost))
            {
                buyButton.interactable = true;
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(Upgrades.inst.CookStationsPlacementMode);
            }
            else
            {
                buyButton.interactable = false;
                cost.text += " - Not Enough";
            }
        }
    }
    public void ShowAnimatronicInfo()
    {
        if (Upgrades.inst.MaxAnimatronicsReached())
        {
            buyButton.gameObject.SetActive(false);
            description.text = maxedAnimatronicDesc;
            cost.text = "";
            buyButton.onClick.RemoveAllListeners();
        }
        else
        {
            description.text = animatronicDesc;
            buyButton.gameObject.SetActive(true);
            cost.text = $"{animatronicCost}";
            if (Currency.inst.AbleToWithdraw(Upgrades.inst.animatronicUpgradeCost))
            {
                buyButton.interactable = true;
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(Upgrades.inst.AnimatronicPlacementMode);
            }
            else
            {
                buyButton.interactable = false;
                cost.text += " - Not Enough";
            }
        }
    }
    public void ShowChangeLayoutInfo()
    {
        description.text = changeLayoutDesc;
        buyButton.gameObject.SetActive(true);
        cost.text = $"{changeLayoutCost}";
        if (Currency.inst.AbleToWithdraw(Upgrades.inst.changeLayoutCost))
        {
            buyButton.interactable = true;
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(Upgrades.inst.ChangeLayoutMode);
        }
        else
        {
            buyButton.interactable = false;
            cost.text += " - Not Enough";
        }
    }

}
