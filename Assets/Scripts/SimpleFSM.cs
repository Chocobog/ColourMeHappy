using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

/*
 * Written by: AJ Abotomey
 * Modified by: Jake Nye 
 * Last Modified: 03/05/2016
 * 
 * This class handles all of the npc Finite State Machine state transitions throughout
 * ColourMeHappy's game.
*/

public class SimpleFSM : MonoBehaviour 
{
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

    //waypoints
	public GameObject[] waypointList;

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
	public float chaseRange = 40.0f;
	public float attackRange = 30.0f;
	public float attackRangeStop = 25.0f;
    public int rotSpeed = 11;
    public float chaseSpeed = 3.0f;

    private NavMeshAgent nav;
    public int waypointLocation = 0;
    public float centralPos;

    //respawning
    public float respawnCountdown;
    public float respawnReset;
    public GameObject respawnEffect;
    public Transform[] guardSpawnPosition = new Transform[2];
	public float guardSpawn = 8.0f;
	public float npcSpawnTimer = 0.0f;
	public GameObject botGuard;
	public GameObject botGuard2;
	public float takeOff = 4.0f;

    //who defeated this enemy
    public string defeater;
    public bool invulnerable;
    FirstPersonController fp;

    // audio reboot clip
    public AudioClip reboot;

    //player score update
    private int scoreUpdate;

    /*
     * Initialize the Finite state machine for the NPC tank
     */
    void Start() {
        
        curState = FSMState.Patrol;

        bDead = false;
		elapsedTime = 0.0f;
        //scoreUpdate = 10;
        respawnReset = 8f;
        // set respawn timer
        respawnCountdown = respawnReset;
        // Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
		playerTransform = objPlayer.transform;

        //make the nav mesh accessible
        nav = GetComponent<NavMeshAgent>();

        if (!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");

	}


    // Update each frame
    void Update() {
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
		if (health <= 0) {

			// enemy has no health left
			respawnCountdown -= 1 * Time.deltaTime; //start counter
			respawnEffect.SetActive (true); //respawn effect
            curState = FSMState.Dead;
            //reset to default
            if ((int)respawnCountdown == 0) {
                nav.Warp(guardSpawnPosition[0].position);
                nav.ResetPath(); //reset navigation path
				health = 100;
				invulnerable = false;
				respawnCountdown = respawnReset;
				respawnEffect.SetActive (false);
            }
            
        } else {
            //says that the FSM is not dead
            bDead = false;
            //distance from player to FSM
            float distance = Vector3.Distance (transform.position, playerTransform.position);

			//State transitions
			if ((distance <= chaseRange) & (distance > attackRangeStop)) {
				curState = FSMState.Chase;
			} else if (distance > chaseRange) {
				curState = FSMState.Patrol;
			}

			// Attack state change
			if (distance <= attackRange) {
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

	/*
     * Patrol state
     */
    protected void UpdatePatrolState() {
        
        //move to next waypoint
        if (Vector3.Distance(nav.transform.position, waypointList[waypointLocation].transform.position) <= 2.0f)
        {
            waypointLocation++;

            //loop back
            if (waypointLocation >= waypointList.Length)
                waypointLocation = 0;

            nav.SetDestination(waypointList[waypointLocation].transform.position);
            
        }
        //move to first waypoint or resume going to waypoint
        else 
        {
            nav.SetDestination(waypointList[waypointLocation].transform.position);
        }

        // Check the distance
        // When the distance is near, transition to chase state
        if (Vector3.Distance(transform.position, playerTransform.position) <= chaseRange) {
			curState = FSMState.Chase;
        }
    }
    /*
     * Chase state
	 */
    protected void UpdateChaseState() {
        //Move to player
        nav.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);

        float dist = Vector3.Distance(transform.position, playerTransform.position);		
		if (dist <= attackRange) {
            curState = FSMState.Attack;
        }
        // Go back to patrol is it become too far
        else if (dist >= chaseRange) {
            curState = FSMState.Patrol;
		}
	}

    /*
	 * Attack state
	 */
    //public GameObject perkGuard;
    protected void UpdateAttackState() {
        float dist = Vector3.Distance(transform.position, playerTransform.position);
        Quaternion guardRotate = Quaternion.LookRotation(playerTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, guardRotate, Time.deltaTime * rotSpeed);

        // stop to attack the player
        if (dist <= attackRangeStop) {
            // Shoot the bullets
            ShootBullet();            
        }
        // Check the distance with the player tank
        if (dist > attackRange)
            curState = FSMState.Chase;
        // Transition to patrol if the tank is too far
        else if (dist > chaseRange)
        {
            nav.Resume();
            curState = FSMState.Patrol;
        }
    }

    /*
     * Dead state
     */
    protected void UpdateDeadState() {
		// Show the dead animation with some physics effects
		if (!bDead) {
            bDead = true;
            nav.Stop();
            invulnerable = true;
            Debug.Log ("hitting this!");
		}
    }
			

    /*
     * Shoot Bullet
     */
    private void ShootBullet() {
        if (elapsedTime >= shootRate) {
			if ((bulletSpawnPoint) & (bullet)) {
                GameObject spawnOrigin = (GameObject)Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
                spawnOrigin.SendMessage("spawnOrigin", gameObject); //send message to the bullet to say who shot this bullet
            }
            elapsedTime = 0.0f;
        }
    }

    // Apply Damage if hit by bullet
    public void takeDamage(int damage ) {
    	health -= damage;
        Debug.Log(health);
    }


	void OnDrawGizmos () {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, chaseRange);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRangeStop);
	}

}
