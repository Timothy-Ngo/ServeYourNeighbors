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

    private void Start()
    {
        msg = msgSR.sprite;
        msgSR.enabled = false;
    }

    private void Update()
    {
        if (timerScript.timeLeft <= 0 && grinding)
        {
            FinishGrinding();
        }
    }
    public void StartGrinding()
    {
        grinding = true;
        timerScript.SetMaxTime(grindTime);
    }

    public void FinishGrinding()
    {
        //msgSR.enabled = true;
        msgObject = SpawnMSG();

        grinding = false;
        msgGrindedUp = true;
    }

    private GameObject SpawnMSG()
    {
        Vector3 offset = new Vector3(0, 0.22f, 0);
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
