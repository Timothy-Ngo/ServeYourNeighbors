// Author: Helen Truong
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Takes in a spawnAmount and spawns that many in intervals at the spawnPoint
public class FoodieSpawner : MonoBehaviour
{
    public static FoodieSpawner inst;
    private void Awake()
    {
        inst = this;
    }

    [Tooltip("When ever you want to make changes to this prefab, make sure you do it in the projects window and not the foodie object in the hierarchy")]
    [SerializeField] private GameObject foodiePrefab; // makes variable accessible in inspector

    [SerializeField] private float foodieInterval = 3.5f; // # of seconds before foodie spawns

    [SerializeField] private Vector3 spawnPoint = Vector3.zero;

    [SerializeField] private Transform foodieParent;

    [SerializeField] private int spawnAmount = 1;

    [SerializeField] private GameObject spawnObject;

    [SerializeField] GameObject player;

    [Header("-----FOODIE PREFABS-----")]
    public GameObject[] foodiePrefabs; // drag and drop prefabs in the inspector

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = spawnObject.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // spawn multiple(s) of one type of foodie
    private IEnumerator spawnFoodie(float interval, GameObject foodie, int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            yield return new WaitForSeconds(interval);

            GameObject newFoodie = Instantiate(foodie, spawnPoint, Quaternion.identity);
            newFoodie.transform.parent = foodieParent;
            //Debug.Log("Foodie Instantiated");
        }
    }

    // spawn multiple(s) of random types of foodies
    private IEnumerator SpawnRandomFoodie(float interval, int spawnAmount)
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            yield return new WaitForSeconds(interval);

            int index = Random.Range(0, foodiePrefabs.Length); // select a random index
            GameObject randomFoodiePrefab = foodiePrefabs[index];

            GameObject newFoodie = Instantiate(randomFoodiePrefab, spawnPoint, Quaternion.identity);
            newFoodie.transform.parent = foodieParent;
            //Debug.Log("Foodie Instantiated");
        }
    }

    // spawns wave of the tomato foodies
    public void SpawnWaveOfFoodies()
    {
        Debug.Log("Spawn wave of foodies");
        StartCoroutine(spawnFoodie(foodieInterval, foodiePrefab, spawnAmount));
    }

    // spawns wave of randomly selected foodies
    public void SpawnWaveOf(int amountOfFoodies)
    {
        StartCoroutine(SpawnRandomFoodie(foodieInterval, amountOfFoodies));
    }

    public void SpawnA(GameObject foodiePrefab)
    {
        StartCoroutine(spawnFoodie(0, foodiePrefab, 1));
    }

    private IEnumerator SpawnKidnappedFoodie()
    {
        Vector3 spawnPosition = player.transform.position;
        GameObject newFoodie = Instantiate(foodiePrefab, spawnPosition, Quaternion.identity);
        newFoodie.transform.parent = foodieParent;
        Foodie foodieScript = newFoodie.GetComponent<Foodie>();
        yield return new WaitForSeconds(0.1f);
        if (FoodieSystem.inst.line.Contains(foodieScript.foodieMovement))
            FoodieSystem.inst.line.Remove(foodieScript.foodieMovement);
        foodieScript.stateMachine.ChangeState(foodieScript.leaveState);

        
        
    }

    public void ReleaseFoodie()
    {
        StartCoroutine(SpawnKidnappedFoodie());
    }
}
