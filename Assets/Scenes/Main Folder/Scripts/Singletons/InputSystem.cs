// Author: Helen Truong

using TMPro;
using UnityEngine;

// currently, keys can be rebound to the same key -- may change later so that each key has to be unique

public class InputSystem : MonoBehaviour, IDataPersistence
{

    public static InputSystem inst;
    private void Awake()
    {
        inst = this;
    }

    [Header("-----KEY TEXT LABELS-----")]
    /*
    public TextMeshProUGUI upKeyLabel;
    public TextMeshProUGUI downKeyLabel;
    public TextMeshProUGUI leftKeyLabel;
    public TextMeshProUGUI rightKeyLabel;
    */
    public TextMeshProUGUI interactKeyLabel;
    public TextMeshProUGUI cookKeyLabel;
    public TextMeshProUGUI kidnapKeyLabel;
    public TextMeshProUGUI serveKeyLabel;
    public TextMeshProUGUI finishPlacementKeyLabel;
    public TextMeshProUGUI pauseKeyLabel;
    public TextMeshProUGUI foodieSightKeyLabel;

    [Header("-----KEYCODES-----")]
    /*
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    */
    public KeyCode interactKey;
    public KeyCode cookKey;
    public KeyCode kidnapKey;
    public KeyCode serveKey;
    public KeyCode finishPlacementKey;
    public KeyCode pauseKey;
    public KeyCode foodieSightKey;

    [Header("-----FLAGS-----")]
    bool rebinding = false;
    bool rebindingUpKey = false;
    bool rebindingDownKey = false;
    bool rebindingLeftKey = false;
    bool rebindingRightKey = false;
    bool rebindingInteractKey = false;
    bool rebindingCookKey = false;
    bool rebindingKidnapKey = false;
    bool rebindingServeKey = false;
    bool rebindingFinishPlacementKey = false;
    bool rebindingPauseKey = false;
    bool rebindingFoodieSightKey = false;

    public void LoadData(GameData data)
    {
        /*
        upKey = data.upKey;
        downKey = data.downKey;
        leftKey = data.leftKey;
        rightKey = data.rightKey;
        */
        interactKey = data.interactKey;
        cookKey = data.cookKey;
        kidnapKey = data.kidnapKey;
        serveKey = data.serveKey;
        finishPlacementKey = data.finishPlacementKey;
        pauseKey = data.pauseKey;
        foodieSightKey = data.foodieSightKey;
    }

    public void SaveData(GameData data)
    {
        /*
        data.upKey = upKey;
        data.downKey = downKey;
        data.leftKey = leftKey;
        data.rightKey = rightKey;
        */
        data.interactKey = interactKey;
        data.cookKey = cookKey;
        data.kidnapKey = kidnapKey;
        data.serveKey = serveKey;
        data.finishPlacementKey = finishPlacementKey;
        data.pauseKey = pauseKey;
        data.foodieSightKey = foodieSightKey;
    }

    public void SetLabels()
    {
        /*
        upKeyLabel.text = upKey.ToString();
        downKeyLabel.text = downKey.ToString();
        leftKeyLabel.text = leftKey.ToString();
        rightKeyLabel.text = rightKey.ToString();
        */
        interactKeyLabel.text = interactKey.ToString();
        cookKeyLabel.text = cookKey.ToString();
        kidnapKeyLabel.text = kidnapKey.ToString();
        serveKeyLabel.text = serveKey.ToString();
        finishPlacementKeyLabel.text = finishPlacementKey.ToString();
        pauseKeyLabel.text = pauseKey.ToString();
        foodieSightKeyLabel.text = foodieSightKey.ToString();
    }

    // https://docs.unity3d.com/ScriptReference/Event-keyCode.html
    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            //Debug.Log("Detected key code: " + e.keyCode);
            
