using UnityEngine;
using System.Collections;

/*
* Written by: Joshua Hurn
* Last Modified: 02/05/2016
*
* This class waits for the player to get ammo found in the level and gives the player ammo for this.
*/

public class AmmoPickup : MonoBehaviour {

    public int ammoGiven = 5; //how much ammo the player will get

    /*
    * When an object hits the ammo
    * @Collider c: Object that hits the ammo
    */
    void OnTriggerEnter(Collider c)
    {
        //if the ammo is hit by the player
        if (c.gameObject.tag == "Player")
        {
            c.SendMessage("ammoPickup", ammoGiven);
            gameObject.SetActive(false); //deactivate the ammo
        }
    }
}
