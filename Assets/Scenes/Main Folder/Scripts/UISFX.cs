using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISFX : MonoBehaviour
{
    public void Select() // I made this script for this method because I don't want to drag and drop this sfx into each and every button in the game. This lets me change the prefab so its easier.
    {
        SoundFX.inst.UISelectSFX(1f);
    }

    public void Click()
    {
        SoundFX.inst.UIClickSFX(1f);
    }
}
