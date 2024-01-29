// Kirin Hardinger
// January 2024
// Adapted from https://blog.yarsalabs.com/creating-a-dialogue-system-in-unity/ and https://gamedevbeginner.com/dialogue-systems-in-unity/

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

    float charsPerSec = 25;

    void Start()
    {
        StartCoroutine(MoveThroughDialogue(dialogueAsset));
        SYN.SetActive(false);
    }

    private IEnumerator MoveThroughDialogue(DialogueAsset dialogueAsset) {
        for (int i = 0; i < dialogueAsset.dialogue.Length; i++) {
            string txtBuffer = "";

            foreach (char c in dialogueAsset.dialogue[i]) {
                txtBuffer += c;
                dialogueText.text = txtBuffer;
                yield return new WaitForSeconds(1 / charsPerSec);
            }

            //The following line of code makes it so that the for loop is paused until the user clicks the left mouse button.
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            //The following line of codes make the coroutine wait for a frame so as the next WaitUntil is not skipped
            yield return null;
        }
        dialogueText.text = "";
        SYN.SetActive(true);
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        SceneManager.LoadScene("Main Scene");
    }
}
