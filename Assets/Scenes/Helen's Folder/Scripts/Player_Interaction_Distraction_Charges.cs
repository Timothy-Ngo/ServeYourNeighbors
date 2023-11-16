// Kirin Hardinger
// October 2023

/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Interaction_Distraction_Charges : MonoBehaviour {
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
    Cooking cooktopScript;
    [SerializeField] private Table tableScript;
    IngredientBox ingredientBoxScript;
    DistractionCharges distractionScript;
    Foodie foodieScript;
    Grinder grinderScript;
    

    private void Start() {
        interactionMessage = GameObject.Find("InteractionPrompt");
        messageText = interactionMessage.GetComponent<TextMeshProUGUI>();
        SetInteraction(false);

        //cooktop = GameObject.Find("cook_station");
        //cooktopScript = cooktop.GetComponent<Cooking>();
    }

    private void Update() {
        if(cooktopRange) {
            if(!cooktopScript.IsPrepping() && !cooktopScript.IsCooking() && PickupSystem.inst.isHoldingIngredient() && !cooktopScript.IsFoodReady()) {
                // use TakeAction function to display a prompt and await user interaction
                if (TakeAction("[C] Cook", KeyCode.C)) {
                    cooktopScript.StartPrep();
                    SetInteraction(false);
                }
            } 
            else if (cooktopScript.IsPrepping()) {
                Prompt("Light it up!!!");
            }
            // if food is ready
            else if (cooktopScript.IsFoodReady()) {
                if (PickupSystem.inst.isHoldingTopping()) {
                    if(TakeAction("[F] Add MSG", KeyCode.F)) 
                    {
                        PickupSystem.inst.DropItem();
                    }
                    // add value of MSG to value of dish
                    // update bool hasMSG to dish
                }
                else if (PickupSystem.inst.isHoldingSomething())
                {
                    Prompt("Hands Are Full");
                }
                else if (TakeAction("[F] Get Dish", KeyCode.F))
                {
                    PickupSystem.inst.PickUpItem(cooktopScript.dish.GetComponent<SpriteRenderer>().sprite);
                    cooktopScript.ResetCooktop();
                    SetInteraction(false);
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
            if (tableScript.foodie != null && tableScript.foodie.stateMachine.currentFoodieState == tableScript.foodie.orderState && PickupSystem.inst.isHoldingDish()) //&& PickupSystem.inst.GetItem() == tableScript.foodie.order)
            {
                // need to add a check for if food is already on the table -- don't need to do this bc checks if foodie is ordering
                if (TakeAction("[F] Give Dish", KeyCode.F))
                {
                    // Make dish pop up on table, 
                    // change foodie to eating state
                    Debug.Log($"current table object: {tableScript.gameObject}");
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
            if (!PickupSystem.inst.isHoldingSomething())
            {
                if (TakeAction("[F] Get Ingredient", KeyCode.F))
                {
                    PickupSystem.inst.PickUpItem(ingredientBoxScript.ingredient);
                    SetInteraction(false);
                }
            }
        }

        else if (trashCanRange)
        {
            // if player is holding something
            if (PickupSystem.inst.isHoldingSomething())
            {
                if (TakeAction("[F] Throw Away", KeyCode.F))
                {
                    PickupSystem.inst.DropItem();
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
            if (!PickupSystem.inst.isHoldingSomething() && foodieScript.stateMachine.currentFoodieState != foodieScript.lineState)
            {
                if (TakeAction("[E] Kidnap", KeyCode.E))
                {
                    foodieReleased = false; // flag for when player is caught kidnapping
                    // pick up the foodie
                    PickupSystem.inst.PickUpItem(foodieScript.foodieSprite);
                    
                    // if foodie was at a table --> put table back into available tables
                    if (foodieScript.stateMachine.currentFoodieState == foodieScript.orderState || foodieScript.stateMachine.currentFoodieState == foodieScript.eatState)
                    {
                        FoodieSystem.inst.availableSeats.Enqueue(foodieScript.tablePosition);
                    }

                    // destroy foodie
                    foodieScript.DestroyFoodie(); 

                    SetInteraction(false);
                }
            }
        }

        else if (grinderRange)
        {
            if (PickupSystem.inst.isHoldingFoodie())
            {
                if (TakeAction("[F] Grind", KeyCode.F))
                {
                    PickupSystem.inst.DropItem();
                    grinderScript.StartGrinding();
                    SetInteraction(false);
                }
            }
            else if (grinderScript.timerScript.timeLeft <= 0 && grinderScript.IsGrindingDone()) 
            {
                if (TakeAction("[F] Take MSG", KeyCode.F))
                {
                    grinderScript.TakeMSG();
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
                    foodieReleased = true;
                    Debug.Log("Caught kidnapping");
                    FoodieSpawner.inst.ReleaseFoodie();
                    PickupSystem.inst.DropItem();
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
            cooktopScript = collision.gameObject.transform.GetComponent<Cooking>();
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
            ingredientBoxScript = collision.gameObject.transform.GetComponent<IngredientBox>();
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
            grinderScript = collision.GetComponent<Grinder>();
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
            distractionScript = collision.GetComponent<DistractionCharges>();
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

*/