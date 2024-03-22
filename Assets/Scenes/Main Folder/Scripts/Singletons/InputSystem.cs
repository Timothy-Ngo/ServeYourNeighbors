// Author: Helen Truong

using TMPro;
using UnityEngine;
using System.Collections.Generic;

// currently, keys can be rebound to the same key -- may change later so that each key has to be unique

public class InputSystem : MonoBehaviour, ISettingsDataPersistence
{

    public static InputSystem inst;
    private void Awake()
    {
        inst = this;
    }

    [Header("-----KEY TEXT LABELS-----")]
    public TextMeshProUGUI interactKeyLabel;
    public TextMeshProUGUI cookKeyLabel;
    public TextMeshProUGUI kidnapKeyLabel;
    public TextMeshProUGUI serveKeyLabel;
    public TextMeshProUGUI finishPlacementKeyLabel;
    public TextMeshProUGUI pauseKeyLabel;
    public TextMeshProUGUI foodieSightKeyLabel;

    public TextMeshProUGUI errorMsg;

    [Header("-----KEYCODES-----")]
    public KeyCode interactKey;
    public KeyCode cookKey;
    public KeyCode kidnapKey;
    public KeyCode serveKey;
    public KeyCode finishPlacementKey;
    public KeyCode pauseKey;
    public KeyCode foodieSightKey;

    List<KeyCode> reservedKeyCodes;

    [Header("-----FLAGS-----")]
    bool rebinding = false;
    bool rebindingInteractKey = false;
    bool rebindingCookKey = false;
    bool rebindingKidnapKey = false;
    bool rebindingServeKey = false;
    bool rebindingFinishPlacementKey = false;
    bool rebindingPauseKey = false;
    bool rebindingFoodieSightKey = false;



    public void LoadData(SettingsData data)
    {
        interactKey = data.interactKey;
        cookKey = data.cookKey;
        kidnapKey = data.kidnapKey;
        serveKey = data.serveKey;
        finishPlacementKey = data.finishPlacementKey;
        pauseKey = data.pauseKey;
        foodieSightKey = data.foodieSightKey;

        SetLabels();
    }

    public void SaveData(SettingsData data)
    {
        data.interactKey = interactKey;
        data.cookKey = cookKey;
        data.kidnapKey = kidnapKey;
        data.serveKey = serveKey;
        data.finishPlacementKey = finishPlacementKey;
        data.pauseKey = pauseKey;
        data.foodieSightKey = foodieSightKey;
    }

    void Start()
    {
        reservedKeyCodes = new List<KeyCode>();

        reservedKeyCodes.Add(KeyCode.W);
        reservedKeyCodes.Add(KeyCode.A);
        reservedKeyCodes.Add(KeyCode.S);
        reservedKeyCodes.Add(KeyCode.D);

        reservedKeyCodes.Add(KeyCode.UpArrow);
        reservedKeyCodes.Add(KeyCode.DownArrow);
        reservedKeyCodes.Add(KeyCode.LeftArrow);
        reservedKeyCodes.Add(KeyCode.RightArrow);

        reservedKeyCodes.Add(KeyCode.Escape);

        reservedKeyCodes.Add(KeyCode.Return);

        // hide error message
        errorMsg.gameObject.SetActive(false);
    }

    public void SetLabels()
    {
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
        if (e.isKey && e.keyCode != KeyCode.Return)//
        {
            //Debug.Log("Detected key code: " + e.keyCode);

            if (rebinding)
            {
                if (rebindingInteractKey)
                {
                    // if inputted key is available, set key to that
                    if (KeyCodeAvailable(interactKey, e.keyCode))
                    {
                        interactKey = e.keyCode;
                    }

                    // update text
                    interactKeyLabel.text = interactKey.ToString();

                    // reset flags
                    rebindingInteractKey = false;
                    rebinding = false;
                }
                else if (rebindingCookKey)
                {
                    // if inputted key is available, set key to that
                    if (KeyCodeAvailable(cookKey, e.keyCode))
                    {
                        cookKey = e.keyCode;
                    }

                    // update text
                    cookKeyLabel.text = cookKey.ToString();
                    
                    // reset flags
                    rebindingCookKey = false;
                    rebinding = false;
                }
                else if (rebindingKidnapKey)
                {
                    // if inputted key is available, set key to that
                    if (KeyCodeAvailable(kidnapKey, e.keyCode))
                    {
                        kidnapKey = e.keyCode;
                    }

                    // update text
                    kidnapKeyLabel.text = kidnapKey.ToString();

                    // reset flags
                    rebindingKidnapKey = false;
                    rebinding = false;
                }
                else if (rebindingServeKey)
                {
                    // if inputted key is available, set key to that
                    if (KeyCodeAvailable(serveKey, e.keyCode))
                    {
                        serveKey = e.keyCode;
                    }

                    // update text
                    serveKeyLabel.text = serveKey.ToString();
                    
                    // reset flags
                    rebindingServeKey = false;
                    rebinding = false;
                }
                else if (rebindingFinishPlacementKey)
                {
                    // if inputted key is available, set key to that
                    if (KeyCodeAvailable(finishPlacementKey, e.keyCode))
                    {
                        finishPlacementKey = e.keyCode;
                    }

                    // update text
                    finishPlacementKeyLabel.text = finishPlacementKey.ToString();
                    
                    // reset flags
                    rebindingFinishPlacementKey = false;
                    rebinding = false;
                }
                else if (rebindingPauseKey)
                {
                    // if inputted key is available, set key to that
                    if (KeyCodeAvailable(pauseKey, e.keyCode))
                    {
                        pauseKey = e.keyCode;
                    }

                    // update text
                    pauseKeyLabel.text = pauseKey.ToString();
                    
                    // reset flags
                    rebindingPauseKey = false;
                    rebinding = false;
                }
                else if (rebindingFoodieSightKey)
                {
                    // if inputted key is available, set key to that
                    if (KeyCodeAvailable(foodieSightKey, e.keyCode))
                    {
                        foodieSightKey = e.keyCode;
                    }

                    // update text
                    foodieSightKeyLabel.text = foodieSightKey.ToString();
                    
                    // reset flags
                    rebindingFoodieSightKey = false;
                    rebinding = false;
                }
            }
        }
    }

