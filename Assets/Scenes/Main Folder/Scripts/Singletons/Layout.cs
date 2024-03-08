using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this system is used to initialize layouts when a new game is started
/// this system will also handle 
/// </summary>

public class Layout : MonoBehaviour, IDataPersistence
{
    public static Layout inst;

    private void Awake()
    {
        inst = this;
    }

    [Header("-----SCENE LAYOUT-----")]
    bool newLayout = true;
    int layout = 1;
    Vector3 trashcanPosition;
    Vector3 tomatoBoxPosition;
    Vector3 flourBoxPosition;
    Vector3 lettuceBoxPosition;
    Vector3 cookStationPosition;
    Vector3 grinderPosition;
    Vector3 tablePosition;

    [Header("-----LAYOUT 1-----")]
    [SerializeField] GameObject layout1;
    [SerializeField] GameObject trashcan1;
    [SerializeField] GameObject tomatoBox1;
    [SerializeField] GameObject flourBox1;
    [SerializeField] GameObject lettuceBox1;
    [SerializeField] GameObject cookStation1;
    [SerializeField] GameObject grinder1;
    [SerializeField] GameObject table1;

    [Header("-----LAYOUT 2-----")]
    [SerializeField] GameObject layout2;
    [SerializeField] GameObject trashcan2;
    [SerializeField] GameObject tomatoBox2;
    [SerializeField] GameObject flourBox2;
    [SerializeField] GameObject lettuceBox2;
    [SerializeField] GameObject cookStation2;
    [SerializeField] GameObject grinder2;
    [SerializeField] GameObject table2;

    [Header("-----LAYOUT 3-----")]
    [SerializeField] GameObject layout3;
    [SerializeField] GameObject trashcan3;
    [SerializeField] GameObject tomatoBox3;
    [SerializeField] GameObject flourBox3;
    [SerializeField] GameObject lettuceBox3;
    [SerializeField] GameObject cookStation3;
    [SerializeField] GameObject grinder3;
    [SerializeField] GameObject table3;

    [Header("-----OBJECT PARENTS-----")]
    [SerializeField] Transform trashcanParent;
    [SerializeField] Transform ingredientBoxParent;
    [SerializeField] Transform cookStationParent;
    [SerializeField] Transform grinderParent;
    [SerializeField] Transform tableParent;

    [Header("-----OBJECTS IN SCENE-----")]
    [SerializeField] GameObject trashcan;
    [SerializeField] GameObject tomatoBox;
    [SerializeField] GameObject flourBox;
    [SerializeField] GameObject lettuceBox;
    [SerializeField] GameObject cookStation;
    [SerializeField] GameObject grinder;
    [SerializeField] GameObject table;

    [Header("-----PREFABS-----")]
    [SerializeField] GameObject trashcanPrefab;
    [SerializeField] GameObject tomatoBoxPrefab;
    [SerializeField] GameObject flourBoxPrefab;
    [SerializeField] GameObject lettuceBoxPrefab;
    [SerializeField] GameObject cookStationPrefab;
    [SerializeField] GameObject grinderPrefab;
    [SerializeField] GameObject tablePrefab;

    public void LoadData(GameData data)
    {
        HideLayouts(); // hide layouts in case one is active on start up

        layout = data.layout;

        if (layout == 1)
        {
            ShowLayout1();
        }
        else if (layout == 2)
        {
            ShowLayout2();
        }
        else if (layout == 3)
        {
            ShowLayout3();
        }

        newLayout = data.newLayout;

        // if newLayout then set default
        if (newLayout)
        {
            SetDefaultPositions();
            newLayout = false;

            // move the objects to designated positions
            SetPositions();

            SaveSystem.inst.SaveGame();
        }
        else // otherwise, get saved positions
        {
            trashcanPosition = data.trashcanPosition;
            tomatoBoxPosition = data.tomatoBoxPosition;
            flourBoxPosition = data.flourBoxPosition;
            lettuceBoxPosition = data.lettuceBoxPosition;
            cookStationPosition = data.cookStationPosition;
            grinderPosition = data.grinderPosition;
            tablePosition = data.tablePosition;

            // move the objects to designated positions
            SetPositions();
        }
        
        
        
    }

