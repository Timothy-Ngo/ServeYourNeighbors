// Author: Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PickupSystem : MonoBehaviour
{
    public static PickupSystem inst;
    private void Awake()
    {
        inst = this;
    }

    [Header("-----PLAYER INFO-----")]
    [SerializeField] GameObject itemInHands;
    private bool holdingItem = false;

    [Header("-----FOODIE-----")]
    [SerializeField] Transform foodieParent;

    [Header("-----FLAGS-----")] // flags to ensure only ingredients get cooked and only foodies get grinded
    private bool holdingIngredient = false;
    private bool holdingTopping = false;
    private bool holdingDish = false;
    private bool holdingFoodie = false;

    [Header("-----DEBUGGING-----")]
    [SerializeField] bool showDebug = false;
    public Sprite spriteTest;


    public void PickUpIngredient(IngredientBox ingredientBoxScript)
    {
        itemInHands = ingredientBoxScript.SpawnIngredient();
        holdingItem = true;
        holdingIngredient = true;
    }

    // picks up already existing objects
    public void PickUpItem(GameObject item)
    {
        // puts item in player's hands
        itemInHands = item;
        // item is positioned above player's head
        Vector3 offset = new Vector3(0, 1, 0); 
        item.transform.parent = Player.inst.gameObject.transform;
        item.transform.localPosition = offset;

        holdingItem = true;

        // sets flags
        if (item.CompareTag("Ingredient"))
        {
            holdingIngredient = true;
        }
        else if (item.CompareTag("Topping"))
        {
            holdingTopping = true;
        }
        else if (item.CompareTag("Dish"))
        {
            holdingDish = true;
        }
        else if (item.CompareTag("Foodie"))
        {
            holdingFoodie = true;
        }
    }

    public void PlaceItem(Transform parent, Vector3 offset)
    {
        itemInHands.transform.parent = parent;
        itemInHands.transform.position = parent.position + offset;
        itemInHands = null;

        ResetFlags();
    }

    public void ReleaseFoodie()
    {

        Foodie foodieScript = itemInHands.GetComponent<Foodie>();
        foodieScript.gameObject.transform.parent = foodieParent;
        foodieScript.stateMachine.ChangeState(foodieScript.leaveState);
        itemInHands = null;
        
        ResetFlags();
    }

    // destroys item
    public void DestroyItem()
    {
        if (holdingFoodie)
        {
            // calling DestroyFoodie() will make sure the number of foodies in the UI is decremented
            itemInHands.GetComponent<Foodie>().DestroyFoodie();
        }
        else
        {
            Destroy(itemInHands);
        }

        itemInHands = null;
        ResetFlags();
    }

    private void ResetFlags()
    {
        holdingItem = false;
        holdingIngredient = false;
        holdingTopping = false;
        holdingDish = false;
        holdingFoodie = false;
    }

    public GameObject GetItemInHands()
    {
        return itemInHands;
    }

    public bool isHoldingItem()
    {
        return holdingItem;
    }

    public bool isHoldingIngredient()
    {
        return holdingIngredient;
    }

    public bool isHoldingTopping()
    {
        return holdingTopping;
    }
    public bool isHoldingDish()
    {
        return holdingDish;
    }

    public bool isHoldingFoodie()
    {
        return holdingFoodie;
    }
}
