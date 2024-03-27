using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour
{
    [SerializeField] GameObject selectionSquare; // Must be added in the inspector
    public bool Selected
    {
        get { return selectionSquare.activeSelf; }
        set { selectionSquare.SetActive(value); }
    }

    public void Start()
    {
        Selected = false;
    }
}
