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
    [SerializeField] TextMeshProUGUI promptText;
    [SerializeField] GameObject enterPrompt;
    [SerializeField] GameObject textBox;

    [Header("-----PLAYER----")]
    [SerializeField] Player player;
    [SerializeField] PickupSystem pickup;
    [SerializeField] PlayerStats playerStats;

    [Header("-----PLAYER UI-----")]
    [SerializeField] GameObject playerInterface;
    [SerializeField] GameObject dayCounter;
    [SerializeField] GameObject gold;
    [SerializeField] GameObject foodieCount;
    [SerializeField] GameObject operationsFee;
    [SerializeField] GameObject synMeter;

    [Header("-----OBJECTS AND SYSTEMS----")]
    [SerializeField] Cooking cookStation;
    [SerializeField] GameObject trashCan;
    [SerializeField] FoodieSpawner foodieSpawner;
    [SerializeField] GameObject tomatoFoodiePrefab;
    [SerializeField] Currency currency;
    [SerializeField] GameLoop gameLoop;
    [SerializeField] GameObject foodiesParent;

    [Header("-----PAUSE GAME-----")]
    [SerializeField] GameObject pauseGameScreen;
    bool pauseScreenOpened = false;

    public bool skip = false;
    public bool typing = false;
    float charsPerSec = 25;
    private Foodie foodieScript;
    public int startServings = 0;

    void Start()
    {
        StartCoroutine(WrapperTutorialCoroutine());
        enterPrompt.SetActive(false);
        promptText.text = "";
        startServings = playerStats.foodiesServed;
        gameLoop.ActivateTutorial();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && typing)
        {
            skip = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // toggles pause screen open/close
            pauseGameScreen.SetActive(!pauseScreenOpened);
            pauseScreenOpened = !pauseScreenOpened;
            if (pauseScreenOpened)
                Time.timeScale = 0; // freezes gameplay
            else
                Time.timeScale = 1;
        }
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
        promptText.text = "Quickly press C to fill the bar";
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
        trashCan.SetActive(false);
        promptText.text = "";

        // tutorial on serving foodies
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[4]));
        player.GetComponent<PlayerInteraction>().CannotKidnap();
        foodieSpawner.SpawnA(tomatoFoodiePrefab);
        yield return new WaitForSeconds(0.5f);
        foodieScript = foodiesParent.transform.GetChild(1).gameObject.GetComponent<Foodie>();
        promptText.text = "Grab a tomato ingredient";
        yield return new WaitUntil(() => pickup.isHoldingIngredient()); // wait for player to grab tomato
        promptText.text = "Cook at the blue cook station";
        yield return new WaitUntil(() => !pickup.isHoldingItem()); // wait for player to cook
        promptText.text = "Quickly press C to fill the bar";
        yield return new WaitUntil(() => cookStation.IsCooking());
        promptText.text = "Wait to finish cooking";
        player.GetComponent<PlayerInteraction>().enabled = false; // only allow movement - do not let player grab items, throw away, etc.
        yield return new WaitUntil(() => cookStation.IsFoodReady());
        player.GetComponent<PlayerInteraction>().enabled = true; // resume player interaction
        promptText.text = "Serve the dish";
        yield return new WaitUntil(() => foodieScript.stateMachine.currentFoodieState == foodieScript.eatState || foodieScript.stateMachine.currentFoodieState == foodieScript.leaveState);
        if(pickup.isHoldingDish())
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
        if (playerStats.foodiesServed <= startServings)
        {
            // bad outcome
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[5]));
        }
        else
        {
            // good outcome
            yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[6]));
        }

        // tutorial on foodie vision
        yield return StartCoroutine(MoveThroughDialogue(dialogueAssets[7]));

        // SceneManager.LoadScene("Main Menu");
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
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
            //The following line of codes make the coroutine wait for a frame so as the next WaitUntil is not skipped
            yield return null;
        }
        dialogueText.text = "";
        enterPrompt.SetActive(false);
        textBox.SetActive(false);
        player.GetComponent<PlayerMovement>().Unfreeze(); // resume player movement
    }
}
