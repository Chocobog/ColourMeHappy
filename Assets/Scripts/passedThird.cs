using UnityEngine;

/*
* Written By: Jake Nye
* Last Modified: 30/05/2016
*
* This class handles the interaction with the 3rd checkpoint
*/

public class passedThird : MonoBehaviour {

    // Canvas elements
    public Canvas movement; // disable if necessary
    public Canvas shooting; // disable if necesssary
    public Canvas retrieveFlag; // disable this canvas
    public Canvas usingPerks; // show this canvas

    // spawning enemies
    public GameObject Enemy;
    public GameObject EnemySpawn1;
    public GameObject EnemySpawn2;
    public GameObject EnemySpawn3;
    public GameObject EnemySpawn4;

    /*
    * Ontrigger call to register end of tutorial!
    * @Collider col: Object that collides with the checkpoint
    */
    public void OnTriggerEnter(Collider col) {
        //if object is the player
        if (col.gameObject.tag == "Player") {
            retrieveFlag.enabled = false;
            usingPerks.enabled = true;

            //instantiate enemies
            Instantiate(Enemy, EnemySpawn3.transform.position, EnemySpawn3.transform.rotation);
            Enemy.GetComponent<tutFSM>().waypoint = GameObject.FindGameObjectWithTag("spawnNodeEnemy");
        }
    }
}
