// Kirin Hardinger
// October 2023
// Adapted from https://www.youtube.com/watch?v=bcvLM_riVuw

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    Image timerBar;
    float maxTime = 0, timeLeft = 0;

    void Start() {
        timerBar = GetComponent<Image>();
        timerBar.enabled = false; // https://discussions.unity.com/t/how-to-make-a-ui-image-appear-disappear/163474
    }

    public void SetMaxTime(int sec) {
        maxTime = sec;
        timeLeft = sec;
        timerBar.fillAmount = 1;
        timerBar.enabled = true; // https://discussions.unity.com/t/how-to-make-a-ui-image-appear-disappear/163474
    }

    void Update() {
        if (timeLeft > 0) {
            timeLeft -= Time.deltaTime;

            // image type must be "Filled" for this to work
            // https://discussions.unity.com/t/my-health-bars-image-fillamount-doesnt-change/153541
            timerBar.fillAmount = timeLeft / maxTime;
        }
        else {
            timerBar.enabled = false; // https://discussions.unity.com/t/how-to-make-a-ui-image-appear-disappear/163474
        }
    }
}
