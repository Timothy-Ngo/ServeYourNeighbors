using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Takes in a spawnAmount and spawns that many in intervals at the spawnPoint

public class FoodieSpawner : MonoBehaviour
{
    [SerializeField] // makes variable accessible in inspector
    private GameObject foodiePrefab;

    [SerializeField]
    private float foodieInterval = 3.5f; // # of seconds before foodie spawns

    [SerializeField]
    private Vector3 spawnPoint = Vector3.zero;

    public Transform foodieParent;

    public int spawnAmount = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnFoodie(foodieInterval, foodiePrefab, spawnAmount));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator spawnFoodie(float interval, GameObject foodie, int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            yield return new WaitForSeconds(interval);

            GameObject newFoodie = Instantiate(foodie, spawnPoint, Quaternion.identity);
            newFoodie.transform.parent = foodieParent;
            //FoodieSystem.inst.line.Enqueue(newFoodie.GetComponent<FoodieMovement>());
            Debug.Log("Foodie Instantiated");
        }
    }
}
