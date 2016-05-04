using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/*
* Written by: Joshua Hurn
* Last Modified: 02/05/2016
*
* This class controls the enemy NPC and their behaviour in the environment
* Enemy NPC logic is a rule based system
*/
public class EnemyNPC : MonoBehaviour
{
    //Enemy/Ally Flag
    public GameObject[] waypointList;

    protected GameObject playerTransform;// Player Transform
    protected GameObject[] PlayerAllies; //Player Allies Transform
    protected GameObject[] enemyFriends; //friends of the enemy

    // Bullet
    public GameObject bullet;
    public GameObject bulletSpawnPoint;

    // Bullet shooting rate
    public float shootRate = 3.0f;
    protected float elapsedTime;
    public int rotSpeed = 11;

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
    public int scoreEnemy; //score of enemy team

    //who defeated this enemy
    public string defeater;
    public bool invulnerable;

    //respawning
    public float respawnCountdown;
    public float respawnReset;
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
    
    //Flag carrier
    public bool FC;
    public bool isEnemyFlagCarrier; //changes if any other enemy picks up the flag
    FirstPersonController fp;

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
        enemyFriends = GameObject.FindGameObjectsWithTag("Enemy");


        showMarker = false;
        scoreUpdate = 10;
        isEnemyFlagCarrier = false;
        FC = false;

