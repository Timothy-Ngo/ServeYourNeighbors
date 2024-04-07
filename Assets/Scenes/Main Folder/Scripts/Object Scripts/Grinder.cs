// Author: Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grinder : MonoBehaviour
{
    public Timer timerScript;
    public int grindTime = 2;
    [SerializeField] SpriteRenderer msgSR;
    Sprite msg;
    bool msgGrindedUp = false;
    bool grinding = false;
    [SerializeField] GameObject msgPrefab;
    public GameObject msgObject;

    // msg placement when grinder is done
    public GameObject offsetGameObject;
    Vector3 offset;

    private void Start()
    {
        msg = msgSR.sprite;
        msgSR.enabled = false;
        offset = offsetGameObject.transform.localPosition;
    }

    private void Update()
    {
        if (timerScript.timeLeft <= 0 && grinding)
        {
            FinishGrinding();
        }
    }
    public void StartGrinding(string foodieType)
    {
        grinding = true;
        timerScript.SetMaxTime(grindTime);
        gameObject.GetComponent<Animator>().Play(foodieType);
        // Play grinding sfx
        SoundFX.inst.GrinderSFX(1f, grindTime);
        SoundFX.inst.GrinderScreamSFX(1f, grindTime);
    }

    public void FinishGrinding()
    {
        //msgSR.enabled = true;
        msgObject = SpawnMSG();

        grinding = false;
        msgGrindedUp = true;
        SoundFX.inst.FinishedDishSFX(1f);   
        gameObject.GetComponent<Animator>().Play("Idle");
    }

    private GameObject SpawnMSG()
    {
        //Vector3 offset = new Vector3(0, 0.22f, 0);
        Vector3 spawnPosition = gameObject.transform.position + offset;
        GameObject msg = Instantiate(msgPrefab, spawnPosition, Quaternion.identity, gameObject.transform);

        return msg;
    }

    public void TakeMSG()
    {
        msgGrindedUp = false;
    }

    public bool IsGrindingDone()
    {
        return msgGrindedUp;
    }
}
