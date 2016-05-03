using UnityEngine;
using System.Collections;

/*
* Written by: Joshua Hurn
* Last Modified: 02/05/2016
*
* This class waits for the player to get the perk and gives the player a random perk
*/

public class Orb : MonoBehaviour {

    string[] perks = new string[] { "Rejuv", "Nimble", "rapid", "radar", "shield" };
    public float orbSpawnTime = 30.0f;
    public bool objectHit; 

    // Use this for initialization
    void Start () {
    }
	
    //When the orb is hit
    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            c.SendMessage("getPerk", perks[Random.Range(0, perks.Length)]);
            gameObject.SetActive(false);
        }
    }
}
