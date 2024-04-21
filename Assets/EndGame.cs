using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGame : MonoBehaviour
{
    [Header("-----FLOORING-----")]
    public GameObject shackDesignObj;
    public GameObject tavernDesignObj;
    public GameObject restaurantDesignObj;
    public Sprite shackImage;
    public Sprite tavernImage;
    public Sprite restaurantImage;
    public GameObject flooring;

    [Header("-----SCORE-----")]
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI scoreText;

    private void Update()
    {
        if (shackDesignObj.activeInHierarchy)
        {
            flooring.GetComponent<Image>().sprite = shackImage;
        }
        else if (tavernDesignObj.activeInHierarchy)
        {
            flooring.GetComponent<Image>().sprite = tavernImage;
        }
        else if (restaurantDesignObj.activeInHierarchy)
        {
            flooring.GetComponent<Image>().sprite = restaurantImage;
        }

        if (dayText.text == "1")
        {
            scoreText.text = "Score: " + dayText.text + " day";
        }
        else
        {
            scoreText.text = "Score: " + dayText.text + " days";
        }
    }
}
