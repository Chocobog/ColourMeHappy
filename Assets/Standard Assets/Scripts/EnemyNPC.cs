using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/*
* Written by: Joshua Hurn
* Last Modified: 08/05/2016
*
* This class controls the enemy NPC and their behaviour in the environment
* Enemy NPC logic is a rule based system
*/
public class EnemyNPC : MonoBehaviour
{
    //Enemy/Ally Flag
    protected GameObject playerTransform;// Player Transform
    protected GameObject[] PlayerAllies; //Player Allies Transform
    protected GameObject[] enemyFriends; //friends of the enemy

    // Bullet
    public GameObject bullet;
    public GameObject bulletSpawnPoint;

    // Bullet shooting rate
    public float shootRate = 1.0f;
    protected float elapsedTime;
    public int rotSpeed = 11;

    // Whether the NPC is destroyed or not
    public int health = 100;

    public float lineOfSightAngle = 50f;
    private SphereCollider col; // Reference to the sphere collider trigger
    private BoxCollider box; //Reference to the box collider trigger

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
    public bool respawned;

    //respawning
    public float respawnCountdown;
    public float respawnReset;
    public GameObject respawnEffect;
    public Transform[] enemySpawnPositions = new Transform[6];

    //navmesh agent (enemy)
    private NavMeshAgent nav;
    NavMeshPath path; //navmesh path

    //player score update
    private int scoreUpdate;

    //For radar effect - FPS script
    public GameObject marker;
    public bool showMarker;
    private float radarCountdown;
    

    public bool FC; //Flag carrier
    public bool isEnemyFlagCarrier; //changes if any other enemy picks up the flag
    FirstPersonController fp; //first person controller

    float randomDecision; //random decision that AI makes

    //Initialisation
    void Start()
    {
        col = GetComponent<SphereCollider>();
        box = GetComponent<BoxCollider>();
        path = new NavMeshPath();

        opposingFlag = "BlueFlag";
        allyFlag = "RedFlag";
        atBase = "At Base - ";
        taken = "Taken - ";

        elapsedTime = 0.0f;
        allyFlagLocation = atBase;

        // Target enemies
        playerTransform = GameObject.FindGameObjectWithTag("Player");
        PlayerAllies = GameObject.FindGameObjectsWithTag("Ally");
        enemyFriends = GameObject.FindGameObjectsWithTag("Enemy");

        invulnerable = false;
        respawnReset = 10f;
        respawnCountdown = respawnReset; // set respawn timer
        showMarker = false;
        scoreUpdate = 10;
        isEnemyFlagCarrier = false;
        FC = false;
        respawned = false;
  
        nav = GetComponent<NavMeshAgent>(); //Get the enemy nav mesh
        animator = GetComponent<Animator>(); //Get the enemy animator
    }