            if (rebinding)
            {
                /*
                if (rebindingUpKey)
                {
                    upKeyLabel.text = e.keyCode.ToString();
                    upKey = e.keyCode;
                    InputManager.alt
                    rebindingUpKey = false;
                    rebinding = false;
                }
                else if (rebindingDownKey)
                {
                    downKeyLabel.text = e.keyCode.ToString();
                    downKey = e.keyCode;
                    rebindingDownKey = false;
                    rebinding = false;
                }
                else if (rebindingLeftKey)
                {
                    leftKeyLabel.text = e.keyCode.ToString();
                    leftKey = e.keyCode;
                    rebindingLeftKey = false;
                    rebinding = false;
                }
                else if (rebindingRightKey)
                {
                    rightKeyLabel.text = e.keyCode.ToString();
                    rightKey = e.keyCode;
                    rebindingRightKey = false;
                    rebinding = false;
                }
                */
                if (rebindingInteractKey)
                {
                    interactKeyLabel.text = e.keyCode.ToString();
                    interactKey = e.keyCode;
                    rebindingInteractKey = false;
                    rebinding = false;
                }
                else if (rebindingCookKey)
                {
                    cookKeyLabel.text = e.keyCode.ToString();
                    cookKey = e.keyCode;
                    rebindingCookKey = false;
                    rebinding = false;
                }
                else if (rebindingKidnapKey)
                {
                    kidnapKeyLabel.text = e.keyCode.ToString();
                    kidnapKey = e.keyCode;
                    rebindingKidnapKey = false;
                    rebinding = false;
                }
                else if (rebindingServeKey)
                {
                    serveKeyLabel.text = e.keyCode.ToString();
                    serveKey = e.keyCode;
                    rebindingServeKey = false;
                    rebinding = false;
                }
                else if (rebindingFinishPlacementKey)
                {
                    finishPlacementKeyLabel.text = e.keyCode.ToString();
                    finishPlacementKey = e.keyCode;
                    rebindingFinishPlacementKey = false;
                    rebinding = false;
                }
                else if (rebindingPauseKey)
                {
                    pauseKeyLabel.text = e.keyCode.ToString();
                    pauseKey = e.keyCode;
                    rebindingPauseKey = false;
                    rebinding = false;
                }
                else if (rebindingFoodieSightKey)
                {
                    foodieSightKeyLabel.text = e.keyCode.ToString();
                    foodieSightKey = e.keyCode;
                    rebindingFoodieSightKey = false;
                    rebinding = false;
                }
            }
        }
    }

    /*
    public void RebindUpKey()
    {
        rebinding = true;

        upKeyLabel.text = "[Press Key]";

        rebindingUpKey = true;
    }

    public void RebindDownKey()
    {
        rebinding = true;

        downKeyLabel.text = "[Press Key]";

        rebindingDownKey = true;
    }
    public void RebindLeftKey()
    {
        rebinding = true;

        leftKeyLabel.text = "[Press Key]";

        rebindingLeftKey = true;
    }

    public void RebindRightKey()
    {
        rebinding = true;

        rightKeyLabel.text = "[Press Key]";

        rebindingRightKey = true;
    }
    */

    public void RebindInteractKey()
    {
        rebinding = true;

        interactKeyLabel.text = "[Press Key]";

        rebindingInteractKey = true;
    }

    public void RebindCookKey()
    {
        rebinding = true;

        cookKeyLabel.text = "[Press Key]";

        rebindingCookKey = true;
    }

    public void RebindKidnapKey()
    {
        rebinding = true;

        kidnapKeyLabel.text = "[Press Key]";

        rebindingKidnapKey = true;
    }

    public void RebindServeKey()
    {
        rebinding = true;

        serveKeyLabel.text = "[Press Key]";

        rebindingServeKey = true;
    }

    public void RebindFinishPlacementKey()
    {
        rebinding = true;

        finishPlacementKeyLabel.text = "[Press Key]";

        rebindingFinishPlacementKey = true;
    }

    public void RebindPauseKey()
    {
        rebinding = true;

        pauseKeyLabel.text = "[Press Key]";

        rebindingPauseKey = true;
    }

    public void RebindFoodieSightKey()
    {
        rebinding = true;

        foodieSightKeyLabel.text = "[Press Key]";

        rebindingFoodieSightKey = true;
    }

    public void SetDefaultKeybinding()
    {
        /*
        upKey = KeyCode.W;
        downKey = KeyCode.S;
        leftKey = KeyCode.A;
        rightKey = KeyCode.D;
        */
        interactKey = KeyCode.F;
        cookKey = KeyCode.C;
        kidnapKey = KeyCode.E;
        serveKey = KeyCode.R;
        finishPlacementKey = KeyCode.Space;
        pauseKey = KeyCode.Escape;
        foodieSightKey = KeyCode.LeftShift;

        SetLabels();
    }
}
