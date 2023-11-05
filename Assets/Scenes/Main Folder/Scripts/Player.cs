using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player inst;

    private void Awake()
    {
        inst = this;
    }

    [Header("Food")] 
    public Food food;
    // Start is called before the first frame update
    void Start()
    {
        food = new Food();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
