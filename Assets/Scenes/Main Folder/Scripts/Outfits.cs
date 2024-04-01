using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Outfits : MonoBehaviour
{

    
    public int outfit = 0;
    public Image outfitImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextOutfit()
    {
        if (outfit + 1 <= Player.inst.outfits.Count-1)
        {
            outfit++;
        }
        else
        {
            outfit = 0;
        }
        outfitImage.sprite = Player.inst.outfits[outfit];
    }

    public void BackOutfit()
    {
        if (outfit - 1 >= 0)
        {
            outfit--;
        }
        else
        {
            outfit = Player.inst.outfits.Count - 1;
        }
        outfitImage.sprite = Player.inst.outfits[outfit];
    }

    public void SetOutift()
    {
        Player.inst.outfitSR.sprite = Player.inst.outfits[outfit];
        Player.inst.outfit = outfit;
    }

}
