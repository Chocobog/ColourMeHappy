using UnityEngine;
using System.Collections;

/*
* Written By: Joshua Hurn
* Last Modified: 10/05/2016
*
* This class controls the sound made when something enters the water
*/
public class WaterAudio : MonoBehaviour {

    public AudioClip enterWater; //sound when something enters the water

    /*
    * When an object enters the trigger area    
    * @Collider c: Object that collides with trigger
    */
    void OnTriggerEnter(Collider c)
    {
        if (c.transform.tag == "Player")
        {
            c.SendMessage("playWalkingAudio", false);
            GetComponent<AudioSource>().clip = enterWater;
            GetComponent<AudioSource>().Play();
        }
            
    }

    /*
    * When an object exits the trigger area    
    * @Collider c: Object that collides with trigger
    */
    void OnTriggerExit (Collider c)
    {
        if (c.transform.tag == "Player")
        {
            c.SendMessage("playWalkingAudio", true);
            GetComponent<AudioSource>().Stop();
        }

    }
}
