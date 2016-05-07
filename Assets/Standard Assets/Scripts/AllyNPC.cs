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
public class AllyNPC : MonoBehaviour
{
    //Enemy/Ally Flag
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

    //animation
    Animator animator;

    //Player HUD update
    public string allyFlagLocation;
    public int scoreEnemy; //score of enemy team

    //flags and flag locations
    public string opposingFlag = "BlueFlag";
    public string allyFlag = "RedFlag";
    private string atBase = "At Base - ";
    private string taken = "Taken - ";

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

    float randomDecision; //random decision that AI makes
    float lastDistance; //last distance found between you and player/player ally
    float currentDistance; //Current distance found between you and player/player ally
    public bool foundSomeone; //if the enemy has found someone

    //Initialisation
    void Start()
    {
        opposingFlag = "BlueFlag";
        allyFlag = "RedFlag";
        atBase = "At Base - ";
        taken = "Taken - ";

        elapsedTime = 0.0f;
        chaseRange = 100f;
        attackRange = 65f;
        allyFlagLocation = atBase;

        // Target enemies
        playerTransform = GameObject.FindGameObjectWithTag("Player");
        PlayerAllies = GameObject.FindGameObjectsWithTag("Ally");
        enemyFriends = GameObject.FindGameObjectsWithTag("Enemy");

        lastDistance = Mathf.Infinity;
        foundSomeone = false;
        invulnerable = false;
        respawnReset = 10f;
        respawnCountdown = respawnReset; // set respawn timer
        showMarker = false;
        scoreUpdate = 10;
        isEnemyFlagCarrier = false;
        FC = false;
  
        nav = GetComponent<NavMeshAgent>(); //Get the enemy nav mesh
        animator = GetComponent<Animator>(); //Get the enemy animator
    }

