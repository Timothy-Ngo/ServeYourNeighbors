// Kirin Hardinger
// February 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Reviews : MonoBehaviour
{
    [Header("-----REVIEWS UI-----")]
    [SerializeField] public GameObject reviewScreen;
    [SerializeField] public GameObject review1;
    [SerializeField] public GameObject review2;
    [SerializeField] public GameObject review3;

    [Header("-----STATS-----")]
    [SerializeField] public PlayerStats playerStats;
    [SerializeField] public ArrayList events = new ArrayList(new string[] {});
    [SerializeField] private int dayDishesMade = 0;
    [SerializeField] private int dayItemsThrown = 0;
    [SerializeField] private int dayFoodiesServed = 0;
    [SerializeField] private int dayMsgAdded = 0;
    [SerializeField] private int daySuccessfulServings = 0;
    [SerializeField] private int dayFailedServings = 0;
    [SerializeField] private int dayFoodiesKidnapped = 0;
    [SerializeField] private int dayFoodiesGround = 0;
    [SerializeField] private int dayTimesDistracted = 0;
    [SerializeField] private int dayKidnappingsCaught = 0;

    [Header("-----MISC-----")]
    [SerializeField] private ArrayList randomMessages = new ArrayList(new[] {"Not much to talk about, it was just meh.", "Could be better.", "Perfectly average experience."});

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    { 
    }

    public void Launch() 
    {
        GetEvents();
        DisplayEvents();
    }

    public void GetEvents() 
    {
        // clears previous events
        events.Clear();

        // adds day events to an array of which 3 random events will be selected to remark on the reviews page
        UpdateDayStats();

        if (dayItemsThrown == 0) 
        {
            events.Add("The restaurant was so clean!!! No trash at all.");
        }
        if (dayTimesDistracted > 0) 
        {
            events.Add("A cool atmosphere! They had a dancing animatronic.");
        }
        if (dayKidnappingsCaught > 0) 
        {
            events.Add("Bad vibes... I watched the chef pick someone up...");
        }
        if (dayFailedServings > 0) 
        {
            events.Add("Horrible service! I got the wrong dish!");
        }
        if (dayMsgAdded > 0) 
        {
            events.Add("Mmm!!! So tasty. I wonder what the recipe is?");
        }
    }

    public void DisplayEvents() 
    {
        while (events.Count < 3) 
        {
            events.Add(randomMessages[Random.Range(0, 2)]);
        }
        int randInd = Random.Range(0, events.Count);
        review1.GetComponent<TMP_Text>().text = events[randInd].ToString();
        events.RemoveAt(randInd);

        randInd = Random.Range(0, events.Count);
        review2.GetComponent<TMP_Text>().text = events[randInd].ToString();
        events.RemoveAt(randInd);

        randInd = Random.Range(0, events.Count);
        review3.GetComponent<TMP_Text>().text = events[randInd].ToString();
        events.RemoveAt(randInd);
    }

    public void UpdateDayStats() 
    {
        Debug.Log("UPDATING-----------------");
        dayDishesMade = playerStats.dishesMade - dayDishesMade;
        dayItemsThrown = playerStats.itemsThrown - dayItemsThrown;
        dayFoodiesServed = playerStats.foodiesServed - dayFoodiesServed;
        dayMsgAdded = playerStats.msgAdded - dayMsgAdded;
        daySuccessfulServings = playerStats.successfulServings - daySuccessfulServings;
        dayFailedServings = playerStats.failedServings - dayFailedServings;
        dayFoodiesKidnapped = playerStats.foodiesKidnapped - dayFoodiesKidnapped;
        dayFoodiesGround = playerStats.foodiesGround - dayFoodiesGround;
        dayTimesDistracted = playerStats.timesDistracted - dayTimesDistracted;
        dayKidnappingsCaught = playerStats.kidnappingsCaught - dayKidnappingsCaught;
    }

    private void ResetDayStats() 
    {
        dayDishesMade = 0;
        dayItemsThrown = 0;
        dayFoodiesServed = 0;
        dayMsgAdded = 0;
        daySuccessfulServings = 0;
        dayFailedServings = 0;
        dayFoodiesKidnapped = 0;
        dayFoodiesGround = 0;
        dayTimesDistracted = 0;
        dayKidnappingsCaught = 0;
    }
}
