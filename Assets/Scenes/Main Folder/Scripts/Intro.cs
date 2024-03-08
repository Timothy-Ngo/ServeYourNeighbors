// Kirin Hardinger
// January 2024
// Adapted from https://blog.yarsalabs.com/creating-a-dialogue-system-in-unity/ and https://gamedevbeginner.com/dialogue-systems-in-unity/
// Updated 7-FEB-2024 to add skipping functionality. Referenced https://stackoverflow.com/questions/67774045/unity-fast-forward-type-writer-effect-upon-keypress

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{
    public DialogueAsset dialogueAsset;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject SYN;
    [SerializeField] GameObject enterPrompt;
    [SerializeField] TextMeshProUGUI continueText;
    public bool skip = false;
    public bool typing = false;
    //
    float charsPerSec = 25;

    void Start()
    {
        StartCoroutine(MoveThroughDialogue(dialogueAsset));
        SYN.SetActive(false);
        enterPrompt.SetActive(false);
        continueText.text = InputSystem.inst.interactKey.ToString();
    }

    private void Update() 
    {
        if(Input.GetKeyDown(InputSystem.inst.interactKey) && typing)
        {
            skip = true;
        }

        if(Input.GetKeyDown(InputSystem.inst.pauseKey)) {
            SceneManager.LoadScene("Layouts");
        }
    }

    private IEnumerator MoveThroughDialogue(DialogueAsset dialogueAsset) {
        for (int i = 0; i < dialogueAsset.dialogue.Length; i++) 
        {
            enterPrompt.SetActive(false);
            string txtBuffer = "";

            foreach (char c in dialogueAsset.dialogue[i]) 
            {
                typing = true;
                txtBuffer += c;
                dialogueText.text = txtBuffer;
                if(!skip) 
                {
                    yield return new WaitForSeconds(1 / charsPerSec);
                }
            }
            enterPrompt.SetActive(true);
            skip = false;
            typing = false;

            //The following line of code makes it so that the for loop is paused until the user presses the interact key
            yield return new WaitUntil(() => Input.GetKeyDown(InputSystem.inst.interactKey));
            //The following line of codes make the coroutine wait for a frame so as the next WaitUntil is not skipped
            yield return null;
        }
        dialogueText.text = "";
        SYN.SetActive(true);
        enterPrompt.SetActive(false);
        yield return new WaitUntil(() => Input.GetKeyDown(InputSystem.inst.interactKey));
        SceneManager.LoadScene("Layouts");
    }
}
