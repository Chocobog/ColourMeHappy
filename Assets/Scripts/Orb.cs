using UnityEngine;
using System.Collections;

/*
* Written by: Joshua Hurn
* Last Modified: 02/05/2016
*
* This class waits for the player to get the perk and gives the player a random perk
*/

public class Orb : MonoBehaviour {

    string[] perks = new string[] { "Rejuv", "Nimble", "rapid", "radar", "shield" }; //perks available to choose from
    public float orbSpawnTime = 30.0f; //time it takes for the orb to spawn again after being destroyed

    // Use this for initialization
    void Start () {
    }

    /*
    * When something comes into contact with the orb
    * @Collider c: Object that hits the orb
    */
    void OnTriggerEnter(Collider c)
    {
        //if the player hits the orb
        if (c.gameObject.tag == "Player")
        {
            c.SendMessage("getPerk", perks[Random.Range(0, perks.Length)]);
            gameObject.SetActive(false);
        }
    }
}
