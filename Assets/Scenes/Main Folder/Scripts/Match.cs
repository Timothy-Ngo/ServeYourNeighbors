// Kirin Hardinger
// October 2023
// Adapted from https://stackoverflow.com/questions/43802207/position-ui-to-mouse-position-make-tooltip-panel-follow-cursor

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Match : MonoBehaviour {
    public Canvas parentCanvas;
    Image match;

    void Start() {
        match = GetComponent<Image>();
        match.enabled = false;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform, Input.mousePosition,
            parentCanvas.worldCamera,
            out pos);
    }

    void Update() {
        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera,
            out movePos);
        transform.position = parentCanvas.transform.TransformPoint(movePos);
    }

    public void SetActive(bool status) {
        match.enabled = status;
    }
}