    public void SaveData(GameData data)
    {
        data.layout = layout;
        data.newLayout = newLayout;

        if (!newLayout)
        {
            // get current positions of objects in the scene and save to variables
            GetPositions();

            // save current positions to save file
            data.trashcanPosition = trashcanPosition;
            data.tomatoBoxPosition = tomatoBoxPosition;
            data.flourBoxPosition = flourBoxPosition;
            data.lettuceBoxPosition = lettuceBoxPosition;
            data.cookStationPosition = cookStationPosition;
            data.grinderPosition = grinderPosition;
            data.tablePosition = tablePosition;
        }

        
        
    }

    public void ShowLayout1()
    {
        layout1.SetActive(true);
    }
    
    public void ShowLayout2()
    {
        layout2.SetActive(true);
    }
    public void ShowLayout3()
    {
        layout3.SetActive(true);
    }

    public void HideLayouts()
    {
        layout1.SetActive(false);
        layout2.SetActive(false);
        layout3.SetActive(false);
    }

    // set layout number and then save default positions -- should only be called when a new game is made
    public void SetLayout(int layoutNum)
    {
        if (layoutNum < 1 || layoutNum > 3)
        {
            Debug.Log("Error: Invalid layout number.");
        }
        else
        {
            layout = layoutNum;
            newLayout = true;
            //SetDefaultPositions();

            SaveSystem.inst.SaveGame();

            
        }
    }

    // set default positions based off of layout -- should only be called when a new game is made
    public void SetDefaultPositions()
    {
        if (layout == 1)
        {
            trashcanPosition = trashcan1.transform.position;
            tomatoBoxPosition = tomatoBox1.transform.position;
            flourBoxPosition = flourBox1.transform.position;
            lettuceBoxPosition = lettuceBox1.transform.position;
            cookStationPosition = cookStation1.transform.position;
            grinderPosition = grinder1.transform.position;
            tablePosition = table1.transform.position;
        }
        else if (layout == 2)
        {
            trashcanPosition = trashcan2.transform.position;
            tomatoBoxPosition = tomatoBox2.transform.position;
            flourBoxPosition = flourBox2.transform.position;
            lettuceBoxPosition = lettuceBox2.transform.position;
            cookStationPosition = cookStation2.transform.position;
            grinderPosition = grinder2.transform.position;
            tablePosition = table2.transform.position;
        }
        else if (layout == 3)
        {
            trashcanPosition = trashcan3.transform.position;
            tomatoBoxPosition = tomatoBox3.transform.position;
            flourBoxPosition = flourBox3.transform.position;
            lettuceBoxPosition = lettuceBox3.transform.position;
            cookStationPosition = cookStation3.transform.position;
            grinderPosition = grinder3.transform.position;
            tablePosition = table3.transform.position;
        }
        else
        {
            Debug.Log("Error: Invalid layout number -- cannot set default positions.");
        }
    }

    // instantiate starting objects based off of positions saved
    public void InstantiateLayout()
    {
        // instantiate prefabs under their parents
        Instantiate(trashcanPrefab, trashcanPosition, Quaternion.identity, trashcanParent);
        Instantiate(tomatoBoxPrefab, tomatoBoxPosition, Quaternion.identity, ingredientBoxParent);
        Instantiate(flourBoxPrefab, flourBoxPosition, Quaternion.identity, ingredientBoxParent);
        Instantiate(lettuceBoxPrefab, lettuceBoxPosition, Quaternion.identity, ingredientBoxParent);
        Instantiate(cookStationPrefab, cookStationPosition, Quaternion.identity, cookStationParent);
        Instantiate(grinderPrefab, grinderPosition, Quaternion.identity, grinderParent);
        Instantiate(tablePrefab, tablePosition, Quaternion.identity, tableParent);
    }

    // set positions of the objects in the scene
    public void SetPositions()
    {
        trashcan.transform.position = trashcanPosition;
        tomatoBox.transform.position = tomatoBoxPosition;
        flourBox.transform.position = flourBoxPosition;
        lettuceBox.transform.position = lettuceBoxPosition;
        cookStation.transform.position = cookStationPosition;
        grinder.transform.position = grinderPosition;
        table.transform.position = tablePosition;
    }

    // get positions of the objects in the scene and store in variables
    public void GetPositions()
    {
        trashcanPosition = trashcan.transform.position;
        tomatoBoxPosition = tomatoBox.transform.position;
        flourBoxPosition = flourBox.transform.position;
        lettuceBoxPosition = lettuceBox.transform.position;
        cookStationPosition = cookStation.transform.position;
        grinderPosition = grinder.transform.position;
        tablePosition = table.transform.position;
    }
}
