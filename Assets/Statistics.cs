using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{

    public int ordersCompleted = 0;
    public int customersKidnapped = 0;
    public int distractionsUsed = 0;
    public int totalGoldGained = 0;



    public static Statistics inst;
    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




}
