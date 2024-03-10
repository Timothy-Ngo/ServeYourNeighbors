// Kirin Hardinger
// October 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    GameObject interactionMessage;
    TMP_Text messageText;
    bool canKidnap = true;
    bool canCook = true;
    bool canGetIngredient = true;
    bool canGetMSG = true;
    bool canGetDish = true;
    bool canThrowAway = true;

    // this should become an array in the future for multiple available cooktops to interact with
    //GameObject cooktop;

    bool foodieReleased = false;
    bool currentlyKidnapping = false;
    [Header("-----RANGES----")]
    bool cooktopRange = false;
    bool tableRange = false;
    bool ingredientBoxRange = false;
    bool trashCanRange = false;
    bool distractionRange = false;
    bool foodieRange = false;
    bool grinderRange = false;
    bool foodieSightRange = false;
    bool counterRange = false;

    [Header("-----SCRIPTS----")]
    Cooking cooktopScript;
    [SerializeField] private Table tableScript;
    IngredientBox ingredientBoxScript;
    Distraction distractionScript;
    Foodie foodieScript;
    Grinder grinderScript;
    Counter counterScript;
    [SerializeField] public PlayerStat playerStats;
    [SerializeField] SYNMeter synMeter;
    [SerializeField] QTKidnap qtKidnap;

    [Header("-----OTHERS----")]
    public bool bluetoothSkill = false;

    private void Start()
    {
        interactionMessage = GameObject.Find("InteractionPrompt");
        messageText = interactionMessage.GetComponent<TextMeshProUGUI>();
        SetInteraction(false);

        //cooktop = GameObject.Find("cook_station");
        //cooktopScript = cooktop.GetComponent<Cooking>();
    }

    private void Update()
    {
        if (canCook && cooktopRange)
        {
            if (!cooktopScript.IsPrepping() && !cooktopScript.IsCooking() && PickupSystem.inst.isHoldingIngredient() && !cooktopScript.IsFoodReady())
            {
                // use TakeAction function to display a prompt and await user interaction
                string action = "[" + InputSystem.inst.cookKey.ToString() + "] Cook";
                if (TakeAction(action, InputSystem.inst.cookKey))
                {
                    cooktopScript.SetIngredient(PickupSystem.inst.GetItemInHands());
                    cooktopScript.StartPrep();
                    PickupSystem.inst.DestroyItem();
                    SetInteraction(false);

                }
            }
            else if (cooktopScript.IsPrepping())
            {
                string prompt = "[" + InputSystem.inst.cookKey + "] Cook!!!";
                Prompt(prompt);
            }
            // if food is ready
            else if (canGetDish && cooktopScript.IsFoodReady())
            {
                string actionMSG = "[" + InputSystem.inst.interactKey.ToString() + "] Add MSG";
                string actionDish = "[" + InputSystem.inst.interactKey.ToString() + "] Get Dish";
                if (PickupSystem.inst.isHoldingTopping())
                {
                    if (TakeAction(actionMSG, InputSystem.inst.interactKey))
                    {
                        PickupSystem.inst.DestroyItem();
                        playerStats.incMSGAdded();
                        cooktopScript.dish.GetComponent<Food>().AddMSG();
                    }

                    // add value of MSG to value of dish
                    // update bool hasMSG to dish
                }
                else if (PickupSystem.inst.isHoldingItem())
                {
                    Prompt("Hands Are Full");
                }
                else if (TakeAction(actionDish, InputSystem.inst.interactKey))
                {
                    PickupSystem.inst.PickUpItem(cooktopScript.dish);
                    cooktopScript.ResetCooktop();
                    SetInteraction(false);
                    playerStats.incDishesMade();
                }
            }
            else if (!cooktopScript.IsPrepping())
            {
                SetInteraction(false);
            }
        }

        else if (tableRange)
        {
            //Debug.Log("In range of table to give dish");
            //Debug.Log(PickupSystem.inst.isHoldingDish());
            // ERROR: NullReferenceException on if line -- doesn't affect gameplay as far as I know
            // Checks if there is a foodie ordering at the table, if the player is holding a dish, and if the dish is correct
            if (tableScript.foodie != null && tableScript.foodie.stateMachine.currentFoodieState == tableScript.foodie.orderState && PickupSystem.inst.isHoldingDish()) //&& PickupSystem.inst.GetItem() == tableScript.foodie.order)
            {
                //Debug.Log("Should be giving dish");
                // need to add a check for if food is already on the table -- don't need to do this bc checks if foodie is ordering
                string action = "[" + InputSystem.inst.serveKey.ToString() + "] Give Dish";
                if (TakeAction(action, InputSystem.inst.serveKey))
                {
                    // Make dish pop up on table, 
                    // change foodie to eating state
                    //Debug.Log($"current table object: {tableScript.gameObject}");

                    GameObject dishInHands = PickupSystem.inst.GetItemInHands();
                    Sprite dishSprite = dishInHands.GetComponent<SpriteRenderer>().sprite;
                    Sprite orderSprite = tableScript.foodie.order;

                    if (dishSprite == orderSprite)
                    {
                        playerStats.incSuccessfulServings();

                        // set dish
                        tableScript.dish = dishInHands;

                        // place dish down on table
                        Vector3 offset = new Vector3(0.75f, 0.3f, 0);
                        PickupSystem.inst.PlaceItem(tableScript.transform, offset);

                        tableScript.foodie.orderState.ReceivedOrder();
                        playerStats.incFoodiesServed();
                    }
                    else
                    {
                        playerStats.incFailedServings();
                        Debug.Log("WRONG DISH GIVEN");
                        PickupSystem.inst.DestroyItem();
                        tableScript.foodie.orderState.ReceivedWrongOrder();
                    }



                    // when foodie state is in eating state -- table is set
                    //      - table gets dish from what player is holding
                    //      - player drops object

                    SetInteraction(false);
                }
            }
        }

        else if (ingredientBoxRange)
        {
            // if player isn't holding anything
            if (canGetIngredient && !PickupSystem.inst.isHoldingItem())
            {
                string action = "[" + InputSystem.inst.interactKey.ToString() + "] Get Ingredient";
                if (TakeAction(action, InputSystem.inst.interactKey))
                {
                    PickupSystem.inst.PickUpIngredient(ingredientBoxScript);
                    SetInteraction(false);
                }
            }
        }

        else if (canThrowAway && trashCanRange)
        {
            // if player is holding something
            if (PickupSystem.inst.isHoldingItem())
            {
                string action = "[" + InputSystem.inst.interactKey.ToString() + "] Throw Away";
                if (TakeAction(action, InputSystem.inst.interactKey))
                {
                    PickupSystem.inst.DestroyItem();
                    SetInteraction(false);
                    playerStats.incItemsThrown();
                }
            }
        }

        else if (distractionRange || bluetoothSkill)
        {
            Debug.Log("Distraction Range");
            if (DistractionSystem.inst.animatronicDistraction.statusText.text == "OFF" && distractionScript.ChargesAvailable())
            {
                string action = "[" + InputSystem.inst.kidnapKey.ToString() + "] Turn On";
                if (TakeAction(action, InputSystem.inst.kidnapKey) || (bluetoothSkill && Input.GetKey(InputSystem.inst.kidnapKey)))
                {
                    playerStats.incTimesDistracted();
                    distractionScript.DecrementCharges();
                    DistractionSystem.inst.StartDistraction();
                    SetInteraction(false);
                }
            }
        }

        else if (foodieRange)
        {
            // cannot kidnap if holding something or if the foodie is in line outside
            if (!PickupSystem.inst.isHoldingItem() && foodieScript.stateMachine.currentFoodieState != foodieScript.lineState && foodieScript.stateMachine.currentFoodieState != foodieScript.leaveState)
            {
                string action = "[" + InputSystem.inst.kidnapKey.ToString() + "] Kidnap";
                if (!currentlyKidnapping && canKidnap && TakeAction(action, InputSystem.inst.kidnapKey))
                {
                    currentlyKidnapping = true;
                    qtKidnap.resetEvent();
                }
                if (currentlyKidnapping && qtKidnap.isComplete())
                {
                    currentlyKidnapping = false;

                    foodieReleased = false; // flag for when player is caught kidnapping

                    foodieScript.foodieSight.SetActive(false);
                    playerStats.incFoodiesKidnapped();

                    // if foodie was at a table --> put table back into available tables
                    if (foodieScript.stateMachine.currentFoodieState == foodieScript.orderState || foodieScript.stateMachine.currentFoodieState == foodieScript.eatState)
                    {
                        FoodieSystem.inst.availableSeats.Enqueue(foodieScript.tablePosition);
                    }
                    if (foodieScript.stateMachine.currentFoodieState == foodieScript.orderState)
                    {
                        // hide the foodie's order bubble and timer bar
                        foodieScript.HideUI();
                    }

                    foodieScript.stateMachine.ChangeState(foodieScript.kidnappedState);

                    // pick up the foodie
                    PickupSystem.inst.PickUpItem(foodieScript.gameObject);

                    SetInteraction(false);
                }
            }
        }

        else if (grinderRange)
        {
            if (PickupSystem.inst.isHoldingFoodie() && !grinderScript.IsGrindingDone())
            {
                if (TakeAction("[F] Grind", KeyCode.F))
                {
                    playerStats.incFoodiesGround();
                    grinderScript.StartGrinding();
                    PickupSystem.inst.DestroyItem();
                    SetInteraction(false);
                }
            }
            else if (canGetMSG && grinderScript.timerScript.timeLeft <= 0 && grinderScript.IsGrindingDone() && !PickupSystem.inst.isHoldingItem())
            {
                string action = "[" + InputSystem.inst.interactKey.ToString() + "] Take MSG";
                if (TakeAction(action, InputSystem.inst.interactKey))
                {
                    grinderScript.TakeMSG();

                    PickupSystem.inst.PickUpItem(grinderScript.msgObject);

                    SetInteraction(false);
                }
            }
        }

        else if (counterRange)
        {
            if (PickupSystem.inst.isHoldingDish() || PickupSystem.inst.isHoldingIngredient() || PickupSystem.inst.isHoldingTopping())
            {
                if (!counterScript.Full())
                {
                    string action = "[" + InputSystem.inst.interactKey.ToString() + "] Place Item";
                    if (TakeAction(action, InputSystem.inst.interactKey))
                    {
                        //Debug.Log("Placed item on counter");
                        // set dish
                        counterScript.item = PickupSystem.inst.GetItemInHands();

                        // place dish down on table
                        Vector3 offset = new Vector3(0f, 0.3f, 0);
                        PickupSystem.inst.PlaceItem(counterScript.transform, offset);
                        counterScript.SetFull(true);

                        SetInteraction(false);
                    }
                }
                else
                {
                    // check if item on counter is dish and item in hands is MSG - if so, let them add MSG. if not, let them swap items

                    // see class type of item on counter
                    // https://www.reddit.com/r/Unity3D/comments/16q46ei/is_there_a_way_to_check_if_an_object_is_an/
                    // if item on counter is a dish and player is holding MSG
                    // Debug.Log("Item type is " + counterScript.item.GetType());
                    Food testItem = counterScript.item.GetComponent<Food>();

                    string actionMSG = "[" + InputSystem.inst.interactKey.ToString() + "] Add MSG";
                    string actionSwap = "[" + InputSystem.inst.interactKey.ToString() + "] Swap Items";
                    if (testItem != null && PickupSystem.inst.isHoldingTopping())
                    {
                        if (TakeAction(actionMSG, InputSystem.inst.interactKey))
                        {
                            PickupSystem.inst.DestroyItem();
                            playerStats.incMSGAdded();
                            counterScript.item.GetComponent<Food>().AddMSG();
                        }
                    }
                    else if (TakeAction(actionSwap, InputSystem.inst.interactKey))
                    {
                        GameObject temp = counterScript.item;

                        counterScript.item = PickupSystem.inst.GetItemInHands();
                        Vector3 offset = new Vector3(0f, 0.3f, 0);
                        PickupSystem.inst.PlaceItem(counterScript.transform, offset);
                        counterScript.SetFull(true);

                        PickupSystem.inst.PickUpItem(temp);

                        SetInteraction(false);
                    }
                }
            }
            else if (!PickupSystem.inst.isHoldingItem() && counterScript.Full())
            {
                string action = "[" + InputSystem.inst.interactKey.ToString() + "] Pick up";
                if (TakeAction(action, InputSystem.inst.interactKey))
                {
                    //Debug.Log("Picked up item from counter");
                    PickupSystem.inst.PickUpItem(counterScript.item);
                    counterScript.SetFull(false);
                    SetInteraction(false);
                }
            }
        }

        if (foodieSightRange)
        {
            if (PickupSystem.inst.isHoldingFoodie())
            {
                // flag makes sure code inside is only called once per collision
                if (!foodieReleased)
                {
                    playerStats.incKidnappingsCaught();
                    foodieReleased = true;
                    synMeter.AdjustSYN(synMeter.kidnappingSYNValue);
                    //Debug.Log("Caught kidnapping");
                    PickupSystem.inst.ReleaseFoodie();
                }

            }
        }

    }

    public void CanKidnap()
    {
        canKidnap = true;
    }

    public void CannotKidnap()
    {
        canKidnap = false;
    }

    public void CanCook()
    {
        canCook = true;
    }

    public void CannotCook()
    {
        canCook = false;
    }

    public void CanGetIngredient()
    {
        canGetIngredient = true;
    }

    public void CannotGetIngredient()
    {
        canGetIngredient = false;
    }

    public void CanGetMSG()
    {
        canGetMSG = true;
    }

    public void CannotGetMSG()
    {
        canGetMSG = false;
    }

    public void CanGetDish()
    {
        canGetDish = true;
    }

    public void CannotGetDish()
    {
        canGetDish = false;
    }

    public void CanThrowAway()
    {
        canThrowAway = true;
    }

    public void CannotThrowAway()
    {
        canThrowAway = false;
    }

    public void SetInteraction(bool status)
    {
        interactionMessage.SetActive(status);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // to create new interactions, add a tag to the collision object in question
        if (collision.tag == "Cooktop")
        { //&& !cooktopScript.IsCooking()) {
            //Debug.Log("Within range of cooktop");
            cooktopRange = true;
            cooktopScript = collision.gameObject.transform.GetComponent<Cooking>();
        }
        else if (collision.CompareTag("Table"))
        {
            //Debug.Log("Within range of table");
            tableRange = true;
            tableScript = collision.gameObject.transform.parent.GetComponent<Table>();
            //Debug.Log($"table: {tableScript}");

        }
        else if (collision.CompareTag("IngredientBox"))
        {
            //Debug.Log("Within range of ingredient box");
            ingredientBoxRange = true;
            ingredientBoxScript = collision.gameObject.transform.GetComponent<IngredientBox>();
            //Debug.Log("ingredientBox: " + collision.gameObject.name);
        }
        else if (collision.CompareTag("TrashCan"))
        {
            //Debug.Log("Within range of trash can");
            trashCanRange = true;
        }
        else if (collision.CompareTag("Distraction"))
        {
            //Debug.Log("Within range of distraction");
            distractionRange = true;
            distractionScript = collision.GetComponent<Distraction>();
            //Debug.Log("distraction: " + collision.gameObject.name);
        }
        else if (collision.CompareTag("Foodie"))
        {
            //Debug.Log("Within range of foodie");
            foodieRange = true;
            foodieScript = collision.GetComponentInParent<Foodie>();
        }
        else if (collision.CompareTag("Grinder"))
        {
            //Debug.Log("Within range of grinder");
            grinderRange = true;
            grinderScript = collision.GetComponent<Grinder>();
        }
        else if (collision.CompareTag("FoodieSight"))
        {
            //Debug.Log("Within range of foodie sight");
            foodieSightRange = true;
        }
        else if (collision.CompareTag("Counter"))
        {
            //Debug.Log("Within range of counter");
            counterRange = true;
            counterScript = collision.GetComponent<Counter>();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Cooktop")
        {
            cooktopRange = false;
            cooktopScript.StopPrep();
            //Debug.Log("Out of range of cooktop");
        }
        else if (collision.CompareTag("Table"))
        {
            tableRange = false;
            tableScript = null;
            //Debug.Log("Out of range of table");
        }
        else if (collision.CompareTag("IngredientBox"))
        {
            ingredientBoxRange = false;
            ingredientBoxScript = null;
            //Debug.Log("Out of range of ingredient box");
        }
        else if (collision.CompareTag("TrashCan"))
        {
            trashCanRange = false;
            //Debug.Log("Out of range of trash can");
        }
        else if (collision.CompareTag("Distraction"))
        {
            distractionRange = false;
            distractionScript = null;
            //Debug.Log("Out of range of distraction");
        }
        else if (collision.CompareTag("Foodie"))
        {
            foodieRange = false;
            foodieScript = null;
            if (currentlyKidnapping)
            {
                currentlyKidnapping = false;
                qtKidnap.ForceStop();
            }
            //Debug.Log("Out of range of foodie");
        }
        else if (collision.CompareTag("Grinder"))
        {
            grinderRange = false;
            grinderScript = null;
            //Debug.Log("Out of range of grinder");
        }
        else if (collision.CompareTag("FoodieSight"))
        {
            foodieSightRange = false;
            //Debug.Log("Out of range of foodie sight");
        }
        else if (collision.CompareTag("Counter"))
        {
            counterRange = false;
            //Debug.Log("Out of range of counter");
        }

        interactionMessage.SetActive(false);
    }

    // displays a given prompt and awaits user interaction
    private bool TakeAction(string prompt, KeyCode action_keycode)
    {
        SetInteraction(true);
        messageText.SetText(prompt);
        if (Input.GetKeyDown(action_keycode))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Prompt(string prompt)
    {
        SetInteraction(true);
        messageText.SetText(prompt);
    }
}
