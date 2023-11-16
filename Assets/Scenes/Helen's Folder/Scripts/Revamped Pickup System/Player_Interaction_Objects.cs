// Kirin Hardinger
// October 2023

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Interaction_Objects : MonoBehaviour {
    GameObject interactionMessage;
    TMP_Text messageText;

    // this should become an array in the future for multiple available cooktops to interact with
    //GameObject cooktop;

    bool foodieReleased = false;
    [Header("-----RANGES----")]
    bool cooktopRange = false;
    bool tableRange = false;
    bool ingredientBoxRange = false;
    bool trashCanRange = false;
    bool distractionRange = false;
    bool foodieRange = false;
    bool grinderRange = false;
    bool foodieSightRange = false;

    [Header("-----SCRIPTS----")] 
    CookingObjects cooktopScript;
    [SerializeField] private Table tableScript;
    IngredientBoxObjects ingredientBoxScript;
    Distraction distractionScript;
    Foodie foodieScript;
    GrinderObjects grinderScript;

    [Header("-----FOOD-----")] 
    public FoodObjects food;
    

    private void Start() {
        interactionMessage = GameObject.Find("InteractionPrompt");
        messageText = interactionMessage.GetComponent<TextMeshProUGUI>();
        SetInteraction(false);

        //cooktop = GameObject.Find("cook_station");
        //cooktopScript = cooktop.GetComponent<Cooking>();
    }

    private void Update() {
        if(cooktopRange) {
            if(!cooktopScript.IsPrepping() && !cooktopScript.IsCooking() && PickupSystemObjects.inst.isHoldingIngredient() && !cooktopScript.IsFoodReady()) {
                // use TakeAction function to display a prompt and await user interaction
                if (TakeAction("[C] Cook", KeyCode.C))
                {
                    cooktopScript.SetIngredient(PickupSystemObjects.inst.GetItemInHands());
                    cooktopScript.StartPrep();
                    PickupSystemObjects.inst.DestroyItem();
                    SetInteraction(false);
                    //food = cooktopScript.gameObject.GetComponentInChildren<FoodObjects>();
                    Debug.Log(food);
                }
            } 
            else if (cooktopScript.IsPrepping()) {
                Prompt("[C] Cook!!!");
            }

            // if food is ready
            else if (cooktopScript.IsFoodReady()) {
                if (PickupSystemObjects.inst.isHoldingTopping()) {
                    if(TakeAction("[F] Add MSG", KeyCode.F)) 
                    {
                        PickupSystemObjects.inst.DestroyItem();
                        food = cooktopScript.dish.GetComponent<FoodObjects>();
                        food.AddMSG();
                        //cooktopScript.dish.GetComponent<FoodObjects>().AddMSG();
                    }
                    
                    // add value of MSG to value of dish
                    // update bool hasMSG to dish
                }
                else if (PickupSystemObjects.inst.isHoldingItem())
                {
                    Prompt("Hands Are Full");
                }
                else if (TakeAction("[F] Get Dish", KeyCode.F))
                {
                    //Player.inst.food = food;
                    bool hasMSG = Player.inst.food.hasMSG;
                    //PickupSystem.inst.PickUpItem(cooktopScript.dish.GetComponent<SpriteRenderer>().sprite);
                    PickupSystemObjects.inst.PickUpItem(cooktopScript.dish);
                    cooktopScript.ResetCooktop();
                    SetInteraction(false);
                    Player.inst.food.hasMSG = hasMSG;
                }
            }
            else if (!cooktopScript.IsPrepping()) {
                SetInteraction(false);
            } 
        }

        else if (tableRange)
        {

            // ERROR: NullReferenceException on if line -- doesn't affect gameplay as far as I know
            // Checks if there is a foodie ordering at the table, if the player is holding a dish, and if the dish is correct
            if (tableScript.foodie != null && tableScript.foodie.stateMachine.currentFoodieState == tableScript.foodie.orderState && PickupSystemObjects.inst.isHoldingDish()) //&& PickupSystem.inst.GetItem() == tableScript.foodie.order)
            {
                // need to add a check for if food is already on the table -- don't need to do this bc checks if foodie is ordering
                if (TakeAction("[F] Give Dish", KeyCode.F))
                {
                    // Make dish pop up on table, 
                    // change foodie to eating state
                    Debug.Log($"current table object: {tableScript.gameObject}");

                    // set dish
                    tableScript.dish = PickupSystemObjects.inst.GetItemInHands();
                    
                    // place dish down on table
                    Vector3 offset = new Vector3(0.75f, 0.3f, 0);
                    PickupSystemObjects.inst.PlaceItem(tableScript.transform, offset);

                    tableScript.foodie.orderState.ReceivedOrder();

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
            if (!PickupSystemObjects.inst.isHoldingItem())
            {
                if (TakeAction("[F] Get Ingredient", KeyCode.F))
                {
                    PickupSystemObjects.inst.PickUpIngredient(ingredientBoxScript);
                    
                    SetInteraction(false);
                }
            }
        }

        else if (trashCanRange)
        {
            // if player is holding something
            if (PickupSystemObjects.inst.isHoldingItem())
            {
                if (TakeAction("[F] Throw Away", KeyCode.F))
                {
                    PickupSystemObjects.inst.DestroyItem();
                    SetInteraction(false);
                }
            }
        }

        else if (distractionRange)
        {
            if (DistractionSystem.inst.animatronicDistraction.statusText.text == "OFF" && distractionScript.ChargesAvailable())
            {
                if (TakeAction("[E] Turn On", KeyCode.E))
                {
                    distractionScript.DecrementCharges();
                    DistractionSystem.inst.StartDistraction();
                    SetInteraction(false);
                }
            }
        }

        else if (foodieRange)
        {
            // cannot kidnap if holding something or if the foodie is in line outside
            if (!PickupSystemObjects.inst.isHoldingItem() && foodieScript.stateMachine.currentFoodieState != foodieScript.lineState)
            {
                if (TakeAction("[E] Kidnap", KeyCode.E))
                {
                    foodieReleased = false; // flag for when player is caught kidnapping
                    //foodieScript.foodieMovement.StopMoving();
                    foodieScript.foodieSight.SetActive(false);
                    
                    // if foodie was at a table --> put table back into available tables
                    if (foodieScript.stateMachine.currentFoodieState == foodieScript.orderState || foodieScript.stateMachine.currentFoodieState == foodieScript.eatState)
                    {
                        FoodieSystem.inst.availableSeats.Enqueue(foodieScript.tablePosition);
                    }
                    if (foodieScript.stateMachine.currentFoodieState == foodieScript.eatState)
                    {
                        // makes food disappear from table
                        foodieScript.table.dish.SetActive(false); 
                        Player.inst.food.ResetDish();
                    }
                    else
                    {
                        // make this into a HideUI function in Foodie.cs
                        foodieScript.orderBubble.SetActive(false);
                        foodieScript.timerScript.gameObject.SetActive(false);
                    }

                    foodieScript.stateMachine.ChangeState(foodieScript.kidnappedState);
                    // pick up the foodie
                    PickupSystemObjects.inst.PickUpItem(foodieScript.gameObject);

                    // destroy foodie
                    //foodieScript.DestroyFoodie(); 

                    SetInteraction(false);
                }
            }
        }

        else if (grinderRange)
        {
            Debug.Log("IN GRINDER RANGE");
            if (PickupSystemObjects.inst.isHoldingFoodie())
            {
                Debug.Log("IS HOLDING FOODIE NEXT TO GRINDER");
                Debug.Log("grinderScript: " + grinderScript.gameObject.name);
                if (TakeAction("[F] Grind", KeyCode.F))
                {
                    grinderScript.StartGrinding();
                    PickupSystemObjects.inst.DestroyItem();
                    SetInteraction(false);
                }
            }
            else if (grinderScript.timerScript.timeLeft <= 0 && grinderScript.IsGrindingDone() && !PickupSystemObjects.inst.isHoldingItem()) 
            {
                if (TakeAction("[F] Take MSG", KeyCode.F))
                {
                    grinderScript.TakeMSG();
                    
                    PickupSystemObjects.inst.PickUpItem(grinderScript.msgObject);

                    SetInteraction(false);
                }
            }
        }

        if (foodieSightRange)
        {
            if (PickupSystemObjects.inst.isHoldingFoodie())
            {
                // flag makes sure code inside is only called once per collision
                if (!foodieReleased)
                {
                    foodieReleased = true;
                    Debug.Log("Caught kidnapping");
                    PickupSystemObjects.inst.ReleaseFoodie();
                    //FoodieSpawner.inst.ReleaseFoodie();
                    //PickupSystemObjects.inst.DropItem();
                }
                    
            }
        }

    }

    public void SetInteraction(bool status) {
        interactionMessage.SetActive(status);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // to create new interactions, add a tag to the collision object in question
        if(collision.tag == "Cooktop") { //&& !cooktopScript.IsCooking()) {
            Debug.Log("Within range of cooktop");
            cooktopRange = true;
            cooktopScript = collision.gameObject.transform.GetComponent<CookingObjects>();
        }
        else if (collision.CompareTag("Table"))
        {
            Debug.Log("Within range of table");
            tableRange = true;
            tableScript = collision.gameObject.transform.parent.GetComponent<Table>();
            Debug.Log($"table: {tableScript}");

        }
        else if (collision.CompareTag("IngredientBox"))
        {
            Debug.Log("Within range of ingredient box");
            ingredientBoxRange = true;
            ingredientBoxScript = collision.gameObject.transform.GetComponent<IngredientBoxObjects>();
            Debug.Log("ingredientBox: " + collision.gameObject.name);
        }
        else if (collision.CompareTag("TrashCan"))
        {
            Debug.Log("Within range of trash can");
            trashCanRange = true;
        }
        else if (collision.CompareTag("Distraction"))
        {
            Debug.Log("Within range of distraction");
            distractionRange = true;
            distractionScript = collision.GetComponent<Distraction>();
            Debug.Log("distraction: " + collision.gameObject.name);
        }
        else if (collision.CompareTag("Foodie"))
        {
            Debug.Log("Within range of foodie");
            foodieRange = true;
            foodieScript = collision.GetComponentInParent<Foodie>();
        }
        else if (collision.CompareTag("Grinder"))
        {
            Debug.Log("Within range of grinder");
            grinderRange = true;
            grinderScript = collision.GetComponent<GrinderObjects>();
        }
        else if (collision.CompareTag("FoodieSight"))
        {
            Debug.Log("Within range of foodie sight");
            foodieSightRange = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Cooktop") {
            cooktopRange = false;
            Debug.Log("Out of range of cooktop");
        }
        else if (collision.CompareTag("Table"))
        {
            tableRange = false;
            tableScript = null;
            Debug.Log("Out of range of table");
        }
        else if (collision.CompareTag("IngredientBox"))
        {
            ingredientBoxRange = false;
            ingredientBoxScript = null;
            Debug.Log("Out of range of ingredient box");
        }
        else if (collision.CompareTag("TrashCan"))
        {
            trashCanRange = false;
            Debug.Log("Out of range of trash can");
        }
        else if (collision.CompareTag("Distraction"))
        {
            distractionRange = false;
            distractionScript = null;
            Debug.Log("Out of range of distraction");
        }
        else if (collision.CompareTag("Foodie"))
        {
            foodieRange = false;
            foodieScript = null;
            Debug.Log("Out of range of foodie");
        }
        else if (collision.CompareTag("Grinder"))
        {
            grinderRange = false;
            grinderScript = null;
            Debug.Log("Out of range of grinder");
        }
        else if (collision.CompareTag("FoodieSight"))
        {
            foodieSightRange = false;
            Debug.Log("Out of range of foodie sight");
        }

        interactionMessage.SetActive(false);
    }
    
    
    
    // displays a given prompt and awaits user interaction
    private bool TakeAction(string prompt, KeyCode action_keycode) {
        SetInteraction(true);
        messageText.SetText(prompt);
        if(Input.GetKeyDown(action_keycode)) {
            return true;
        } else {
            return false;
        }
    }

    private void Prompt(string prompt) {
        SetInteraction(true);
        messageText.SetText(prompt);
    }
    
}
