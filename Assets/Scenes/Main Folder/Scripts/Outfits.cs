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
        string outfitName = "outfit" + outfit.ToString();
        Player.inst.outfitSR.sprite = Player.inst.outfits[outfit];
        Player.inst.outfit = outfit;
        Player.inst.outfitName = outfitName;
        Player.inst.outfitObject.GetComponent<Animator>().Play(Player.inst.outfitName);
        Animator playerAnimator = Player.inst.outfitObject.GetComponent<Animator>();
        AnimatorStateInfo playerAnimatorInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

        OnStateEnter(playerAnimator, playerAnimatorInfo, 8);

        //Player.inst.gameObject.GetComponent<Animator>().Play("Idle");
    }
    public void OnStateEnter(Animator playerAnimator, AnimatorStateInfo playerAnimatorInfo, int layermask)
    {
        Player.inst.outfitObject.GetComponent<Animator>().Play(Player.inst.outfitName);
    }

}
