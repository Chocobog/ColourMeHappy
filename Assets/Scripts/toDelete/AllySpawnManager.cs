using UnityEngine;
using System.Collections;

/*
* @Written By: Joshua Hurn
* @Last Modified: 23/04/2016
*
* This controls the Allies being spawned at the ally base
*/

public class AllySpawnManager : MonoBehaviour {
    
    public GameObject ally; //Ally to be spawned
    public float spawnTime = 10.0f; //Time it takes for ally to be spawned after health = 0
    public Transform[] spawnPoints; //Spawn points for allies
    public int allyLimit = 5;

    private GameObject[] allyCount;
    private int spawnIndex = 0;

    //flag spawn
    private GameObject[] allyFlag;
    public GameObject blueFlag;
    public Transform flagSpawn;



    // Use this for initialization
    void Start () {
        //spawn allies 
       // InvokeRepeating("spawn", 0, 0.1f);
	}
	
    //Spawns alies and controls when, where and how many are spawned in the game
    void spawn()
    {
        //count allies in game
        allyCount = GameObject.FindGameObjectsWithTag("Ally");

        // If ally limit has reached, don't spawn anymore allies
        if (allyCount.Length == allyLimit)
        {
            return;
        }

        //Spawn ally
        Instantiate(ally, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
        spawnIndex++;
        
    }

    //Check to see if flag has been destroyed by other team
    void Update()
    {
        
    }
}
