using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
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

    [SerializeField] int tablesCost;
    [SerializeField] int stationsCost;
    [SerializeField] int animatronicCost;
    [SerializeField] int changeLayoutCost;

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
        description.text = tablesDesc;
        cost.text = $"{tablesCost}";
        //buyButton = originalButton;
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(Upgrades.inst.TablesPlacementMode);
    }

    public void ShowStationsInfo()
    {
        description.text = stationsDesc;
        cost.text = $"{stationsCost}";
        //buyButton = originalButton;
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(Upgrades.inst.CookStationsPlacementMode);
    }
    public void ShowAnimatronicInfo()
    {
        description.text = animatronicDesc;
        cost.text = $"{animatronicCost}";
        //buyButton = originalButton;
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(Upgrades.inst.AnimatronicPlacementMode);
    }
    public void ShowChangeLayoutInfo()
    {
        description.text = changeLayoutDesc;
        cost.text = $"{changeLayoutCost}";
        //buyButton = originalButton;
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(Upgrades.inst.ChangeLayoutMode);
    }

}
