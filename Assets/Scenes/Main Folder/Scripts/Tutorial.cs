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
    [SerializeField] GameObject synMeter;
    [SerializeField] TextMeshProUGUI foodieNumText;

    [Header("-----OBJECTS AND SYSTEMS----")]
    [SerializeField] Cooking cookStation;
    [SerializeField] Cooking cookStation2;
    [SerializeField] Grinder grinder;
    [SerializeField] GameObject lettuceBox;
    [SerializeField] GameObject flourBox;
    [SerializeField] GameLoop gameLoop;
    [SerializeField] GameObject upgradesObj;
    [SerializeField] GameObject skillTreeObj;
    [SerializeField] List<Counter> counters;
    [SerializeField] GameObject secondTable;
    [SerializeField] GameObject animatronic;
    [SerializeField] DistractionSystem distractionSystem;

    [Header("-----FOODIE STUFF-----")]
    [SerializeField] GameObject foodiesParent;
    [SerializeField] FoodieSystem foodieSystem;
    [SerializeField] FoodieSpawner foodieSpawner;
    [SerializeField] GameObject tomatoFoodiePrefab;
    [SerializeField] GameObject cabbageFoodiePrefab;
    [SerializeField] GameObject breadFoodiePrefab;

    public bool skip = false;
    public bool typing = false;
    float charsPerSec = 25;
    [SerializeField] private Foodie foodieScript;
    [SerializeField] private Foodie foodieScript2;
    public int startServings = 0;

    void Start()
    {
        StartCoroutine(WrapperTutorialCoroutine());
        enterPrompt.SetActive(false);
        promptText.text = "";
        startServings = playerStat.foodiesServed;
        gameLoop.ActivateTutorial();

        upgradesObj.SetActive(false);
        skillTreeObj.SetActive(false);
    }

    private void Update()
    {
        continueText.text = InputSystem.inst.interactKey.ToString();

        if (Input.GetKeyDown(InputSystem.inst.interactKey) && typing)
        {
            skip = true;
        }

        if (intenseText.gameObject.activeInHierarchy)
        {
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

        // prevent game over due to syn overflow
        if (synMeter.GetComponent<SYNMeter>().fillAmount >= 0.7f)
        {
            synMeter.GetComponent<SYNMeter>().SetSYN(0.2f);
        }
    }

    private void ResetAbilities()
    {
        player.GetComponent<PlayerInteraction>().CannotGetDish();
        player.GetComponent<PlayerInteraction>().CannotCook();
        player.GetComponent<PlayerInteraction>().CannotGetIngredient();
        player.GetComponent<PlayerInteraction>().CannotGetMSG();
        player.GetComponent<PlayerInteraction>().CannotKidnap();
        player.GetComponent<PlayerInteraction>().CannotThrowAway();
        player.GetComponent<PlayerInteraction>().CannotDistract();
    }

    // https://onewheelstudio.com/blog/2022/8/16/chaining-unity-coroutines-knowing-when-a-coroutine-finishes
    // https://stackoverflow.com/questions/35701012/disabling-a-script-attached-to-a-game-object-in-unity-c-sharp
    private IEnumerator WrapperTutorialCoroutine()
    {
        // tutorial on movement
        ResetAbilities();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[0]));
        Vector3 initialposition = player.transform.localPosition;
        promptText.text = "Move around";
        yield return new WaitUntil(() => player.transform.localPosition != initialposition);
        yield return new WaitForSeconds(4);
        promptText.text = "";

        // tutorial on cooking and ingredients
        yield return StartCoroutine(CookingLoop());
        ResetAbilities();

        // tutorial on throwing away items
        // cookStation.GetComponent<Cooking>().enabled = false; // prevent the user from doing other stuff like cooking again
        int initialItemsThrown = playerStat.itemsThrown;
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[3]));
        player.GetComponent<PlayerInteraction>().CanThrowAway();
        promptText.text = "Throw away the dish";
        yield return new WaitUntil(() => playerStat.itemsThrown > initialItemsThrown);
        promptText.text = "";
        ResetAbilities();

        // tutorial on serving foodies
        yield return StartCoroutine(ServeLoop());

        // tutorial on kidnapping and MSG
        yield return StartCoroutine(KidnapMSGLoop());
        promptText.text = "";
        ResetAbilities();

        // tutorial on foodie vision
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        int newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.1f);
        foodieScript = foodiesParent.transform.GetChild(2).gameObject.GetComponent<Foodie>();
        foodieScript.ActivateTutorial();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[14]));
        promptText.text = "Press " + InputSystem.inst.foodieSightKey.ToString() + " to see customer vision";
        yield return new WaitUntil(() => Input.GetKeyDown(InputSystem.inst.foodieSightKey));
        promptText.text = "";
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.1f);
        foodieScript2 = foodiesParent.transform.GetChild(2).gameObject.GetComponent<Foodie>();
        foodieScript2.ActivateTutorial();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[15]));
        promptText.text = "Kidnap a customer";
        player.GetComponent<PlayerInteraction>().CanKidnap();
        yield return new WaitUntil(() => foodieScript2.isScared);
        promptText.text = "";
        yield return new WaitUntil(() => foodieScript2 == null);
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[16]));
        synMeter.SetActive(true);
        yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[17]));
        promptText.text = "Dispose of the body";
        player.GetComponent<PlayerInteraction>().CanThrowAway();
        player.GetComponent<PlayerInteraction>().CanGetMSG();
        yield return new WaitUntil(() => !pickup.isHoldingItem());
        promptText.text = "";
        ResetAbilities();

        // tutorial on SYN meter and distraction
        yield return StartCoroutine(DistractionLoop());
        ResetAbilities();

        // tutorial on new foodies
        // bread
        yield return StartCoroutine(BreadLoop());
        ResetAbilities();

        // salad
        yield return StartCoroutine(SaladLoop());
        ResetAbilities();

        // tutorial on counters +  free-for-all
        foodieSystem.GetCurrentSeats();
        playerStat.reset();
        player.GetComponent<PlayerInteraction>().CanCook();
        player.GetComponent<PlayerInteraction>().CanGetIngredient();
        player.GetComponent<PlayerInteraction>().CanGetDish();
        player.GetComponent<PlayerInteraction>().CanKidnap();
        player.GetComponent<PlayerInteraction>().CanDistract();
        player.GetComponent<PlayerInteraction>().CanGetMSG();
        player.GetComponent<PlayerInteraction>().CanThrowAway();
        animatronic.GetComponent<Distraction>().ResetCharges();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[27]));
        cookStation2.gameObject.SetActive(true);
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.3f);
        foodieSpawner.SpawnA(cabbageFoodiePrefab);
        newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.3f);
        foodieSpawner.SpawnA(breadFoodiePrefab);
        newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.3f);
        foodieSpawner.SpawnA(cabbageFoodiePrefab);
        newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.3f);
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.3f);
        foodieSpawner.SpawnA(breadFoodiePrefab);
        newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.3f);
        foodieScript = foodiesParent.transform.GetChild(6).gameObject.GetComponent<Foodie>();
        yield return new WaitUntil(() => foodiesParent.transform.childCount == 1);
        if (playerStat.successfulServings >= 6)
        {
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[28]));
        }
        else if (playerStat.successfulServings >= 3)
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

    private IEnumerator CookingLoop()
    {
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[1]));
        player.GetComponent<PlayerInteraction>().CanGetIngredient();
        promptText.text = "Grab a tomato";
        // https://forum.unity.com/threads/setting-system-function-bool.623533/
        yield return new WaitUntil(() => pickup.isHoldingIngredient()); // wait for player to grab tomato
        promptText.text = "";
        ResetAbilities();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[2]));
        player.GetComponent<PlayerInteraction>().CanCook();
        promptText.text = "Cook at the blue cook station";
        yield return new WaitUntil(() => cookStation.IsPrepping());
        promptText.text = "Quickly press " + InputSystem.inst.cookKey.ToString() + " to cook";
        yield return new WaitUntil(() => cookStation.IsCooking() || !player.GetComponent<PlayerInteraction>().cooktopRange);
        if (!player.GetComponent<PlayerInteraction>().cooktopRange)
        {
            promptText.text = "";
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[33]));
            yield return StartCoroutine(CookingLoop());
        }
        else
        {
            promptText.text = "Wait to finish cooking";
            yield return new WaitUntil(() => cookStation.IsFoodReady());
            promptText.text = "Pick up the dish";
            player.GetComponent<PlayerInteraction>().CanGetDish();
            yield return new WaitUntil(() => pickup.isHoldingDish());
            promptText.text = "";
        }
    }

    private IEnumerator ServeLoop()
    {
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[4]));
        promptText.text = "Grab a tomato ingredient";
        player.GetComponent<PlayerInteraction>().CanGetIngredient();
        yield return new WaitUntil(() => pickup.isHoldingIngredient());
        ResetAbilities();
        promptText.text = "Cook at the blue cook station";
        player.GetComponent<PlayerInteraction>().CanCook();
        yield return new WaitUntil(() => cookStation.IsPrepping());
        promptText.text = "Quickly press " + InputSystem.inst.cookKey.ToString() + " to cook";
        yield return new WaitUntil(() => cookStation.IsCooking() || !player.GetComponent<PlayerInteraction>().cooktopRange);
        if (!player.GetComponent<PlayerInteraction>().cooktopRange)
        {
            promptText.text = "";
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[33]));
            yield return StartCoroutine(ServeLoop());
        }
        else
        {
            promptText.text = "Wait to finish cooking";
            yield return new WaitUntil(() => cookStation.IsFoodReady());
            foodieSpawner.SpawnA(tomatoFoodiePrefab);
            int newCount = int.Parse(foodieNumText.text) + 1;
            foodieNumText.text = newCount.ToString();
            yield return new WaitForSeconds(0.1f);
            foodieScript = foodiesParent.transform.GetChild(1).gameObject.GetComponent<Foodie>();
            player.GetComponent<PlayerInteraction>().CanGetDish();
            promptText.text = "Serve the dish";
            yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.eatState || foodieScript.stateMachine.currentFoodieState == foodieScript.leaveState);
            promptText.text = "";
            if (playerStat.foodiesServed <= startServings)
            {
                // bad outcome
                if (pickup.isHoldingItem())
                {
                    pickup.DestroyItem();
                }
                if (cookStation.GetComponent<Cooking>().dish)
                {
                    Destroy(cookStation.GetComponent<Cooking>().dish);
                    cookStation.GetComponent<Cooking>().Reset();
                }
                foreach (Counter counter in counters)
                {
                    if (counter.Full())
                    {
                        counter.Reset();
                    }
                }
                yield return new WaitUntil(() => foodieScript == null);
                yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[5]));
            }
            else
            {
                // good outcome
                yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.leaveState);
                yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[6]));
            }
            ResetAbilities();
        }
    }

    private IEnumerator KidnapMSGLoop()
    {
        ResetAbilities();
        player.GetComponent<PlayerInteraction>().CanKidnap();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[7]));
        yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[8]));
        promptText.text = "Kidnap:\n Approach and press " + InputSystem.inst.kidnapKey.ToString() + " quickly";
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        int newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
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
        grinder.gameObject.SetActive(true);
        promptText.text = "GRIND THEM UP...GRIND THEM UP...";
        player.GetComponent<PlayerInteraction>().CanGetMSG();
        yield return new WaitUntil(() => grinder.IsGrindingDone());
        promptText.text = "";
        yield return StartCoroutine(MSGCookLoop());
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[11]));
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[12]));
        player.GetComponent<PlayerInteraction>().CanGetDish();
        promptText.text = "Pick up the MSG";
        player.GetComponent<PlayerInteraction>().CanGetMSG();
        yield return new WaitUntil(() => pickup.isHoldingTopping());
        player.GetComponent<PlayerInteraction>().CanCook();
        promptText.text = "Add it to the dish";
        yield return new WaitUntil(() => cookStation.dish.GetComponent<Food>().hasMSG);
        promptText.text = "Pick up the dish";
        yield return new WaitUntil(() => !pickup.isHoldingItem());
        promptText.text = "SERVE. SERVE. SERVE.";
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.1f);
        foodieScript = foodiesParent.transform.GetChild(1).gameObject.GetComponent<Foodie>();

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
                cookStation.GetComponent<Cooking>().Reset();
            }
            foreach (Counter counter in counters)
            {
                if (counter.Full())
                {
                    counter.Reset();
                }
            }
            promptText.text = "";
            yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[13]));
            ResetAbilities();
            yield return StartCoroutine(KidnapMSGLoop());
        }
        else
        {
            yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.leaveState);
        }
    }

    private IEnumerator MSGCookLoop()
    {
        promptText.text = "";
        ResetAbilities();
        player.GetComponent<PlayerInteraction>().CanGetIngredient();
        promptText.text = "Grab a tomato";
        // https://forum.unity.com/threads/setting-system-function-bool.623533/
        yield return new WaitUntil(() => pickup.isHoldingIngredient()); // wait for player to grab tomato
        promptText.text = "";
        ResetAbilities();
        player.GetComponent<PlayerInteraction>().CanCook();
        promptText.text = "Cook at the blue cook station";
        yield return new WaitUntil(() => cookStation.IsPrepping());
        promptText.text = "Quickly press " + InputSystem.inst.cookKey.ToString() + " to cook";
        yield return new WaitUntil(() => cookStation.IsCooking() || !player.GetComponent<PlayerInteraction>().cooktopRange);
        if (!player.GetComponent<PlayerInteraction>().cooktopRange)
        {
            promptText.text = "";
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[33]));
            yield return StartCoroutine(MSGCookLoop());
            ResetAbilities();
        }
        else
        {
            promptText.text = "";
            yield return new WaitUntil(() => cookStation.IsFoodReady());
            ResetAbilities();
        }
    }

    private IEnumerator DistractionLoop()
    {
        animatronic.SetActive(true);
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[18]));
        secondTable.SetActive(true);
        foodieSystem.GetCurrentSeats();
        promptText.text = "Distract, then kidnap";
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        int newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.1f);
        foodieScript = foodiesParent.transform.GetChild(1).gameObject.GetComponent<Foodie>();
        foodieScript.ActivateTutorial();
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        newCount = int.Parse(foodieNumText.text) + 1;
        foodieNumText.text = newCount.ToString();
        yield return new WaitForSeconds(0.1f);
        foodieScript2 = foodiesParent.transform.GetChild(2).gameObject.GetComponent<Foodie>();
        foodieScript2.ActivateTutorial();
        yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.orderState && foodieScript2.stateMachine.currentFoodieState == foodieScript2.orderState);
        player.GetComponent<PlayerInteraction>().CanDistract();
        if (!foodieScript.gameObject.GetComponent<FoodieMovement>().facingRight)
        {
            foodieScript.gameObject.GetComponent<FoodieMovement>().facingRight = true;
        }
        if (!foodieScript2.gameObject.GetComponent<FoodieMovement>().facingRight)
        {
            foodieScript2.gameObject.GetComponent<FoodieMovement>().facingRight = true;
        }
        yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.distractedState || foodieScript2.stateMachine.currentFoodieState == foodieScript2.distractedState);
        player.GetComponent<PlayerInteraction>().CanKidnap();
        yield return new WaitUntil(() => distractionSystem.animatronicDistraction.distractionTrigger.enabled == false || foodieScript.stateMachine.currentFoodieState == foodieScript.kidnappedState || foodieScript2.stateMachine.currentFoodieState == foodieScript2.kidnappedState);
        if (foodieScript.stateMachine.currentFoodieState == foodieScript.kidnappedState || foodieScript2.stateMachine.currentFoodieState == foodieScript2.kidnappedState)
        {
            promptText.text = "Dispose of the body";
            player.GetComponent<PlayerInteraction>().CanGetMSG();
            player.GetComponent<PlayerInteraction>().CanThrowAway();
            yield return new WaitUntil(() => !pickup.isHoldingItem() || foodieScript.isScared || foodieScript2.isScared);
            if (foodieScript.isScared || foodieScript2.isScared)
            {
                promptText.text = "";
                yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[19]));
                promptText.text = "Dispose of the body";
                player.GetComponent<PlayerInteraction>().CanGetMSG();
                player.GetComponent<PlayerInteraction>().CanThrowAway();
                yield return new WaitUntil(() => !pickup.isHoldingItem());
                animatronic.GetComponent<Distraction>().ResetCharges();
                foodieSystem.GetCurrentSeats();
                promptText.text = "";
                ResetAbilities();
                yield return StartCoroutine(DistractionLoop());
            }
            else
            {
                promptText.text = "";
                yield return new WaitUntil(() => distractionSystem.animatronicDistraction.distractionTrigger.enabled == false);
                if (foodieScript.stateMachine.currentFoodieState == foodieScript.kidnappedState)
                {
                    foodieScript2.stateMachine.ChangeState(foodieScript2.leaveState);
                }
                else
                {
                    foodieScript.stateMachine.ChangeState(foodieScript.leaveState);
                }
                yield return new WaitUntil(() => foodieScript == null && foodieScript2 == null);
                yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[20]));
                ResetAbilities();
            }
        }
        else
        {
            foodieScript.stateMachine.ChangeState(foodieScript.leaveState);
            foodieScript2.stateMachine.ChangeState(foodieScript2.leaveState);
            animatronic.GetComponent<Distraction>().ResetCharges();
            foodieSystem.GetCurrentSeats();
            promptText.text = "";
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[19]));
            yield return new WaitUntil(() => foodieScript == null && foodieScript2 == null);
            ResetAbilities();
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
            ResetAbilities();
            promptText.text = "";
            intenseText.alignment = TextAlignmentOptions.Center;
            yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[23]));
            intenseText.alignment = TextAlignmentOptions.Left;
            pickup.DestroyItem();
            yield return StartCoroutine(FlourLoop());
        }
    }

    private IEnumerator BreadLoop()
    {
        playerStat.reset();
        if (player.transform.localPosition.x >= 6 && player.transform.localPosition.x <= 7 && player.transform.localPosition.y >= 12 && player.transform.localPosition.y <= 13)
        {
            player.transform.localPosition = new Vector3(6.5f, 10f, 0f);
        }
        yield return StartCoroutine(FlourLoop());
        player.GetComponent<PlayerInteraction>().CannotGetIngredient();
        player.GetComponent<PlayerInteraction>().CanCook();
        promptText.text = "Cook it at the blue cooking station";
        yield return new WaitUntil(() => cookStation.IsPrepping());
        yield return new WaitUntil(() => cookStation.IsCooking() || !player.GetComponent<PlayerInteraction>().cooktopRange);
        if (!player.GetComponent<PlayerInteraction>().cooktopRange)
        {
            promptText.text = "";
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[33]));
            yield return StartCoroutine(BreadLoop());
        }
        else
        {
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
            int newCount = int.Parse(foodieNumText.text) + 1;
            foodieNumText.text = newCount.ToString();
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
                    cookStation.GetComponent<Cooking>().Reset();
                }
                foreach (Counter counter in counters)
                {
                    if (counter.Full())
                    {
                        counter.Reset();
                    }
                }
                yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[25]));
            }
            ResetAbilities();
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
            ResetAbilities();
            promptText.text = "";
            intenseText.alignment = TextAlignmentOptions.Center;
            yield return StartCoroutine(MoveThroughIntenseDialogue(dialogueAssets[23]));
            intenseText.alignment = TextAlignmentOptions.Left;
            pickup.DestroyItem();
            yield return StartCoroutine(LettuceLoop());
        }
    }
    private IEnumerator SaladLoop()
    {
        playerStat.reset();
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[26]));
        if (player.transform.localPosition.x >= 8 && player.transform.localPosition.x <= 9 && player.transform.localPosition.y >= 12 && player.transform.localPosition.y <= 13)
        {
            player.transform.localPosition = new Vector3(8.5f, 10f, 0f);
        }
        yield return StartCoroutine(LettuceLoop());
        player.GetComponent<PlayerInteraction>().CannotGetIngredient();
        player.GetComponent<PlayerInteraction>().CanCook();
        promptText.text = "Cook it at the blue cooking station";
        yield return new WaitUntil(() => cookStation.IsPrepping());
        yield return new WaitUntil(() => cookStation.IsCooking() || !player.GetComponent<PlayerInteraction>().cooktopRange);
        if (!player.GetComponent<PlayerInteraction>().cooktopRange)
        {
            promptText.text = "";
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[33]));
            yield return StartCoroutine(SaladLoop());
        }
        else
        {
            promptText.text = "Wait to finish cooking";
            yield return new WaitUntil(() => cookStation.IsFoodReady());
            player.GetComponent<PlayerInteraction>().CanGetDish();
            promptText.text = "Pick up the salad";
            yield return new WaitUntil(() => pickup.isHoldingDish());
            player.GetComponent<PlayerInteraction>().CannotCook();
            player.GetComponent<PlayerInteraction>().CannotGetDish();
            promptText.text = "Serve the customer";
            foodieSpawner.SpawnA(cabbageFoodiePrefab);
            int newCount = int.Parse(foodieNumText.text) + 1;
            foodieNumText.text = newCount.ToString();
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
                    cookStation.GetComponent<Cooking>().Reset();
                }
                foreach (Counter counter in counters)
                {
                    if (counter.Full())
                    {
                        counter.Reset();
                    }
                }
                yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[25]));
            }
            promptText.text = "";
            ResetAbilities();
        }
    }
}
