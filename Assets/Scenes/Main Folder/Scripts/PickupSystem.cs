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
    [SerializeField] private GameObject playerHolding;
    private SpriteRenderer holding;
    private bool holdingSomething = false;

    [Header("-----FLAGS-----")] // flags to ensure nothing but ingredients get cooked
    private bool holdingDish = false;
    private bool holdingIngredient = false;
    private bool holdingTopping = false;
    private bool holdingFoodie = false;

    [Header("-----LISTS-----")] // manually add items in the inspector
    public List<Sprite> dishes;
    public List<Sprite> ingredients;
    public List<Sprite> toppings;
    public List<Sprite> foodies;

    [Header("-----DEBUGGING-----")]
    [SerializeField] bool showDebug = false;
    public Sprite spriteTest;


    private void Start()
    {
        holding = playerHolding.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (showDebug)
        {
            // debugging
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!holdingSomething)
                    PickUpItem(spriteTest);
                else
                    DropItem();
            }
        }
    }

    // change sprite of what player is holding to show pickup
    public void PickUpItem(Sprite item)
    {
        if (!holdingSomething)
        {
            // changes flags
            if (dishes.Contains(item)) // if item is a dish
            {
                Debug.Log("holding dish");
                holdingDish = true;
            }
            else if (ingredients.Contains(item)) // if item is an ingredient
            {
                Debug.Log("holding ingredient");
                holdingIngredient = true;
            }
            else if (toppings.Contains(item)) 
            {
                Debug.Log("holding topping");
                holdingTopping = true;
            }
            else if (foodies.Contains(item))
            {
                Debug.Log("holding foodie");
                holdingFoodie = true;
            }

            // changes sprite to item being picked up
            holding.sprite = item;

            holdingSomething = true;
        }
    }

    public void DropItem()
    {
        if (holdingSomething)
        {
            // resets flags
            holdingDish = false;
            holdingIngredient = false;
            holdingTopping = false;
            holdingFoodie = false;

            // player isn't holding anything
            holding.sprite = null;
        
            holdingSomething = false;
        }
    }

    public Sprite GetItem()
    {
        return holding.sprite;
    }

    public bool isHoldingSomething()
    {
        return holdingSomething;
    }
    public bool isHoldingDish() {
        return holdingDish;
    }

    public bool isHoldingIngredient()
    {
        return holdingIngredient;
    }

    public bool isHoldingTopping() {
        return holdingTopping;
    }

    public bool isHoldingFoodie()
    {
        return holdingFoodie;
    }
}
