// Kirin Hardinger
// October 2023
// Adapted from https://www.youtube.com/watch?v=rE8RKdenkf4

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QTEvent : MonoBehaviour {
    public float fillAmount;
    public float timeThreshold = 0;
    public KeyCode keycode;
    public bool active = false;
    public bool complete = false;

    [SerializeField] public GameObject prompt;

    private void Start() {
        GetComponent<Image>().fillAmount = 0;
        prompt.SetActive(false);
    }

    private void Update() {
        if(active && !complete) {
            if (Input.GetKeyDown(InputSystem.inst.cookKey)) {
                AdjustFillAmount(.3f);
            }

            timeThreshold += Time.deltaTime;

            if (fillAmount < 0) {
                fillAmount = 0;
            }

            if (timeThreshold > .5f) {
                timeThreshold = 0;
                AdjustFillAmount(-.02f);
            }

            if (fillAmount >= 1) {
                Debug.Log("QT Event complete");
                prompt.SetActive(false);
                active = false;
                complete = true;
            }

            GetComponent<Image>().fillAmount = fillAmount;
        }
    }

    public void resetEvent() {
        //Debug.Log("*********Reset!");
        fillAmount = 0f;
        prompt.SetActive(true);
        active = true;
        complete = false;
    }

    public void AdjustFillAmount(float amount)
    {
        fillAmount += amount;
    }


    public bool isComplete() {
        if(complete) {
            GetComponent<Image>().fillAmount = 0;
        }
        return complete;
    }
}
