// Author: Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBox : MonoBehaviour
{
    [SerializeField] GameObject ingredientPrefab;
    [SerializeField] bool showDebug = false;

    private void Update()
    {
        if (showDebug)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                SpawnIngredient();
            }
        }
    }

    public GameObject SpawnIngredient()
    {
        Vector3 offset = new Vector3(0, 1, 0);
        Vector3 spawnPosition = Player.inst.transform.position + offset;
        GameObject ingredient = Instantiate(ingredientPrefab, spawnPosition, Quaternion.identity, Player.inst.transform);

        return ingredient;
    }

    public void Animate(string state)
    {
        if(!(gameObject.GetComponent<Animator>() is null))
        {
            gameObject.GetComponent<Animator>().Play(state);
            if (state.ToLower() == "open")
            {
                SoundFX.inst.OpenIngredientBoxSFX(1f);
            }
            else if (state.ToLower() == "close")
            {
                SoundFX.inst.CloseIngredientBoxSFX(1f);
            }
        }
    }
}
