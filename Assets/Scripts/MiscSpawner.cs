using UnityEngine;
using System.Collections;

/*
* Written by: Joshua Hurn
* Last Modified: 02/05/2016
*
* This class controls all the spawning for ammo, health and orbs after the player has obtained them
*/

public class MiscSpawner : MonoBehaviour {

    //spawn times
    public float orbSpawnTime = 30.0f;
    public float ammoSpawnTime = 30.0f; 
    public float healthSpawnTime = 30.0f;
    public float resetTime = 30f;


    //spawn points
    public GameObject[] AmmoSpawnPoints;
    public GameObject[] healthSpawnPoints;
    public GameObject[] orbSpawnPoints;

    //Checks to see if ammo/health/perks been picked up
    void Update()
    {

        //if ammo has been set to inactive
        for (int i = 0; i < AmmoSpawnPoints.Length; i++)
        {
            if (!AmmoSpawnPoints[i].activeInHierarchy)
            {
                ammoSpawnTime -= 1 * Time.deltaTime; //start countdown
                if ((int)ammoSpawnTime == 0)
                {
                    AmmoSpawnPoints[i].SetActive(true);
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
                    healthSpawnPoints[i].SetActive(true);
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
                    orbSpawnPoints[i].SetActive(true);
                    orbSpawnTime = resetTime;
                }
                
            }
        }
    }
}
