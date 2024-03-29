// Kirin Hardinger
// November 2023
// Updated to regular MonoBehaviour script in February 2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour, IDataPersistence
{
    // cooking- and serving-related stats
    public int dishesMade = 0;
    public int itemsThrown = 0;
    public int foodiesServed = 0;
    public int msgAdded = 0;
    public int successfulServings = 0;
    public int failedServings = 0;

    // kidnapping- and distraction-related stats
    public int foodiesKidnapped = 0;
    public int foodiesGround = 0;
    public int timesDistracted = 0;
    public int kidnappingsCaught = 0;

    public void LoadData(GameData data)
    {
        dishesMade = data.dishesMade;
        itemsThrown = data.itemsThrown;
        foodiesServed = data.foodiesServed;
        msgAdded = data.msgAdded;
        successfulServings = data.successfulServings;
        failedServings = data.failedServings;

        foodiesKidnapped = data.foodiesKidnapped;
        foodiesGround = data.foodiesGround;
        timesDistracted = data.timesDistracted;
        kidnappingsCaught = data.kidnappingsCaught;
    }

    public void SaveData(GameData data)
    {
        data.dishesMade = dishesMade;
        data.itemsThrown = itemsThrown;
        data.foodiesServed = foodiesServed;
        data.msgAdded = msgAdded;
        data.successfulServings = successfulServings;
        data.failedServings = failedServings;

        data.foodiesKidnapped = foodiesKidnapped;
        data.foodiesGround = foodiesGround;
        data.timesDistracted = timesDistracted;
        data.kidnappingsCaught = kidnappingsCaught;
    }

    public void reset()
    {
        dishesMade = 0;
        itemsThrown = 0;
        foodiesServed = 0;
        msgAdded = 0;
        successfulServings = 0;
        failedServings = 0;
        foodiesKidnapped = 0;
        foodiesGround = 0;
        timesDistracted = 0;
        kidnappingsCaught = 0;
    }

    public int getDishesMade()
    {
        return dishesMade;
    }

    public void setDishesMade(int x)
    {
        dishesMade = x;
    }

    public void incDishesMade()
    {
        dishesMade++;
        //Debug.Log("dishesMade is now " + dishesMade.ToString());
    }

    public int getItemsThrown()
    {
        return itemsThrown;
    }

    public void setItemsThrown(int x)
    {
        itemsThrown = x;
    }

    public void incItemsThrown()
    {
        itemsThrown++;
        //Debug.Log("itemsThrown is now " + itemsThrown.ToString());
    }

    public int getFoodiesServed()
    {
        return foodiesServed;
    }

    public void setFoodiesServed(int x)
    {
        foodiesServed = x;
    }

    public void incFoodiesServed()
    {
        foodiesServed++;
        //Debug.Log("foodiesServed is now " + foodiesServed.ToString());
    }

    public int getMSGAdded()
    {
        return msgAdded;
    }

    public void setMSGAdded(int x)
    {
        msgAdded = x;
    }

    public void incMSGAdded()
    {
        msgAdded++;
        //Debug.Log("msgAdded is now " + msgAdded.ToString());
    }

    public int getSuccessfulServings()
    {
        return successfulServings;
    }

    public void setSuccessfulServings(int x)
    {
        successfulServings = x;
    }

    public void incSuccessfulServings()
    {
        successfulServings++;
        //Debug.Log("msgAdded is now " + msgAdded.ToString());
    }

    public int getFailedServings()
    {
        return failedServings;
    }

    public void setFailedServings(int x)
    {
        failedServings = x;
    }

    public void incFailedServings()
    {
        failedServings++;
        //Debug.Log("msgAdded is now " + msgAdded.ToString());
    }

    public int getFoodiesKidnapped()
    {
        return foodiesKidnapped;
    }

    public void setFoodiesKidnapped(int x)
    {
        foodiesKidnapped = x;
    }

    public void incFoodiesKidnapped()
    {
        foodiesKidnapped++;
        //Debug.Log("foodiesKidnapped is now " + foodiesKidnapped.ToString());
    }

    public int getFoodiesGround()
    {
        return successfulServings;
    }

    public void setFoodiesGround(int x)
    {
        successfulServings = x;
    }

    public void incFoodiesGround()
    {
        successfulServings++;
        //Debug.Log("msgAdded is now " + msgAdded.ToString());
    }

    public int getTimesDistracted()
    {
        return timesDistracted;
    }

    public void setTimesDistracted(int x)
    {
        timesDistracted = x;
    }

    public void incTimesDistracted()
    {
        timesDistracted++;
        //Debug.Log("timesDistracted is now " + timesDistracted.ToString());
    }

    public int getKidnappingsCaught()
    {
        return kidnappingsCaught;
    }

    public void setKidnappingsCaught(int x)
    {
        kidnappingsCaught = x;
    }

    public void incKidnappingsCaught()
    {
        kidnappingsCaught++;
        //Debug.Log("msgAdded is now " + msgAdded.ToString());
    }
}