    // Update each frame
    void Update()
    {
        elapsedTime += Time.deltaTime; // Update the time
        // enemy has no health left
        if (health <= 0)
        {
            respawnCountdown -= 1 * Time.deltaTime; //start counter
            //Only plays this animation once
            animator.Play("dead");
            respawnEffect.SetActive(true); //respawn effect
            nav.Stop();            
            invulnerable = true;
            if (flag != null)
            {
                Destroy(flag.gameObject); //if enemy has the flag, destroy when the are defeated
                allyFlagLocation = atBase;
                fp.allyFlagLocation = atBase;//update when destroyed
                FC = false;
                isEnemyFlagCarrier = false;
                //Update on enemies that FC has been destroyed
                foreach (GameObject friends in enemyFriends)
                    friends.SendMessage("flagTaken", false);
            }
            //if player shot last bullet to kill enemy update score
            if (defeater.Equals(playerTransform.tag) && !invulnerable)
            {
                fp = playerTransform.GetComponent<FirstPersonController>();
                fp.playerScore += scoreUpdate;
            }
            //take back to enemy base spawn position, chosen time of 3 second offset to show player visual effect
            if (transform.position != enemySpawnPositions[0].position && (int)respawnCountdown == (int)respawnReset - 3)
                nav.Warp(enemySpawnPositions[0].position);
            //respawn over - set back to default
            if ((int)respawnCountdown == 0)
            {
                nav.ResetPath(); //reset navigation path
                health = 100;
                invulnerable = false;
                respawnCountdown = respawnReset;
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
                    Debug.Log("1");
                    animator.Play("idle pose with a gun");
                    nav.SetDestination(GameObject.FindGameObjectWithTag("Ally").transform.position);
                }
                //If the player comes into the chase distance but not in attack range
                else if (Vector3.Distance(transform.position, playerTransform.transform.position) <= chaseRange && Vector3.Distance(transform.position, playerTransform.transform.position) > attackRange)
                {
                    Debug.Log("2");
                    animator.Play("idle pose with a gun");
                    nav.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
                }
                //if no allies or player is within distance 
                else if (Vector3.Distance(transform.position, playerTransform.transform.position) > chaseRange && Vector3.Distance(transform.position, ally.transform.position) > chaseRange)
                {
                    
                    animator.Play("idle");
                    fp = playerTransform.GetComponent<FirstPersonController>(); //used to get location of enemy flag
                    //if enemy flag has been taken and this instance is the flag carrier
                    if (FC && fp.enemyFlagLocation.Equals(taken))
                    {
                        Debug.Log("3");
                        nav.SetDestination(GameObject.FindGameObjectWithTag("enemySafeSpot").transform.position);
                    }
                    //if player has taken enemy flag
                    else if (fp.enemyFlagLocation.Equals(taken))
                    {
                        //if distance to player flag is < then distance to enemy flag then go to the player flag
                        if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag(opposingFlag).transform.position) < Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag(allyFlag).transform.position))
                        {
                            Debug.Log("8");
                            nav.SetDestination(GameObject.FindGameObjectWithTag(opposingFlag).transform.position); //get enemy flag
                        }
                        else
                        {
                            Debug.Log("9");
                            nav.SetDestination(GameObject.FindGameObjectWithTag(allyFlag).transform.position); //get enemy flag
                        }
                    }
                    //if your the flag carrier and enemy flag is at the base
                    else if(FC && fp.enemyFlagLocation.Equals(atBase))
                    {
                        Debug.Log("4");
                        nav.SetDestination(GameObject.FindGameObjectWithTag(allyFlag).transform.position); //go back to flag for capture
                    }
                    //if this enemy is not the FC and we have taken player flag OR if both teams have taken the flag
                    else if (!FC && isEnemyFlagCarrier || allyFlagLocation.Equals(taken) && fp.enemyFlagLocation.Equals(taken))
                    {

                        //Randomly choose to find allies/player and destroy or follow to protect the FC
                        if (randomDecision == 0)
                        {
                            Debug.Log("5");
                            nav.SetDestination(closestEnemy(PlayerAllies).position); //Go to closest allie or player
                        }
                        else
                        {
                            Debug.Log("6");
                            nav.SetDestination(GameObject.FindGameObjectWithTag(opposingFlag).transform.position); //Go to FC
                            //if within 20f of the FC
                            if (Vector3.Distance(nav.transform.position, GameObject.FindGameObjectWithTag(opposingFlag).transform.position) < 20f)
                            {
                                nav.SetDestination(GameObject.FindGameObjectWithTag(allyFlag).transform.position);
                            }
                        }
                    }
                    //if blue flag is at the base, attempt to capture flag
                    else if (allyFlagLocation.Equals(atBase) && GameObject.FindGameObjectWithTag(opposingFlag))
                    {
                        Debug.Log("7");
                        nav.SetDestination(GameObject.FindGameObjectWithTag(opposingFlag).transform.position);
                    }
                }
                //if the player or players ally comes into the attack distance
                if (Vector3.Distance(transform.position, ally.transform.position) <= attackRange)
                {
                    enemyEncountered(ally);
                    foundSomeone = true;
                }
                if (Vector3.Distance(transform.position, playerTransform.transform.position) <= attackRange)
                {
                    enemyEncountered(playerTransform);
                    foundSomeone = true;
                }
            }
            //if no player or player ally is found reset distance
            if (!foundSomeone)
                lastDistance = Mathf.Infinity;
            else
                foundSomeone = false; //reset flag
        }
    }

    //Collisions with the enemy
    public void OnTriggerEnter(Collider c)
    {
        //if this is not the enemies team flag then take the flag
        if (c.transform.tag == opposingFlag)
        {
            allyFlagLocation = taken;
            fp.allyFlagLocation = taken;
            //Add flag to player 
            flag = c.transform;
            flag.transform.parent = transform;
            flag.transform.position = flagHolder.transform.position;
            c.enabled = false;
            //update location of flag
            FC = true;
            isEnemyFlagCarrier = true;
            foreach (GameObject friends in enemyFriends)
            {
                friends.SendMessage("flagTaken", true);
            }
        }

        //if enemy returns to flag with opposing team flag
        if (c.transform.tag == allyFlag && flag != null)
        {
            FC = false;
            isEnemyFlagCarrier = false;
            fp.scoreEnemy++;
            Destroy(flag.gameObject); //Destroy the flag gameObject
            //update location of flag
            allyFlagLocation = atBase;
            fp.allyFlagLocation = atBase;
            foreach (GameObject friends in enemyFriends)
            {
                friends.SendMessage("flagTaken", isEnemyFlagCarrier);
            }
        }
    }

    // lets the other enemies know if the flag has been taken or not and gives random number for AI decision
    public void flagTaken (bool b)
    {
        isEnemyFlagCarrier = b;
        randomDecision = Random.Range(0, 2);
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

    /*
    * Using the passed game object strafe along the x or y axis depending on the situation
    * @Gameobject p: player or player ally gameobject
    */
    private void enemyEncountered(GameObject p)
    {
        currentDistance = (transform.position - p.transform.position).magnitude; //distance between enemy and ally
        //strafe along y axis if your going back from the player and moving out of attack range, else strafe along x axis
        if (currentDistance > lastDistance)
        {
            nav.SetDestination(transform.position + new Vector3(0.0f, 5.0f, 0.0f));
            lastDistance = currentDistance; //update last distance
        }
        else if (currentDistance <= lastDistance)
        {
            nav.SetDestination(transform.position + new Vector3(5.0f, 0.0f, 0.0f));
            lastDistance = currentDistance; //update last distance
        }
            
        //rotate enemy towards ally
        Quaternion enemyRotate = Quaternion.LookRotation(p.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, enemyRotate, Time.deltaTime * rotSpeed);
        ShootBullet();
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
            //if players health is above 0 then add player to find
            if (fp.health > 0)
            {
                if (playerDist < minDist)
                {
                    minDist = playerDist;
                    tMin = playerTransform.transform;
                }
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