using UnityEngine;
using System.Collections;

/*
* Written by: Joshua Hurn
* Last Modified: 02/05/2016
*
* This class waits for the player to get a health pack found in the level and gives the player health for this.
*/

public class healthPickup : MonoBehaviour {

    public int healthGiven = 10;

    /*
    * When an object hits the health pack
    * @Collider c: Object that hits the health pack
    */
    void OnTriggerEnter(Collider c)
    {
        //if the object is the player
        if (c.gameObject.tag == "Player")
        {
            c.SendMessage("healthPickup", healthGiven);
            gameObject.SetActive(false); //deactivate the health pack
        }
    }
}