        //Get the enemy nav mesh
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        invulnerable = false;
        respawnReset = 10f;
        respawnCountdown = respawnReset; // set respawn timer
    }

    // Update each frame
    void Update()
    {
        // Update the time
        elapsedTime += Time.deltaTime;
        
        // enemy has no health left
        if (health <= 0)
        {
            respawnCountdown -= 1 * Time.deltaTime; //start counter
            //Only plays this animation once
            animator.Play("dead");
            respawnEffect.SetActive(true); //respawn effect
            nav.Stop();
            invulnerable = true;
            if(flag!=null) 
                Destroy(flag.gameObject); //if enemy has the flag, destroy when the are defeated
            //if player shot last bullet to kill enemy update score
            if (defeater.Equals(playerTransform.tag) && !invulnerable)
            {
                fp = playerTransform.GetComponent<FirstPersonController>();
                fp.playerScore += scoreUpdate;
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
        }
        else
        {
            //Enemy is alive
            foreach (GameObject ally in PlayerAllies)
            {
                //if the players ally comes into the chase distance
                if (Vector3.Distance(transform.position, ally.transform.position) <= chaseRange && Vector3.Distance(transform.position, ally.transform.position) > attackRange)
                {
                    animator.Play("idle pose with a gun");
                    nav.SetDestination(GameObject.FindGameObjectWithTag("Ally").transform.position);
                }
                //If the player comes into the chase distance but not in attack range
                else if (Vector3.Distance(transform.position, playerTransform.transform.position) <= chaseRange && Vector3.Distance(transform.position, playerTransform.transform.position) > attackRange)
                {
                    animator.Play("idle pose with a gun");
                    nav.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
                }
                //if no allies or player is within distance 
                else if (Vector3.Distance(transform.position, playerTransform.transform.position) > chaseRange && Vector3.Distance(transform.position, ally.transform.position) > chaseRange)
                {
                    animator.Play("idle");
                    fp = playerTransform.GetComponent<FirstPersonController>(); //used to get location of enemy flag
                    //if enemy flag has been taken and this instance is the flag carrier
                    if (FC && fp.enemyFlagLocation.Equals("Taken -"))
                    {
                        nav.SetDestination(GameObject.FindGameObjectWithTag("enemySafeSpot").transform.position);
                    }
                    //if your the flag carrier and enemy flag is at the base
                    else if(FC && fp.enemyFlagLocation.Equals("At Base - "))
                    {
                        nav.SetDestination(GameObject.FindGameObjectWithTag("RedFlag").transform.position); //go back to flag for capture
                    }
                    //if player has taken enemy flag and this enemy instance is not a FC
                    else if (!FC && isEnemyFlagCarrier) //|| allyFlagLocation.Equals("Taken - ") && fp.enemyFlagLocation.Equals("Taken - ")
                    {
                        float randomDecision = 0;//Random.Range(0, 2);
                        //Randomly choose to find allies/player and destroy or follow to protect the FC
                        if (randomDecision == 0)
                        {
                            nav.SetDestination(closestEnemy(PlayerAllies).position); //Go to closest allie or player
                        }
                        else { }
                        // nav.SetDestination(GameObject.FindGameObjectWithTag("BlueFlag").transform.position); //Go to FC
                    }
                    //if blue flag is at the base, attempt to capture flag
                    else if (allyFlagLocation.Equals("At Base - ") && GameObject.FindWithTag("BlueFlag"))
                    {
                        nav.SetDestination(GameObject.FindGameObjectWithTag("BlueFlag").transform.position);
                    }
                    //if player has taken enemy flag
                    else if (fp.enemyFlagLocation.Equals("Taken - "))
                    {
                        //if distance to player flag is < then distance to enemy flag
                        if(Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("BlueFlag").transform.position) < Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("RedFlag").transform.position))
                        {
                            nav.SetDestination(GameObject.FindGameObjectWithTag("BlueFlag").transform.position); //get enemy flag
                        } else
                            nav.SetDestination(GameObject.FindGameObjectWithTag("RedFlag").transform.position); //get enemy flag
                    }
                }
                //if the player or players ally comes into the attack distance
                if (Vector3.Distance(transform.position, ally.transform.position) <= attackRange || Vector3.Distance(transform.position, playerTransform.transform.position) <= attackRange)
                {
                    //rotate enemy towards player
                    nav.SetDestination(transform.position + new Vector3(5.0f, 0.0f, 0.0f));
                    Quaternion enemyRotate = Quaternion.LookRotation(playerTransform.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, enemyRotate, Time.deltaTime * rotSpeed);
                    ShootBullet();
                }
            }  
        }
    }

    //Collisions with the enemy
    public void OnTriggerEnter(Collider c)
    {
        //if this is not the enemies team flag then take the flag
        if (c.gameObject.tag == "BlueFlag" && c.transform.parent.tag != "Enemy")
        {
            allyFlagLocation = "Taken - ";
            fp.allyFlagLocation = "Taken - ";
            //Add flag to player 
            flag = c.transform;
            flag.transform.parent = transform;
            flag.transform.position = flagHolder.transform.position;
            //update location of flag

            //Add mist to player
            flagMist = c.transform;
            flagMist.transform.parent = transform;
            flagMist.transform.position = flagHolder.transform.position;
            FC = true;
            isEnemyFlagCarrier = true;
           // nav.SetDestination(GameObject.FindGameObjectWithTag("RedFlag").transform.position);
            foreach (GameObject friends in enemyFriends)
            {
                friends.SendMessage("flagTaken", isEnemyFlagCarrier);
            }
        }

        //if enemy returns to flag with opposing team flag
        if (c.gameObject.tag == allyFlag && flag != null)
        {
            FC = false;
            isEnemyFlagCarrier = false;
            fp.scoreEnemy++;
            //Destroy the flag gameObject
            Destroy(flag.gameObject);
            
            //update location of flag
            allyFlagLocation = "At Base - ";
            fp.allyFlagLocation = "At Base - ";
            foreach (GameObject friends in enemyFriends)
            {
                friends.SendMessage("flagTaken", isEnemyFlagCarrier);
            }
        }
        else { }
    }

    // lets the other enemies know if the flag has been taken or not
    public void flagTaken (bool b)
    {
        isEnemyFlagCarrier = b;
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
                
                GameObject spawnOrigin = (GameObject)Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
                spawnOrigin.SendMessage("spawnOrigin", gameObject); //send message to the bullet to say who shot this bullet
                animator.Play("shot"); //animation for shooting
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

    /*
    * Returns the closest player or allies position
    * @Gameobject[] g: Array of player allies 
    */
    public Transform closestEnemy(GameObject[] g)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        float playerDist = Vector3.Distance(playerTransform.transform.position, currentPos); //player distance
        foreach (GameObject closest in g)
        {
            float dist = Vector3.Distance(closest.transform.position, currentPos); // ally distance
            //if player distance or ally of player distance is closer then last found one
            if (dist < minDist)
            {
                minDist = dist;
                tMin = closest.transform;
            }
            if (playerDist < minDist)
            {
                minDist = playerDist;
                tMin = playerTransform.transform;
            }
        }
        return tMin;
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
