using UnityEngine;
using System.Collections;

/*
* Written by: Joshua Hurn
* Last Modified: 02/05/2016
*
* This class controls all the spawning for ammo, health, flags and orbs after the player has obtained them or destroyed them
*/

public class MiscSpawner : MonoBehaviour {

    //spawn times
    public float orbSpawnTime = 30.0f;
    public float ammoSpawnTime = 30.0f; 
    public float healthSpawnTime = 30.0f;
    public float resetTime = 30f;

    //Red flag instantiation
    private GameObject[] enemyFlag;
    public GameObject redFlag;
    private GameObject[] allyFlag;
    public GameObject blueFlag;
    

    //spawn points
    public GameObject[] AmmoSpawnPoints;
    public GameObject[] healthSpawnPoints;
    public GameObject[] orbSpawnPoints;
    public Transform redFlagSpawn;
    public Transform blueFlagSpawn;

    //updates every frame
    void Update()
    {
        //search for red and blue flag
        enemyFlag = GameObject.FindGameObjectsWithTag("RedFlag");
        allyFlag = GameObject.FindGameObjectsWithTag("BlueFlag");

        //if no red flag in the game, create one
        if (enemyFlag.Length == 0)
        {
            Instantiate(redFlag, redFlagSpawn.position, redFlag.transform.rotation);
            Debug.Log("Red flag has been created!");
        }


        //If no flag then create one
        if (allyFlag.Length == 0)
        {
            Instantiate(blueFlag, blueFlagSpawn.position, blueFlag.transform.rotation);
        }

        //if ammo has been set to inactive
        for (int i = 0; i < AmmoSpawnPoints.Length; i++)
        {
            if (!AmmoSpawnPoints[i].activeInHierarchy)
            {
                ammoSpawnTime -= 1 * Time.deltaTime; //start countdown
                if ((int)ammoSpawnTime == 0)
                {
                    AmmoSpawnPoints[i].SetActive(true); //set active if countdown reaches 0
                    ammoSpawnTime = resetTime;
                }
            }
        }

        //if healthpack has been set to inactive
        for (int i = 0; i < healthSpawnPoints.Length; i++)
        {
            if (!healthSpawnPoints[i].activeInHierarchy)
            {
                healthSpawnTime -= 1 * Time.deltaTime; //start countdown
                if ((int)healthSpawnTime == 0)
                {
                    healthSpawnPoints[i].SetActive(true); //set active if countdown reaches 0
                    healthSpawnTime = resetTime;
                }
            }
        }

        //If orb has been set to inactive
        for (int i = 0; i < orbSpawnPoints.Length; i++)
        {
            if (!orbSpawnPoints[i].activeInHierarchy)
            {
                orbSpawnTime -= 1 * Time.deltaTime; //start countdown
                if ((int)orbSpawnTime == 0)
                {
                    orbSpawnPoints[i].SetActive(true); //set active if countdown reaches 0
                    orbSpawnTime = resetTime;
                }
                
            }
        }
    }
}
