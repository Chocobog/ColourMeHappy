using UnityEngine;
using System.Collections;

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

    public GameObject respawnNodeEnemy;

    public void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") {
            //movement.enabled = false;
            //shooting.enabled = false;
            retrieveFlag.enabled = false;
            usingPerks.enabled = true;
        }
    }

    public void OnTriggerExit(Collider col) {
        if (col.gameObject.tag == "Player") {
            //Instantiate(Enemy, EnemySpawn1.transform.position, EnemySpawn1.transform.rotation);
            //Instantiate(Enemy, EnemySpawn2.transform.position, EnemySpawn2.transform.rotation);
            Instantiate(Enemy, EnemySpawn3.transform.position, EnemySpawn3.transform.rotation);
            Instantiate(Enemy, EnemySpawn4.transform.position, EnemySpawn4.transform.rotation);

            // assigns the node to the waypoint component
            Enemy.GetComponent<tutFSM>().waypoint = GameObject.FindGameObjectWithTag("spawnNodeEnemy");
            //Enemy.GetComponent<tutFSM>().guardSpawnPosition = GameObject.FindGameObjectWithTag("respawnNodeEnemy");

        }
    }
	

}
