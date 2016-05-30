using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

/*
 * Written by: AJ Abotomey
 * Modified by: Jake Nye 
 * Last Modified: 30/05/2016
 * 
 * This class handles all of the npc Finite State Machine state transitions throughout
 * ColourMeHappy's game tutorial.
*/

public class tutFSM : MonoBehaviour
{
    //states
    public enum FSMState
    {
        None,
        Patrol,
        Chase,
        Attack,
        Dead,
    }

    // Current state that the NPC is reaching
    public FSMState curState;

    protected Transform playerTransform;// Player Transform

    // Bullet
    public GameObject bullet;
    public GameObject bulletSpawnPoint;

    // Bullet shooting rate
    public float shootRate = 0.4f;
    protected float elapsedTime;

    // Whether the NPC is destroyed or not
    protected bool bDead;
    public int health = 100;

    // Ranges for chase and attack
    public float chaseRange = 1000.0f;
    public float attackRange = 110.0f;
    public float attackRangeStop = 45.0f;
    public int rotSpeed = 11;
    public float chaseSpeed = 3.0f;


    private NavMeshAgent nav; //navmesh
    public GameObject waypoint;

    //respawning
    public float respawnCountdown;
    public float respawnReset;
    public GameObject respawnEffect;
    public Transform[] guardSpawnPosition = new Transform[1];
    public float guardSpawn = 8.0f;
    public float npcSpawnTimer = 0.0f;
    public GameObject botGuard;
    public GameObject botGuard2;
    public float takeOff = 4.0f;

    //who defeated this enemy
    public string defeater;
    public bool invulnerable;
    FirstPersonController fp;


     // Initialize the Finite state machine
    void Start()
    {
        curState = FSMState.Patrol;
        bDead = false;
        elapsedTime = 0.0f;
        respawnReset = 8f;
        // set respawn timer
        respawnCountdown = respawnReset;
        // Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        //make the nav mesh accessible
        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(playerTransform.position);

        if (!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");
    }


    // Update each frame
    void Update()
    {
        npcSpawnTimer = npcSpawnTimer * Time.deltaTime;
        switch (curState)
        {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        // Update the time
        elapsedTime += Time.deltaTime;

        // Go to dead state if no health left
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else {
            //says that the FSM is not dead
            bDead = false;
            //distance from player to FSM
            float distance = Vector3.Distance(transform.position, playerTransform.position);

            //State transitions
            if ((distance <= chaseRange) & (distance > attackRangeStop))
            {
                curState = FSMState.Chase;
            }
            else if (distance > chaseRange)
            {
                curState = FSMState.Patrol;
            }

            // Attack state change
            if (distance <= attackRange)
            {
                curState = FSMState.Attack;
            }
        }


    }

    /*
    * Used to find out who shot the final bullet to kill the enemy
    * @string s: Origin of the bullet that was shot
    */
    public void defeatedBy(GameObject s)
    {
        defeater = s.tag;
    }

    protected Vector3 destPos;

     // Patrol state
    protected void UpdatePatrolState()
    {
        // Check the distance
        if (Vector3.Distance(transform.position, playerTransform.position) <= chaseRange)
        {
            curState = FSMState.Chase;
        }
    }

    // Chase state
    protected void UpdateChaseState()
    {
        //Move to player
        nav.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        if (dist <= attackRange)
        {
            curState = FSMState.Attack;
        }

        // Go back to patrol is it become too far
        else if (dist >= chaseRange)
        {
            curState = FSMState.Patrol;
        }
    }

	//Attack state
    protected void UpdateAttackState()
    {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        Quaternion guardRotate = Quaternion.LookRotation(playerTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, guardRotate, Time.deltaTime * rotSpeed);

        // stop to attack the player
        if (dist <= attackRangeStop)
        {
            // Shoot the bullets
            ShootBullet();
        }
        // Check the distance 
        if (dist > attackRange)
            curState = FSMState.Chase;
        // Transition to patrol 
        else if (dist > chaseRange)
        {
            nav.Resume();
            curState = FSMState.Patrol;
        }
    }

    // Dead state
    protected void UpdateDeadState()
    {
        // Show the dead animation with some physics effects
        if (!bDead)
        {
            bDead = true;
            nav.Stop();
            invulnerable = true;
        }
    }

    //Shoot Bullet
    private void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            if ((bulletSpawnPoint) & (bullet))
            {
                GameObject spawnOrigin = (GameObject)Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
                spawnOrigin.SendMessage("spawnOrigin", gameObject); //send message to the bullet to say who shot this bullet
            }
            elapsedTime = 0.0f;
        }
    }

    /*
    * Apply Damage if hit by bullet
    * @int damage: damage taken from bullet
    */
    public void takeDamage(int damage)
    {
        health -= damage;
    }
}
