// Kirin Hardinger
// October 2023
// Adapted from https://www.youtube.com/watch?v=bcvLM_riVuw

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    Image timerBar;
    public float maxTime = 0, timeLeft = 0;
    bool running = false;
    bool complete = false;
    public bool paused = false;

    void Start() {
        timerBar = GetComponent<Image>();
        timerBar.enabled = false; // https://discussions.unity.com/t/how-to-make-a-ui-image-appear-disappear/163474
    }

    public void SetMaxTime(float sec) {
        maxTime = sec;
        timeLeft = sec;
        timerBar.enabled = true; // https://discussions.unity.com/t/how-to-make-a-ui-image-appear-disappear/163474
        timerBar.fillAmount = 1;
        running = true;
    }

    void Update() {
        if (paused) { 
            return; 
        } 

        if(timeLeft > 0 && running) {
            timeLeft -= Time.deltaTime;

            // image type must be "Filled" for this to work
            // https://discussions.unity.com/t/my-health-bars-image-fillamount-doesnt-change/153541
            timerBar.fillAmount = timeLeft / maxTime;
        } else if(timeLeft <= 0 && running) {
            running = false;
            complete = true;
        } else {
            timerBar.enabled = false; // https://discussions.unity.com/t/how-to-make-a-ui-image-appear-disappear/163474
        }
    }

    // can call this in Update() to check if the timer is done
    public bool TimerComplete() {
        Debug.Log("done!");
        return complete;
    }
}
