using UnityEngine;

/*
* Written By: Jake Nye
* Modified By: Joshua Hurn
* Last Modified: 30/05/2016
*
* This class handles the triggers against the first check point and all canvases in the tutotial
* 
*/

public class Tutorial : MonoBehaviour {

    //Tutorial menu canvas
    public bool tutorialPlayed;
    public Canvas playTutorialPrompt;
    public Canvas movement;
    public Canvas shooting;
    public Canvas retrieveFlag;
    public Canvas incEnemies;

    //Checkpoints
    public GameObject checkPoint1;
    public GameObject enemy1;
    public GameObject enemySpawnPoint;

    /* 
    * OnTriggerEnter is called when a collider enters this trigger
    * @Collider collide: when the player collides with the checkpoints
    */
    void OnTriggerEnter(Collider collide)
    {
        //if the box collider is breached by the player, destroy this object
        if (collide.gameObject.tag == "Player")
        {
            Destroy(checkPoint1);
            movement.enabled = false;
            shooting.enabled = true;
        }

    }
}
