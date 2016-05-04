using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/*
* Written by: Joshua Hurn
* Last Modified: 02/05/2016
*
* This class controls the enemy NPC and their behaviour in the environment
*
*/
public class EnemyNPC : MonoBehaviour
{
    //Enemy/Ally Flag
    public GameObject[] waypointList;

    protected GameObject playerTransform;// Player Transform
    protected GameObject[] PlayerAllies; //Player Allies Transform

    // Bullet
    public GameObject bullet;
    public GameObject bulletSpawnPoint;

    // Bullet shooting rate
    public float shootRate = 3.0f;
    protected float elapsedTime;

    // Whether the NPC is destroyed or not
    public int health = 100;

    // Ranges for chase and attack
    public float chaseRange;
    public float attackRange;
    public float attackRangeStop = 10.0f;

    //flag capture
    public Transform flag;
    public Transform flagMist;
    public Transform flagHolder;
    public string opposingFlag;
    public string allyFlag;

    //animation
    Animator animator;

    //Player HUD update
    public string allyFlagLocation;
    public Text allyFlagLocationTxt;
    public int scoreEnemy; //score of enemy team
    public Text scoreEnemyTxt;

    //who defeated this enemy
    public string defeater;
    public bool invulnerable;

    //respawning
    public float respawnCountdown;
    public float respawnReset = 10f;
    public GameObject respawnEffect;
    public Transform[] enemySpawnPositions = new Transform[6];

    //navmesh agent (enemy)
    private NavMeshAgent nav;

    //player score update
    private int scoreUpdate;

    //For radar effect - FPS script
    public GameObject marker;
    public bool showMarker;
    private float radarCountdown;

    //Initialisation
    void Start()
    {
        elapsedTime = 0.0f;

        chaseRange = 100f;
        attackRange = 65f;

        allyFlagLocation = "At Base - ";
        opposingFlag = "BlueFlag";
        allyFlag = "RedFlag";

        // Target enemies
        playerTransform = GameObject.FindGameObjectWithTag("Player");
        PlayerAllies = GameObject.FindGameObjectsWithTag("Ally");

        showMarker = false;
        scoreUpdate = 10;

        //Get the tanks nav mesh
        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(waypointList[0].transform.position);

        animator = GetComponent<Animator>();

        invulnerable = false;
        respawnCountdown = respawnReset; // set respawn timer
    }

    // Update each frame
    void Update()
    {
        //update location of flag
        allyFlagLocationTxt.text = allyFlagLocation;
        scoreEnemyTxt.text = scoreEnemy.ToString();

        // Update the time
        elapsedTime += Time.deltaTime;
        
        // Go to dead state if no health left
        if (health <= 0)
        {
            respawnCountdown -= 1 * Time.deltaTime; //start counter
            //Only plays this animation once
            animator.Play("dead");
            respawnEffect.SetActive(true); //respawn effect
            nav.Stop();
            invulnerable = true;
            //if player shot last bullet to kill enemy update score
            if (defeater.Equals(playerTransform.tag) && !invulnerable)
            {
                FirstPersonController player = playerTransform.GetComponent<FirstPersonController>();
                player.playerScore += scoreUpdate;
            }
            //take back to enemy base spawn position, chosen time of 3 second offset to show player visual effect
            if (transform.position != enemySpawnPositions[0].position && (int)respawnCountdown == (int)respawnReset - 3)
                transform.position = enemySpawnPositions[0].position;
            //respawn over - set back to default
            if ((int)respawnCountdown == 0)
            {
                nav.Resume();
                health = 100;
                invulnerable = false;
                respawnCountdown = 10f;
                respawnEffect.SetActive(false);
            }
            

            //teleporter effect
            //transform position back to base
            //timer for 10 seconds then nav.Resume
        }
        
        else
        {
            //If the player comes into the chase distance but not in attack range
            if (Vector3.Distance(transform.position, playerTransform.transform.position) <= chaseRange && Vector3.Distance(transform.position, playerTransform.transform.position) > attackRange)
            {
                animator.Play("idle pose with a gun");
                nav.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
            }
            else if (Vector3.Distance(transform.position, playerTransform.transform.position) > chaseRange)
            {
                //animator for walking on the ground
                animator.Play("idle");
            }

            //if the player comes into the attack distance
            if (Vector3.Distance(transform.position, playerTransform.transform.position) <= attackRange)
            {
                ShootBullet();
                
            }
        }
    }

    //Collisions with the enemy
    public void OnTriggerEnter(Collider c)
    {
        //if this is not the enemies team flag then take the flag
        if (c.gameObject.tag == "BlueFlag")
        {
            Debug.Log("Found opposing flag");
            //Add flag to player 
            flag = c.transform;
            flag.transform.parent = transform;
            flag.transform.position = flagHolder.transform.position;
            //update location of flag
            allyFlagLocation = "Taken - ";

            //Add mist to player
            flagMist = c.transform;
            flagMist.transform.parent = transform;
            flagMist.transform.position = flagHolder.transform.position;

            nav.SetDestination(waypointList[1].transform.position);
        }

        //if enemy returns to flag with opposing team flag
        if (c.gameObject.tag == allyFlag && flag != null)
        {
            //Destroy the flag gameObject
            Destroy(flag.gameObject);
            scoreEnemy++;
            //update location of flag
            allyFlagLocation = "At Base -";
        }
    }


    /*
     * Shoot Bullet
     */
    private void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            if ((bulletSpawnPoint) & (bullet))
            {
                animator.Play("shot"); //animation for shooting
                Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation); // Shoot the bullet
            }
            elapsedTime = 0.0f;
        }
    }

    // Apply Damage if hit by bullet
    public void takeDamage(int damage)
    {
        health -= damage;
    }

    /*
    * Used to find out who shot the final bullet to kill the enemy
    * @string s: Origin of the bullet that was shot
    */
    public void defeatedBy(string s)
    {
        defeater = s;
    }

    /*
    * Activates the enemy marker for the player to see
    * @bool b: Passed from the FPS script when radar effect triggered
    */
    public void markerActivate(bool b)
    {
        showMarker = b;
        marker.SetActive(showMarker);
    }

    /*
    * Deactivates the enemy marker for the player
    * @bool b: Passed from the FPS script when radar effect ends
    */
    public void markerDeactivate(bool b)
    {
        showMarker = b;
        marker.SetActive(showMarker);
    }

    //Drawn elements on the screen
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}
