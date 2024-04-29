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
    bool canDistract = true;

    // this should become an array in the future for multiple available cooktops to interact with
    //GameObject cooktop;

    //bool foodieReleased = false;
    bool currentlyKidnapping = false;
    [Header("-----RANGES----")]
    public bool cooktopRange = false;
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
    Foodie foodieSightScript;
    Counter counterScript;
    [SerializeField] public PlayerStat playerStats;
    [SerializeField] SYNMeter synMeter;
    [SerializeField] QTKidnap qtKidnap;

    [Header("-----OTHERS----")]
    public bool bluetoothSkill = false;

    List<Collider2D> collidedObjects = new List<Collider2D>();

    public bool calledCollisionFunction = false;

    private void Start()
    {
        interactionMessage = GameObject.Find("InteractionPrompt");
        messageText = interactionMessage.GetComponent<TextMeshProUGUI>();
        SetInteraction(false);

        //cooktop = GameObject.Find("cook_station");
        //cooktopScript = cooktop.GetComponent<Cooking>();
    }
    Vector3 currentObject = Vector3.zero;
    Vector3 prevObject = Vector3.zero;
    private void Update()
    {
        Collider2D closestObject = null;
        float closestDistance = 100f;
        Collider2D prevClosestObject = null;

        

        foreach (Collider2D collidedObject in collidedObjects)
        {
            Vector3 objectPosition = collidedObject.gameObject.transform.position;
            float objectDistance = CalculateEuclideanDistance(objectPosition);
            if (objectDistance < closestDistance)
            {
                prevObject = currentObject;
                currentObject = objectPosition;
                closestDistance = objectDistance;

                prevClosestObject = closestObject;
                //Debug.Log("previous object: " + prevClosestObject.gameObject.name);
                
                
                closestObject = collidedObject;
                //Debug.Log("current object: " + closestObject.gameObject.name);

                if (prevObject != currentObject)
                {
                    
                    //Debug.Log("closest object is now: " + closestObject.gameObject.name);
                    //calledCollisionFunction = false;
                }
            }
        }

        //if (closestObject == null)
        //{
        //    calledCollisionFunction = false;
        //}
        
        if (closestObject != null )//&& !calledCollisionFunction)
        {
            //calledCollisionFunction = true;
            if (closestObject.tag == "Cooktop")
            {
                CooktopCollision();
            }
            else if (closestObject.CompareTag("Table"))
            {
                TableCollision();
            }
            else if (closestObject.CompareTag("IngredientBox"))
            {
                IngredientBoxCollision();
            }
            else if (closestObject.CompareTag("TrashCan"))
            {
                TrashcanCollision();
            }
            else if (closestObject.CompareTag("Distraction"))
            {
                DistractionCollision();
            }
            else if (closestObject.CompareTag("Foodie"))
            {
                FoodieCollision();
            }
            else if (closestObject.CompareTag("Grinder"))
            {
                GrinderCollision();
            }
            else if (closestObject.CompareTag("FoodieSight"))
            {
                FoodieSightCollision();
            }
            else if (closestObject.CompareTag("Counter"))
            {
                CounterCollision();
            }
            

        }

    }

    public void CooktopCollision()
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
                        SoundFX.inst.AddMSGSFX(1f);
                        PickupSystem.inst.DestroyItem();
                        playerStats.incMSGAdded();
                        cooktopScript.dish.GetComponent<Food>().AddMSG();
                    }
                }
                else if (PickupSystem.inst.isHoldingItem())
                {
                    Prompt("Hands Are Full");
                }
                else if (TakeAction(actionDish, InputSystem.inst.interactKey))
                {
                    SoundFX.inst.PickUpDishSFX(1f);
                    PickupSystem.inst.PickUpItem(cooktopScript.dish);
                    cooktopScript.Reset();
                    SetInteraction(false);
                    playerStats.incDishesMade();
                }
            }
            else if (!cooktopScript.IsPrepping())
            {
                SetInteraction(false);
            }
        }
    }

    public void TableCollision()
    {
        if (tableRange)
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
                    string dishName = dishInHands.GetComponent<Food>().dishName;
                    string orderName = tableScript.foodie.orderName;
                    Debug.Log(dishName);
                    Debug.Log(orderName);

                    if (dishName == orderName)
                    {
                        playerStats.incSuccessfulServings();

                        // set dish
                        tableScript.dish = dishInHands;
                        tableScript.dish.GetComponent<Food>().EatAnimation();

                        // place dish down on table
                        SoundFX.inst.ServeDishSFX(1f);
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
    }

    public void IngredientBoxCollision()
    {
        if (ingredientBoxRange)
        {
            // if player isn't holding anything
            if (canGetIngredient && !PickupSystem.inst.isHoldingItem())
            {
                string action = "[" + InputSystem.inst.interactKey.ToString() + "] Get Ingredient";
                if (TakeAction(action, InputSystem.inst.interactKey))
                {
                    SoundFX.inst.PickUpIngredientSFX(1f);
                    PickupSystem.inst.PickUpIngredient(ingredientBoxScript);
                    SetInteraction(false);
                }
            }
        }
    }

    public void TrashcanCollision()
    {
        if (canThrowAway && trashCanRange)
        {
            // if player is holding something
            if (PickupSystem.inst.isHoldingItem())
            {
                string action = "[" + InputSystem.inst.interactKey.ToString() + "] Throw Away";
                if (TakeAction(action, InputSystem.inst.interactKey))
                {
                    SoundFX.inst.ThrowAwaySFX(1f);
                    PickupSystem.inst.DestroyItem();
                    SetInteraction(false);
                    playerStats.incItemsThrown();
                }
            }
        }
    }

    public void DistractionCollision()
    {
        if (distractionRange && canDistract)
        {
            Debug.Log("Distraction Range");
            if (DistractionSystem.inst.animatronicDistraction.statusText.text == "OFF" && distractionScript.ChargesAvailable())
            {
                string action = "[" + InputSystem.inst.activateAnimatronicKey.ToString() + "] Turn On";
                if (TakeAction(action, InputSystem.inst.activateAnimatronicKey))
                {
                    playerStats.incTimesDistracted();
                    distractionScript.DecrementCharges();
                    DistractionSystem.inst.StartDistraction();
                    SetInteraction(false);
                }
            }
        }
    }

    public void FoodieCollision()
    {
        if (foodieRange)
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

                    //foodieReleased = false; // flag for when player is caught kidnapping

                    foodieScript.foodieSight.SetActive(false);
                    playerStats.incFoodiesKidnapped();

                    // if foodie was at a table --> put table back into available tables
                    if (foodieScript.stateMachine.currentFoodieState == foodieScript.orderState || foodieScript.stateMachine.currentFoodieState == foodieScript.eatState || foodieScript.stateMachine.currentFoodieState == foodieScript.distractedState)
                    {
                        FoodieSystem.inst.availableSeats.Enqueue(foodieScript.tablePosition);
                    }
                    if (foodieScript.stateMachine.currentFoodieState == foodieScript.orderState || foodieScript.stateMachine.currentFoodieState == foodieScript.distractedState)
                    {
                        // hide the foodie's order bubble and timer bar
                        foodieScript.HideUI();
                    }
                    foodieScript.timerScript.timeLeft = 0;
                    foodieScript.gameObject.GetComponent<Animator>().Play("Kidnapped");
                    foodieScript.stateMachine.ChangeState(foodieScript.kidnappedState);

                    // pick up the foodie
                    PickupSystem.inst.PickUpItem(foodieScript.gameObject);

                    SetInteraction(false);
                }
            }
        }
    }

    public void GrinderCollision()
    {
        if (grinderRange)
        {
            if (canGetMSG && PickupSystem.inst.isHoldingFoodie() && !grinderScript.IsGrindingDone())
            {
                string action = "[" + InputSystem.inst.interactKey.ToString() + "] Grind";
                if (TakeAction(action, InputSystem.inst.interactKey))
                {
                    if (PickupSystem.inst.GetItemInHands().gameObject.name == "cabbage_foodie(Clone)")
                    {
                        Debug.Log("Cabbage");
                        grinderScript.StartGrinding("Cabbage");
                    }
                    else if (PickupSystem.inst.GetItemInHands().gameObject.name == "bread_foodie(Clone)")
                    {
                        Debug.Log("Bread");
                        grinderScript.StartGrinding("Bread");
                    }
                    else if (PickupSystem.inst.GetItemInHands().gameObject.name == "tomato_foodie(Clone)")
                    {
                        Debug.Log("Tomato");
                        grinderScript.StartGrinding("Tomato");
                    }
                    playerStats.incFoodiesGround();
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
    }

    public void CounterCollision()
    {
        if (counterRange)
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

                        // play sfx
                        GameObject item = counterScript.item;
                        if (item.CompareTag("Ingredient") || item.CompareTag("Topping"))
                        {
                            SoundFX.inst.IngredientCounterPlaceSFX(1f);
                        }
                        else if (item.CompareTag("Dish"))
                        {
                            SoundFX.inst.DishCounterPlaceSFX(1f);
                        }
                        else
                        {
                            Debug.Log($"No sfx for {item.name} of tag type {item.tag}.");
                        }
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
    }

    public void FoodieSightCollision()
    {
        if (foodieSightRange)
        {
            if (PickupSystem.inst.isHoldingFoodie() && foodieSightScript.stateMachine.currentFoodieState != foodieSightScript.lineState)
            {
                Debug.Log("CAUGHT!!!!!!!!!!!!!");
                /*
                // flag makes sure code inside is only called once per collision
                if (!foodieReleased)
                {
                    playerStats.incKidnappingsCaught();
                    foodieReleased = true;
                    synMeter.AdjustSYN(synMeter.kidnappingSYNValue);
                    //Debug.Log("Caught kidnapping");
                    PickupSystem.inst.ReleaseFoodie();
                }
                */

                // flag makes sure code inside is only called once per collision
                if (!foodieSightScript.isScared)
                {
                    playerStats.incKidnappingsCaught();
                    foodieSightScript.isScared = true;
                    synMeter.AdjustSYN(synMeter.kidnappingSYNValue);
                    //Debug.Log("Caught kidnapping");
                    //PickupSystem.inst.ReleaseFoodie();

                    foodieSightScript.HideUI();
                    FoodieSystem.inst.availableSeats.Enqueue(foodieSightScript.orderState.tablePosition);
                    foodieSightScript.gameObject.GetComponent<Animator>().Play("Scared Leave");
                    foodieSightScript.stateMachine.ChangeState(foodieSightScript.leaveState);
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

    public void CanDistract()
    {
        canDistract = true;
    }

    public void CannotDistract()
    {
        canDistract = false;
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

            if ((!cooktopScript.IsPrepping() && !cooktopScript.IsCooking() && PickupSystem.inst.isHoldingIngredient() && !cooktopScript.IsFoodReady()) || (cooktopScript.IsPrepping()) || (canGetDish && cooktopScript.IsFoodReady()))
            {
                collidedObjects.Add(collision);
            }
        }
        else if (collision.CompareTag("Table"))
        {
            //Debug.Log("Within range of table");
            tableRange = true;
            tableScript = collision.gameObject.transform.parent.GetComponent<Table>();
            //Debug.Log($"table: {tableScript}");

            if (tableScript.foodie != null && tableScript.foodie.stateMachine.currentFoodieState == tableScript.foodie.orderState && PickupSystem.inst.isHoldingDish())
            {
                collidedObjects.Add(collision);
            }

        }
        else if (collision.CompareTag("IngredientBox"))
        {
            //Debug.Log("Within range of ingredient box");
            if (ingredientBoxScript != null)
            {
                if (ingredientBoxScript.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Open"))
                {
                    ingredientBoxScript.Animate("Close");

                }
            }
            ingredientBoxRange = true;
            ingredientBoxScript = collision.gameObject.transform.GetComponent<IngredientBox>();

            ingredientBoxScript.Animate("Open");
            //Debug.Log("ingredientBox: " + collision.gameObject.name);
            
            if (canGetIngredient && !PickupSystem.inst.isHoldingItem())
            {
                collidedObjects.Add(collision);
            }
        }
        else if (collision.CompareTag("TrashCan"))
        {
            //Debug.Log("Within range of trash can");
            collision.gameObject.GetComponent<Animator>().Play("Open");
            trashCanRange = true;

            if (PickupSystem.inst.isHoldingItem())
            {
                collidedObjects.Add(collision);
            }
          
        }
        else if (collision.CompareTag("Distraction"))
        {
            //Debug.Log("Within range of distraction");
            distractionRange = true;
            distractionScript = collision.GetComponent<Distraction>();
            //Debug.Log("distraction: " + collision.gameObject.name);

            if (DistractionSystem.inst.animatronicDistraction.statusText.text == "OFF" && distractionScript.ChargesAvailable())
            {
                collidedObjects.Add(collision);
            }
        }
        else if (collision.CompareTag("Foodie"))
        {
            // may have issues with the foodie moving in and out of collision
            //Debug.Log("Within range of foodie");
            foodieRange = true;
            foodieScript = collision.GetComponentInParent<Foodie>();

            if (!PickupSystem.inst.isHoldingItem() && foodieScript.stateMachine.currentFoodieState != foodieScript.lineState && foodieScript.stateMachine.currentFoodieState != foodieScript.leaveState)
            {
                collidedObjects.Add(collision);
            }
        }
        else if (collision.CompareTag("Grinder"))
        {
            //Debug.Log("Within range of grinder");
            grinderRange = true;
            grinderScript = collision.GetComponent<Grinder>();

            if ((canGetMSG && PickupSystem.inst.isHoldingFoodie() && !grinderScript.IsGrindingDone()) || (canGetMSG && grinderScript.timerScript.timeLeft <= 0 && grinderScript.IsGrindingDone() && !PickupSystem.inst.isHoldingItem()))
            {
                collidedObjects.Add(collision);
            }
        }
        else if (collision.CompareTag("FoodieSight"))
        {
            //Debug.Log("Within range of foodie sight");
            foodieSightRange = true;
            foodieSightScript = collision.GetComponentInParent<Foodie>();

            if (PickupSystem.inst.isHoldingFoodie() && foodieSightScript.stateMachine.currentFoodieState != foodieSightScript.lineState)
            {
                collidedObjects.Add(collision);
            }
        }
        else if (collision.CompareTag("Counter"))
        {
            //Debug.Log("Within range of counter");
            counterRange = true;
            counterScript = collision.GetComponent<Counter>();

            if ((PickupSystem.inst.isHoldingDish() || PickupSystem.inst.isHoldingIngredient() || PickupSystem.inst.isHoldingTopping()) || (!PickupSystem.inst.isHoldingItem() && counterScript.Full()))
            {
                collidedObjects.Add(collision);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Cooktop")
        {
            cooktopRange = false;
            if (cooktopScript.IsPrepping())
            {
                cooktopScript.StopPrep();
            }

            
            if (collidedObjects.Contains(collision)) 
            {
                collidedObjects.Remove(collision);
            }
            //Debug.Log("Out of range of cooktop");
        }
        else if (collision.CompareTag("Table"))
        {
            tableRange = false;
            tableScript = null;
            
            if (collidedObjects.Contains(collision))
            {
                collidedObjects.Remove(collision);
            }
            //Debug.Log("Out of range of table");
        }
        else if (collision.CompareTag("IngredientBox"))
        {
            
            if (ingredientBoxScript.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Open"))
            {
                ingredientBoxScript.Animate("Close");

            }
            ingredientBoxRange = false;
            ingredientBoxScript = null;

            if (collidedObjects.Contains(collision))
            {
                collidedObjects.Remove(collision);
            }
            //Debug.Log("Out of range of ingredient box");
        }
        else if (collision.CompareTag("TrashCan"))
        {
            collision.gameObject.GetComponent<Animator>().Play("Idle");
            trashCanRange = false;

            if (collidedObjects.Contains(collision))
            {
                collidedObjects.Remove(collision);
            }
            //Debug.Log("Out of range of trash can");
        }
        else if (collision.CompareTag("Distraction"))
        {
            distractionRange = false;
            distractionScript = null;

            if (collidedObjects.Contains(collision))
            {
                collidedObjects.Remove(collision);
            }
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


            if (collidedObjects.Contains(collision))
            {
                collidedObjects.Remove(collision);
            }
            //Debug.Log("Out of range of foodie");
        }
        else if (collision.CompareTag("Grinder"))
        {
            grinderRange = false;
            grinderScript = null;

            if (collidedObjects.Contains(collision))
            {
                collidedObjects.Remove(collision);
            }
            //Debug.Log("Out of range of grinder");
        }
        else if (collision.CompareTag("FoodieSight"))
        {
            foodieSightRange = false;
            foodieSightScript = null;

            if (collidedObjects.Contains(collision))
            {
                collidedObjects.Remove(collision);
            }
            //Debug.Log("Out of range of foodie sight");
        }
        else if (collision.CompareTag("Counter"))
        {
            counterRange = false;

            if (collidedObjects.Contains(collision))
            {
                collidedObjects.Remove(collision);
            }
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

    // calculates the Euclidean distance between the player and an object
    private float CalculateEuclideanDistance(Vector3 objectPosition)
    {
        Vector3 playerPosition = gameObject.transform.position;

        float xDiff = playerPosition.x - objectPosition.x;
        float yDiff = playerPosition.y - objectPosition.y;

        float euclideanDistance = Mathf.Sqrt(Mathf.Pow(xDiff, 2) + Mathf.Pow(yDiff, 2));

        return euclideanDistance;
    }
}
