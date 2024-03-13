// Kirin Hardinger
// February 2024
// Pulled dialogue system from Intro.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [Header("-----DIALOGUES----")]
    public List<DialogueAsset> dialogueAssets;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] TextMeshProUGUI intenseText;
    [SerializeField] GameObject intenseTextObj;
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] GameObject enterPrompt;
    [SerializeField] GameObject textBox;
    [SerializeField] TextMeshProUGUI continueText;

    [Header("-----PLAYER----")]
    [SerializeField] Player player;
    [SerializeField] PickupSystem pickup;
    [SerializeField] PlayerStat playerStat;

    [Header("-----PLAYER UI-----")]
    [SerializeField] GameObject playerInterface;
    [SerializeField] GameObject dayCounter;
    [SerializeField] GameObject gold;
    [SerializeField] GameObject foodieCount;
    [SerializeField] GameObject operationsFee;
    [SerializeField] GameObject synMeter;

    [Header("-----OBJECTS AND SYSTEMS----")]
    [SerializeField] Cooking cookStation;
    [SerializeField] Cooking cookStation2;
    [SerializeField] GameObject trashCan;
    [SerializeField] GameObject grinderObj;
    [SerializeField] Grinder grinder;
    [SerializeField] GameObject lettuceBox;
    [SerializeField] GameObject flourBox;
    [SerializeField] Currency currency;
    [SerializeField] GameLoop gameLoop;
    [SerializeField] List<Counter> counters;

    [Header("-----FOODIE STUFF-----")]
    [SerializeField] GameObject foodiesParent;
    [SerializeField] GameObject tableAndChair;
    [SerializeField] GameObject tableAndChar2;
    [SerializeField] FoodieSystem foodieSystem;
    [SerializeField] GameObject animatronic;
    [SerializeField] Distraction distraction;
    [SerializeField] FoodieSpawner foodieSpawner;
    [SerializeField] GameObject tomatoFoodiePrefab;
    [SerializeField] GameObject cabbageFoodiePrefab;
    [SerializeField] GameObject breadFoodiePrefab;

    [Header("-----PAUSE GAME-----")]

    public bool skip = false;
    public bool typing = false;
    float charsPerSec = 25;
    private Foodie foodieScript;
    private Foodie foodieScript2;
    public int startServings = 0;

    void Start()
    {
        StartCoroutine(WrapperTutorialCoroutine());
        enterPrompt.SetActive(false);
        promptText.text = "";
        startServings = playerStat.foodiesServed;
        gameLoop.ActivateTutorial();
    }

    private void Update()
    {
        continueText.text = InputSystem.inst.interactKey.ToString();

        if (Input.GetKeyDown(InputSystem.inst.interactKey) && typing)
        {
            skip = true;
        }

        // shake the intense text
        // taken from Kirin Hardinger's Shake_Script.cs from a previous Unity project
        intenseTextObj.transform.localPosition += Random.insideUnitSphere * 1.0f;

        if (intenseTextObj.transform.localPosition.x < -7)
        {
            intenseTextObj.transform.localPosition = new Vector3(-7, intenseTextObj.transform.localPosition.y, intenseTextObj.transform.localPosition.z);
        }
        if (intenseTextObj.transform.localPosition.x > 3)
        {
            intenseTextObj.transform.localPosition = new Vector3(3, intenseTextObj.transform.localPosition.y, intenseTextObj.transform.localPosition.z);
        }
        if (intenseTextObj.transform.localPosition.y < -170)
        {
            intenseTextObj.transform.localPosition = new Vector3(intenseTextObj.transform.localPosition.x, -170, intenseTextObj.transform.localPosition.z);
        }
        if (intenseTextObj.transform.localPosition.y > -160)
        {
            intenseTextObj.transform.localPosition = new Vector3(intenseTextObj.transform.localPosition.x, -160, intenseTextObj.transform.localPosition.z);
        }
        intenseTextObj.transform.localPosition = new Vector3(intenseTextObj.transform.localPosition.x, intenseTextObj.transform.localPosition.y, 0);
    }

    // https://onewheelstudio.com/blog/2022/8/16/chaining-unity-coroutines-knowing-when-a-coroutine-finishes
    private IEnumerator WrapperTutorialCoroutine()
    {
        // tutorial on movement
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[0]));
        // https://stackoverflow.com/questions/35701012/disabling-a-script-attached-to-a-game-object-in-unity-c-sharp
        player.GetComponent<PlayerInteraction>().enabled = false; // only allow movement - do not let player grab items, throw away, etc.
        promptText.text = "Move around";
        yield return new WaitForSeconds(4);
        promptText.text = "";

        // tutorial on grabbing ingredients
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[1]));
        player.GetComponent<PlayerInteraction>().enabled = true; // resume player interaction
        promptText.text = "Grab a tomato";
        // https://forum.unity.com/threads/setting-system-function-bool.623533/
        yield return new WaitUntil(() => pickup.isHoldingIngredient()); // wait for player to grab tomato
        promptText.text = "";

        // tutorial on cooking
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[2]));
        promptText.text = "Cook at the blue cook station";
        yield return new WaitUntil(() => !pickup.isHoldingItem()); // wait for player to cook
        promptText.text = "Quickly press " + InputSystem.inst.cookKey.ToString() + " to cook";
        yield return new WaitUntil(() => cookStation.IsCooking());
        promptText.text = "Wait to finish cooking";
        player.GetComponent<PlayerInteraction>().enabled = false; // only allow movement - do not let player grab items, throw away, etc.
        yield return new WaitUntil(() => cookStation.IsFoodReady());
        player.GetComponent<PlayerInteraction>().enabled = true; // resume player interaction
        promptText.text = "Pick up the dish";
        yield return new WaitUntil(() => pickup.isHoldingDish());
        promptText.text = "";

        // tutorial on throwing away items
        // cookStation.GetComponent<Cooking>().enabled = false; // prevent the user from doing other stuff like cooking again
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[3]));
        promptText.text = "Throw away the dish";
        trashCan.SetActive(true);
        yield return new WaitUntil(() => !pickup.isHoldingItem());
        player.GetComponent<PlayerInteraction>().CannotThrowAway();
        promptText.text = "";

        // tutorial on serving foodies
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[4]));
        player.GetComponent<PlayerInteraction>().CannotKidnap();
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        yield return new WaitForSeconds(0.1f);
        foodieScript = foodiesParent.transform.GetChild(1).gameObject.GetComponent<Foodie>();
        promptText.text = "Grab a tomato ingredient";
        yield return new WaitUntil(() => pickup.isHoldingIngredient()); // wait for player to grab tomato
        player.GetComponent<PlayerInteraction>().CannotGetIngredient();
        promptText.text = "Cook at the blue cook station";
        yield return new WaitUntil(() => !pickup.isHoldingItem()); // wait for player to cook
        promptText.text = "Quickly press " + InputSystem.inst.cookKey.ToString() + " to cook";
        yield return new WaitUntil(() => cookStation.IsCooking());
        promptText.text = "Wait to finish cooking";
        player.GetComponent<PlayerInteraction>().enabled = false; // only allow movement - do not let player grab items, throw away, etc.
        yield return new WaitUntil(() => cookStation.IsFoodReady());
        player.GetComponent<PlayerInteraction>().enabled = true; // resume player interaction
        promptText.text = "Serve the dish";
        yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.eatState || foodieScript.stateMachine.currentFoodieState == foodieScript.leaveState);
        if (pickup.isHoldingDish())
        {
            pickup.DestroyItem();
        }
        if (cookStation.GetComponent<Cooking>().dish)
        {
            Destroy(cookStation.GetComponent<Cooking>().dish);
            cookStation.GetComponent<Cooking>().ResetCooktop();
        }
        promptText.text = "";
        yield return new WaitForSeconds(2.5f);
        if (playerStat.foodiesServed <= startServings)
        {
            // bad outcome
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[5]));
        }
        else
        {
            // good outcome
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[6]));
        }

        // tutorial on kidnapping and MSG
        yield return StartCoroutine(KidnapMSGLoop());
        promptText.text = "";

        // tutorial on foodie vision
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        yield return new WaitForSeconds(0.1f);
        foodieScript = foodiesParent.transform.GetChild(2).gameObject.GetComponent<Foodie>();
        foodieScript.ActivateTutorial();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[14]));
        promptText.text = "Press " + InputSystem.inst.foodieSightKey.ToString() + " to see customer vision";
        yield return new WaitUntil(() => Input.GetKeyDown(InputSystem.inst.foodieSightKey));
        promptText.text = "";
        yield return new WaitForSeconds(3);
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        yield return new WaitForSeconds(0.1f);
        foodieScript2 = foodiesParent.transform.GetChild(2).gameObject.GetComponent<Foodie>();
        foodieScript2.ActivateTutorial();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[15]));
        promptText.text = "Kidnap a customer";
        yield return new WaitForSeconds(3);
        player.GetComponent<PlayerInteraction>().CanKidnap();//
        yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.kidnappedState || foodieScript2.stateMachine.currentFoodieState == foodieScript2.kidnappedState);
        pickup.DestroyItem();
        for (int i = 1; i < foodiesParent.transform.childCount; i++)
        {
            Destroy(foodiesParent.transform.GetChild(i).gameObject);
        }
        foodieSystem.GetCurrentSeats();
        player.GetComponent<PlayerInteraction>().CannotKidnap();
        promptText.text = "";

        // tutorial on SYN meter and distraction
        player.transform.localPosition = new Vector3(6.5f, 9f, 0f);
        yield return StartCoroutine(DistractionLoop());
        player.GetComponent<PlayerInteraction>().CannotKidnap();

        // tutorial on new foodies
        // bread
        playerStat.reset();
        player.transform.localPosition = new Vector3(6.5f, 9f, 0f);
        yield return StartCoroutine(FlourLoop());
        player.GetComponent<PlayerInteraction>().CannotGetIngredient();
        player.GetComponent<PlayerInteraction>().CanCook();
        promptText.text = "Cook it at the blue cooking station";
        yield return new WaitUntil(() => cookStation.IsCooking());
        promptText.text = "Wait to finish cooking";
        yield return new WaitUntil(() => cookStation.IsFoodReady());
        player.GetComponent<PlayerInteraction>().CanGetDish();
        promptText.text = "Pick up the bread";
        yield return new WaitUntil(() => pickup.isHoldingDish());
        player.GetComponent<PlayerInteraction>().CannotCook();
        player.GetComponent<PlayerInteraction>().CannotGetDish();
        promptText.text = "";
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[24]));
        promptText.text = "Serve the customer";
        foodieSpawner.SpawnA(breadFoodiePrefab);
        yield return new WaitForSeconds(0.1f);
        foodieScript = foodiesParent.transform.GetChild(1).gameObject.GetComponent<Foodie>();
        yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.leaveState);
        promptText.text = "";
        if (playerStat.foodiesServed <= 0)
        {
            // bad outcome
            if (pickup.isHoldingDish())
            {
                pickup.DestroyItem();
            }
            if (cookStation.GetComponent<Cooking>().dish)
            {
                Destroy(cookStation.GetComponent<Cooking>().dish);
                cookStation.GetComponent<Cooking>().ResetCooktop();
            }
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[25]));
        }

        // salad
        playerStat.reset();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[26]));
        player.transform.localPosition = new Vector3(6.5f, 9f, 0f);
        yield return StartCoroutine(LettuceLoop());
        player.GetComponent<PlayerInteraction>().CannotGetIngredient();
        player.GetComponent<PlayerInteraction>().CanCook();
        promptText.text = "Cook it at the blue cooking station";
        yield return new WaitUntil(() => cookStation.IsCooking());
        promptText.text = "Wait to finish cooking";
        yield return new WaitUntil(() => cookStation.IsFoodReady());
        player.GetComponent<PlayerInteraction>().CanGetDish();
        promptText.text = "Pick up the salad";
        yield return new WaitUntil(() => pickup.isHoldingDish());
        player.GetComponent<PlayerInteraction>().CannotCook();
        player.GetComponent<PlayerInteraction>().CannotGetDish();
        promptText.text = "Serve the customer";
        foodieSpawner.SpawnA(cabbageFoodiePrefab);
        yield return new WaitForSeconds(0.1f);
        foodieScript = foodiesParent.transform.GetChild(1).gameObject.GetComponent<Foodie>();
        yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.leaveState);
        promptText.text = "";
        if (playerStat.foodiesServed <= 0)
        {
            // bad outcome
            if (pickup.isHoldingDish())
            {
                pickup.DestroyItem();
            }
            if (cookStation.GetComponent<Cooking>().dish)
            {
                Destroy(cookStation.GetComponent<Cooking>().dish);
                cookStation.GetComponent<Cooking>().ResetCooktop();
            }
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[25]));
        }
        promptText.text = "";

        // tutorial on counters +  free-for-all
        player.GetComponent<PlayerInteraction>().CanCook();
        player.GetComponent<PlayerInteraction>().CanGetIngredient();
        player.GetComponent<PlayerInteraction>().CanGetDish();
        distraction.ResetCharges();
        player.transform.localPosition = new Vector3(6.5f, 9f, 0f);
        player.transform.localPosition = new Vector3(6.5f, 9f, 0f);
        playerStat.reset();
        for (int i = 0; i < counters.Count; i++)
        {
            counters[i].gameObject.SetActive(true);
        }
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[27]));
        cookStation2.gameObject.SetActive(true);
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        yield return new WaitForSeconds(0.3f);
        foodieSpawner.SpawnA(cabbageFoodiePrefab);
        yield return new WaitForSeconds(0.3f);
        foodieSpawner.SpawnA(breadFoodiePrefab);
        yield return new WaitForSeconds(0.3f);
        foodieSpawner.SpawnA(cabbageFoodiePrefab);
        yield return new WaitForSeconds(0.3f);
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        yield return new WaitForSeconds(0.3f);
        foodieSpawner.SpawnA(breadFoodiePrefab);
        yield return new WaitForSeconds(0.3f);
        foodieScript = foodiesParent.transform.GetChild(6).gameObject.GetComponent<Foodie>();
        yield return new WaitUntil(() => foodiesParent.transform.childCount == 1);
        if (playerStat.successfulServings == 6)
        {
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[28]));
        }
        else if(playerStat.successfulServings >= 3)
        {
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[29]));
        } 
        else
        {
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[30]));
        }


        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[31]));
        yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[32]));
        SceneManager.LoadScene("Main Menu");
    }

    private IEnumerator MoveThroughDialogue(DialogueAsset dialogueAsset)
    {
        player.GetComponent<PlayerMovement>().Freeze(); // stop player movement
        textBox.SetActive(true);
        for (int i = 0; i < dialogueAsset.dialogue.Length; i++)
        {
            enterPrompt.SetActive(false);
            string txtBuffer = "";

            foreach (char c in dialogueAsset.dialogue[i])
            {
                typing = true;
                txtBuffer += c;
                dialogueText.text = txtBuffer;
                if (!skip)
                {
                    yield return new WaitForSeconds(1 / charsPerSec);
                }
            }
            enterPrompt.SetActive(true);
            skip = false;
            typing = false;

            //The following line of code makes it so that the for loop is paused until the user presses the Enter key
            yield return new WaitUntil(() => Input.GetKeyDown(InputSystem.inst.interactKey));
            //The following line of codes make the coroutine wait for a frame so as the next WaitUntil is not skipped
            yield return null;
        }
        dialogueText.text = "";
        enterPrompt.SetActive(false);
        textBox.SetActive(false);
        player.GetComponent<PlayerMovement>().Unfreeze(); // resume player movement
    }

    private IEnumerator MoveThroughIntenseDialogue(DialogueAsset dialogueAsset)
    {
        player.GetComponent<PlayerMovement>().Freeze(); // stop player movement
        textBox.SetActive(true);
        for (int i = 0; i < dialogueAsset.dialogue.Length; i++)
        {
            enterPrompt.SetActive(false);
            string txtBuffer = "";

            foreach (char c in dialogueAsset.dialogue[i])
            {
                typing = true;
                txtBuffer += c;
                intenseText.text = txtBuffer;
                if (!skip)
                {
                    yield return new WaitForSeconds(1 / charsPerSec);
                }
            }
            enterPrompt.SetActive(true);
            skip = false;
            typing = false;

            //The following line of code makes it so that the for loop is paused until the user presses the Enter key
            yield return new WaitUntil(() => Input.GetKeyDown(InputSystem.inst.interactKey));
            //The following line of codes make the coroutine wait for a frame so as the next WaitUntil is not skipped
            yield return null;
        }
        intenseText.text = "";
        enterPrompt.SetActive(false);
        textBox.SetActive(false);
        player.GetComponent<PlayerMovement>().Unfreeze(); // resume player movement//
    }

    private IEnumerator KidnapMSGLoop()
    {
        player.GetComponent<PlayerInteraction>().CanKidnap();
        player.GetComponent<PlayerInteraction>().CannotCook();
        player.GetComponent<PlayerInteraction>().CannotGetIngredient();
        player.GetComponent<PlayerInteraction>().CannotGetMSG();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[7]));
        yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[8]));
        promptText.text = "Kidnap your customer";
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        yield return new WaitForSeconds(0.1f);
        foodieScript = foodiesParent.transform.GetChild(1).gameObject.GetComponent<Foodie>();
        foodieScript.ActivateTutorial();
        yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.kidnappedState);
        intenseText.alignment = TextAlignmentOptions.Center;
        intenseText.fontSize = 100;
        promptText.text = "";
        yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[9]));
        intenseText.fontSize = 50;
        yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[10]));
        intenseText.alignment = TextAlignmentOptions.Left;
        grinderObj.SetActive(true);
        promptText.text = "GRIND THEM UP...GRIND THEM UP...";
        yield return new WaitUntil(() => grinder.IsGrindingDone());
        promptText.text = "";
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[11]));
        yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[12]));//
        promptText.text = "Cook a tomato soup";
        player.GetComponent<PlayerInteraction>().CanCook();
        player.GetComponent<PlayerInteraction>().CanGetIngredient();
        yield return new WaitUntil(() => pickup.isHoldingIngredient());
        player.GetComponent<PlayerInteraction>().CannotGetIngredient();
        yield return new WaitUntil(() => cookStation.IsFoodReady());
        promptText.text = "Pick up the MSG";
        player.GetComponent<PlayerInteraction>().CanGetMSG();
        player.GetComponent<PlayerInteraction>().CannotGetIngredient();
        player.GetComponent<PlayerInteraction>().CannotGetDish();
        yield return new WaitUntil(() => pickup.isHoldingTopping());
        promptText.text = "Add it to the dish";
        player.GetComponent<PlayerInteraction>().CanGetDish();
        yield return new WaitUntil(() => !pickup.isHoldingItem());
        promptText.text = "SERVE. SERVE. SERVE.";
        player.GetComponent<PlayerInteraction>().CannotKidnap();
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        yield return new WaitForSeconds(0.1f);
        foodieScript = foodiesParent.transform.GetChild(1).gameObject.GetComponent<Foodie>();
        foodieScript.DeactivateTutorial();

        yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.eatState || foodieScript.stateMachine.currentFoodieState == foodieScript.leaveState);

        if (foodieScript.stateMachine.currentFoodieState == foodieScript.leaveState)
        {
            if (pickup.isHoldingDish())
            {
                pickup.DestroyItem();
            }
            if (cookStation.GetComponent<Cooking>().dish)
            {
                Destroy(cookStation.GetComponent<Cooking>().dish);
                cookStation.GetComponent<Cooking>().ResetCooktop();
            }
            promptText.text = "";
            yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[13]));
            yield return StartCoroutine(KidnapMSGLoop());
        }
    }

    private IEnumerator DistractionLoop()
    {
        synMeter.SetActive(true);
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[16]));
        yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[17]));
        animatronic.SetActive(true);
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[18]));
        promptText.text = "Distract your customers, then kidnap";
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        yield return new WaitForSeconds(0.1f);
        foodieScript = foodiesParent.transform.GetChild(1).gameObject.GetComponent<Foodie>();
        foodieScript.ActivateTutorial();
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        yield return new WaitForSeconds(0.1f);
        foodieScript2 = foodiesParent.transform.GetChild(2).gameObject.GetComponent<Foodie>();
        foodieScript2.ActivateTutorial();
        yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.distractedState || foodieScript2.stateMachine.currentFoodieState == foodieScript2.distractedState);
        player.GetComponent<PlayerInteraction>().CanKidnap();
        yield return new WaitForSeconds(2);
        if(foodieScript.stateMachine.currentFoodieState == foodieScript.kidnappedState || foodieScript2.stateMachine.currentFoodieState == foodieScript2.kidnappedState)
        {
            pickup.DestroyItem();
            for (int i = 1; i < foodiesParent.transform.childCount; i++)
            {
                Destroy(foodiesParent.transform.GetChild(i).gameObject);
            }
            foodieSystem.GetCurrentSeats();
            promptText.text = "";
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[20]));
        } 
        else
        {
            pickup.DestroyItem();
            for (int i = 1; i < foodiesParent.transform.childCount; i++)
            {
                Destroy(foodiesParent.transform.GetChild(i).gameObject);
            }
            distraction.ResetCharges();
            foodieSystem.GetCurrentSeats();
            promptText.text = "";
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[19]));
            yield return StartCoroutine(DistractionLoop());
        }
    }

    private IEnumerator FlourLoop()
    {
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[21]));
        flourBox.SetActive(true);
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[22]));
        player.GetComponent<PlayerInteraction>().CanGetIngredient();
        promptText.text = "Grab flour";
        yield return new WaitUntil(() => pickup.isHoldingIngredient());
        if (pickup.GetItemInHands().name == "flour_ingredient(Clone)")
        {
            promptText.text = "";
        }
        else
        {
            promptText.text = "";
            intenseText.alignment = TextAlignmentOptions.Center;
            yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[23]));
            intenseText.alignment = TextAlignmentOptions.Left;
            pickup.DestroyItem();
            yield return StartCoroutine(FlourLoop());
        }
    }
    private IEnumerator LettuceLoop()
    {
        lettuceBox.SetActive(true);
        player.GetComponent<PlayerInteraction>().CanGetIngredient();
        promptText.text = "Grab lettuce";
        yield return new WaitUntil(() => pickup.isHoldingIngredient());
        if (pickup.GetItemInHands().name == "lettuce_ingredient(Clone)")
        {
            promptText.text = "";
        }
        else
        {
            promptText.text = "";
            intenseText.alignment = TextAlignmentOptions.Center;
            yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[23]));
            intenseText.alignment = TextAlignmentOptions.Left;
            pickup.DestroyItem();
            yield return StartCoroutine(LettuceLoop());
        }
    }
}
