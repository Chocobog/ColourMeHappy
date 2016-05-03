using UnityEngine;
using System.Collections;

/*
* @Written By: Joshua Hurn
* @Last Modified: 23/04/2016
*
* This controls the enemies being spawned at the enemy base
*/

public class EnemySpawnManager : MonoBehaviour {
    
    public GameObject enemy; //Enemy to be spawned
    public float spawnTime = 10.0f; //Time it takes for enemy to be spawned after health = 0
    public Transform[] spawnPoints; //Spawn points for enemies
    public int enemyLimit = 6;

    private GameObject[] enemyCount;
    private int spawnIndex = 0;

    //flag spawn
    private GameObject[] enemyFlag;
    public GameObject redFlag;
    public GameObject redFlagMist;
    public Transform flagSpawn;


	// Use this for initialization
	void Start () {
        //spawn enemies 
        InvokeRepeating("spawn", 0, 0.1f);
	}
	
    //Spawns enemies and controls when, where and how many are spawned in the game
    void spawn()
    {
        //count enemies in game
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy");

        // If enemy limit has reached, don't spawn anymore enemies
        if (enemyCount.Length == enemyLimit)
        {
            return;
        }

        //Spawn enemy
        Instantiate(enemy, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
        spawnIndex++;
        
    }

    //Check to see if flag has been destroyed by other team
    void Update()
    {
        enemyFlag = GameObject.FindGameObjectsWithTag("RedFlag");
        //flagMist = GameObject.FindGameObjectsWithTag("redFlagMist");

        //If no flag then create one
        if (enemyFlag.Length == 0)
        {
            Instantiate(redFlag, flagSpawn.position, redFlag.transform.rotation);
            Debug.Log("Flag has been created!");
            Instantiate(redFlagMist, flagSpawn.position, Quaternion.identity);
            //redFlagMist.SetActive(true);
            //Debug.Log("Mist has been created!");
            //Instantiate(redFlagMist, flagSpawn.position, redFlagMist.transform.rotation);
            Debug.Log("Flag mist has been created!");
        }


    }
}