    // Update each frame
    void FixedUpdate()
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
                fp.isBlueFlagRetrieved = true;
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
            int randomSpawn = Random.Range(0, enemySpawnPositions.Length);
            Vector3 spawnPoint = enemySpawnPositions[randomSpawn].position;
            Collider[] hitColliders = Physics.OverlapSphere(spawnPoint, 0.1f);
            //take back to random enemy base spawn position, after 3 seconds (visual effect for player)
            if (hitColliders.Length == 0 && (int)respawnCountdown <= (int)respawnReset - 3 && !respawned)
            {
                nav.Warp(spawnPoint); //warp back to enemy spawn point
                respawned = true;
            }
            else if (hitColliders.Length > 0)
                randomSpawn = Random.Range(0, enemySpawnPositions.Length); //choose another random location if this one is occupied

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
                //if no allies or player is within distance 
                animator.Play("idle");
                fp = playerTransform.GetComponent<FirstPersonController>(); //used to get location of enemy flag
                //if enemy flag has been taken and this instance is the flag carrier
                if (FC && fp.enemyFlagLocation.Equals(taken))
                {
                    Debug.Log("3");
                    nav.CalculatePath(GameObject.FindGameObjectWithTag("enemySafeSpot").transform.position, path);
                    nav.SetPath(path);
                }
                //if this enemy is not the FC and we have taken player flag OR if both teams have taken the flag
                else if (!FC && isEnemyFlagCarrier || allyFlagLocation.Equals(taken) && fp.enemyFlagLocation.Equals(taken))
                {

                    //Randomly choose to find allies/player and destroy or follow to protect the FC
                    if (randomDecision == 0)
                    {
                        Debug.Log("5");
                        nav.CalculatePath(closestEnemy(PlayerAllies).position, path); //Go to closest allie or player
                        nav.SetPath(path);
                    }
                    else
                    {
                        Debug.Log("6");
                        nav.CalculatePath(GameObject.FindGameObjectWithTag(opposingFlag).transform.position, path); //Go to FC
                        nav.SetPath(path);
                        //if within 20f of the FC
                        if (Vector3.Distance(nav.transform.position, GameObject.FindGameObjectWithTag("enemySafeSpot").transform.position) < 100f)
                        {
                            nav.CalculatePath(closestEnemy(PlayerAllies).position, path); //Go to closest allie or player
                            nav.SetPath(path);
                        }
                        else if (Vector3.Distance(nav.transform.position, GameObject.FindGameObjectWithTag(opposingFlag).transform.position) < 20f)
                        {
                            nav.CalculatePath(GameObject.FindGameObjectWithTag(allyFlag).transform.position, path);
                            nav.SetPath(path);
                        }
                    }
                }
                //if player has taken enemy flag and this instance is now an FC
                else if (fp.enemyFlagLocation.Equals(taken))
                {
                    //if distance to player flag is < then distance to enemy flag then go to the player flag
                    if (Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag(opposingFlag).transform.position) < Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag(allyFlag).transform.position))
                    {
                        Debug.Log("8");
                        nav.CalculatePath(GameObject.FindGameObjectWithTag(opposingFlag).transform.position, path); //get player flag
                        nav.SetPath(path);
                    }
                    else
                    {
                        Debug.Log("9");
                        nav.CalculatePath(GameObject.FindGameObjectWithTag(allyFlag).transform.position, path); //go to enemy flag
                        nav.SetPath(path);
                    }
                }
                //if your the flag carrier and enemy flag is at the base
                else if(FC && fp.enemyFlagLocation.Equals(atBase) && GameObject.FindGameObjectWithTag(allyFlag))
                {
                    Debug.Log("4");
                    nav.CalculatePath(GameObject.FindGameObjectWithTag(allyFlag).transform.position, path); //go back to flag for capture
                    nav.SetPath(path);
                }

                //if blue flag is at the base, attempt to capture flag
                else if (allyFlagLocation.Equals(atBase) && GameObject.FindGameObjectWithTag(opposingFlag))
                {
                    Debug.Log("7");
                    nav.CalculatePath(GameObject.FindGameObjectWithTag(opposingFlag).transform.position, path);
                    nav.SetPath(path);
                }
            }
        }
    }

    //Triggers with the enemy
    public void OnTriggerEnter(Collider c)
    {
        //if bullet hits the enemy, stick to the enemy
        if (c.transform.tag == "BulletSplat")
            c.transform.parent = gameObject.transform;

        //if this is not the enemies team flag then take the flag if hit by enemy box collider
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
            fp.isBlueFlagTaken = true;
            foreach (GameObject friends in enemyFriends)
            {
                friends.SendMessage("flagTaken", true);
            }
        }

        //if enemy returns to flag with opposing team flag if hit by enemy box collider
        if (c.transform.tag == allyFlag && flag != null)
        {
            FC = false;
            isEnemyFlagCarrier = false;
            fp.scoreEnemy++;
            Destroy(flag.gameObject); //Destroy the flag gameObject
            //update location of flag
            allyFlagLocation = atBase;
            fp.allyFlagLocation = atBase;
            fp.isBlueFlagCaptured = true;
            foreach (GameObject friends in enemyFriends)
            {
                friends.SendMessage("flagTaken", isEnemyFlagCarrier);
            }
        }
    }

    /*
    * When anything stays within enemy radius
    * Tutorial from: https://www.youtube.com/watch?v=mBGUY7EUxXQ
    */
    public void OnTriggerStay(Collider c)
    {
        //if player within enemy radius
        if (c.gameObject == playerTransform)
        {
            Vector3 direction = c.transform.position - transform.position; //direction of player
            float angle = Vector3.Angle(direction, transform.forward); //angle from the direction

            if (angle < lineOfSightAngle * 0.5f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + transform.up, direction, out hit, 400))
                {
                    //if raycast hits the player and nothing else in front of the player
                    if (hit.collider.gameObject == playerTransform)
                    {
                        transform.LookAt(playerTransform.transform.position); //look at the player
                        Debug.Log("Found Player, Time to Destroy");
                        animator.Play("idle pose with a gun"); //animation
                        //if within a certain radius, run around the player and shoot
                        if (Vector3.Distance(transform.position, playerTransform.transform.position) > 150f)
                            nav.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);
                        else
                            nav.Move(transform.TransformDirection(Vector3.left) * (10 * Time.deltaTime)); //move to the left
                        ShootBullet();
                    }
                }
            }

        }
        return;
    }

    /* 
    * lets the other enemies know if the flag has been taken or not and gives random number for AI decision
    * @Bool b: boolean to say if there is an enemy flag carrier or not
    */
    public void flagTaken (bool b)
    {
        isEnemyFlagCarrier = b;
        randomDecision = Random.Range(0, 2);
    }

    // Shoot Bullet
    private void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            if ((bulletSpawnPoint) & (bullet))
            {
                
                GameObject spawnOrigin = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
                spawnOrigin.SendMessage("spawnOrigin", gameObject); //send message to the bullet to say who shot this bullet
                animator.Play("shot"); //animation for shooting
            }
            elapsedTime = 0.0f;
        }
    }

    /* 
    * Apply damage if hit by bullet
    * @int damage: damage applied to enemy
    */
    public void takeDamage(int damage)
    {
        health -= damage;
    }

    /*
    * Used to find out who shot the final bullet to kill the enemy
    * @string s: Origin of the bullet that was shot
    */
    public void defeatedBy(GameObject s)
    {
        defeater = s.tag;
        transform.LookAt(s.transform.position); //turn enemy towards the shooter
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
        foreach (GameObject playerAlly in g)
        {
            AllyNPC anpc = playerAlly.GetComponent<AllyNPC>();
            float dist = Vector3.Distance(playerAlly.transform.position, currentPos); // ally distance
            //if player distance or ally of player distance is closer then last found one
           if (anpc.health > 0)//if player allies health is above 0 then add player to search
            {
                if (dist < minDist)
                {
                    minDist = dist;
                    tMin = playerAlly.transform;
                }
            }
            //if players health is above 0 then add player to search
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

    //Drawn elements on the screen - DEBUGGING
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, Vector3.Angle(direction, transform.forward));

        Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position, direction);
    }
}