    // checks if a given keyCode is the same as any in-use keycodes
    bool KeyCodeInUse(KeyCode key)
    {
        if (key == interactKey || key == cookKey || key == kidnapKey || key == serveKey || key == finishPlacementKey || key == pauseKey || key == foodieSightKey)
        {
            return false;
        }

        return true;
    }

    // checks if a given keyCode is available to bind to
    bool KeyCodeAvailable(KeyCode prevKey, KeyCode inputtedKey)
    {
        if (prevKey != inputtedKey)
        {
            // check if keyCode is available to be used
            if (reservedKeyCodes.Contains(inputtedKey)) // if inputted key is a reserved KeyCode -- don't allow rebinding
            {
                // key binding remains unchanged

                // show error message
                errorMsg.gameObject.SetActive(true);
                errorMsg.text = "Cannot set key: key is reserved for movement or menu navigation";

                return false;
            }
            else if (!KeyCodeInUse(inputtedKey)) // if inputted key is used by another binding (aka is not available) -- don't allow rebinding
            {
                // key binding remains unchanged

                // show error message
                errorMsg.gameObject.SetActive(true);
                errorMsg.text = "Cannot set key: key is already in use";

                return false;
            }
            else // key is available
            {
                return true;
            }
        }

        // key and inputted key are the same -- key is technically available
        return true;
    }

    // functions assigned to OnClick() in inspector
    public void RebindInteractKey()
    {
        // hide error msg
        errorMsg.gameObject.SetActive(false);

        // change key label to show active rebinding state
        interactKeyLabel.text = "[Press Key]";

        // set flags
        rebinding = true;
        rebindingInteractKey = true;
    }

    public void RebindCookKey()
    {
        // hide error msg
        errorMsg.gameObject.SetActive(false);

        // change key label to show active rebinding state
        cookKeyLabel.text = "[Press Key]";

        // set flags
        rebinding = true;
        rebindingCookKey = true;
    }

    public void RebindKidnapKey()
    {
        // hide error msg
        errorMsg.gameObject.SetActive(false);

        // change key label to show active rebinding state
        kidnapKeyLabel.text = "[Press Key]";

        // set flags
        rebinding = true;
        rebindingKidnapKey = true;
    }

    public void RebindServeKey()
    {
        // hide error msg
        errorMsg.gameObject.SetActive(false);

        // change key label to show active rebinding state
        serveKeyLabel.text = "[Press Key]";

        // set flags
        rebinding = true;
        rebindingServeKey = true;
    }

    public void RebindFinishPlacementKey()
    {
        // hide error msg
        errorMsg.gameObject.SetActive(false);

        // change key label to show active rebinding state
        finishPlacementKeyLabel.text = "[Press Key]";

        // set flags
        rebinding = true;
        rebindingFinishPlacementKey = true;
    }

    public void RebindPauseKey()
    {
        // hide error msg
        errorMsg.gameObject.SetActive(false);

        // change key label to show active rebinding state
        pauseKeyLabel.text = "[Press Key]";

        // set flags
        rebinding = true;
        rebindingPauseKey = true;
    }

    public void RebindFoodieSightKey()
    {
        // hide error msg
        errorMsg.gameObject.SetActive(false);

        // change key label to show active rebinding state
        foodieSightKeyLabel.text = "[Press Key]";

        // set flags
        rebinding = true;
        rebindingFoodieSightKey = true;
    }

    public void SetDefaultKeybinding()
    {
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
