using UnityEngine;

/*
* Written By: Jake Nye
* Last Modified: 30/05/2016
*
* This class handles the interaction with the 4th checkpoint
*/ 

public class passedFourth : MonoBehaviour {

    //Canvas
    public Canvas movement;
    public Canvas Shooting;
    public Canvas retrieveFlag;
    public Canvas usingPerks;
    public Canvas incEnemies;
    public Canvas tutFinished;

    public GameObject OoBWall; // out of boundary wall

    public Collider collide; // checkpoint

    /*
    * Ontrigger call to register end of tutorial!
    * @Collider col: Object that collides with the checkpoint
    */
    public void OnTriggerEnter(Collider col) {
        //if object is the player
        if (col.gameObject.tag == "Player" && usingPerks.enabled == true) {
            OoBWall.SetActive(false);
            usingPerks.enabled = false;
            tutFinished.enabled = true;
        }
    }
}
