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

}
