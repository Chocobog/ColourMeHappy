using UnityEngine;
using System.Collections;

/*
 * Written by: AJ Abotomey
 * Modified by: Jake Nye on the 03/05/2016
 * Comments: This script handles all of the npc Finite State Machine state transitions throughout
 *           ColourMeHappy's game.
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
    public float chaseSpeed = 4.0f;

    

    private NavMeshAgent nav;
    public int waypointLocation = 0;
    public float centralPos;
    /*
     * Initialize the Finite state machine for the NPC tank
     */
    void Start() {
        
        curState = FSMState.Patrol;

        bDead = false;
        elapsedTime = 0.0f;

        // Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        //make the nav mesh accessible
        nav = GetComponent<NavMeshAgent>();

        if(!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");

	}


    // Update each frame
    void Update() {
        switch (curState) {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        // Update the time
        elapsedTime += Time.deltaTime;

        //
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

        // Go to dead state if no health left
        if (health <= 0)
            curState = FSMState.Dead;

        //Animator animator = GetComponent<Animator>();
        //animator for walking on the ground
        //animator.SetBool("grounded", true);

        //Animation animator = GetComponent<Animation>();
        //animator.Play("loop_walk_funny");

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
            // rotate to destination position
            //Quaternion targetRot = Quaternion.LookRotation(destPos - transform.position);
            //GetComponent<Rigidbody>().MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotSpeed));

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

        

		// Transitions
        // Check the distance
        // When the distance is near, transition to chase state
        if (Vector3.Distance(transform.position, playerTransform.position) <= chaseRange) {
            curState = FSMState.Chase;
        }
    }

    //GameObject robot; // necessary to access this for future changes to the robot models animation state changes
    /*
     * Chase state
	 */
    protected void UpdateChaseState() {

        // NavMeshAgent move code goes here
        nav.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        //if ((dist <= chaseRange) & (dist > attackRangeStop))
        //{
        //    nav.Stop();
        //    Vector3 ownPos = new Vector3(transform.position.x, 0.0f, playerTransform.position.z);
        //    Vector3 playerPos = new Vector3(playerTransform.position.x, 0.0f, playerTransform.position.z);

        //    GetComponent<Rigidbody>().MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerPos - ownPos), rotSpeed * Time.deltaTime));

        //    GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * (Time.deltaTime * chaseSpeed));

        //    GameObject pGuard = GameObject.FindGameObjectWithTag("perkGuard");
        //    Quaternion guardRotate = Quaternion.LookRotation(playerPos - transform.position);
        //    pGuard.transform.rotation = Quaternion.Slerp(pGuard.transform.rotation, guardRotate, Time.deltaTime * rotSpeed);

        //}
        //else {
        //    nav.Resume();
        //}

		// Transitions
        // Check the distance with player tank
        // When the distance is near, transition to attack state
		
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

        GameObject pGuard = GameObject.FindGameObjectWithTag("perkGuard");
        Quaternion guardRotate = Quaternion.LookRotation(playerTransform.position - transform.position);
        pGuard.transform.rotation = Quaternion.Slerp(pGuard.transform.rotation, guardRotate, Time.deltaTime * rotSpeed);
        
        
        // stop to attack the player
        if (dist <= attackRangeStop) {
            // whilst attacking chase the player
            //nav.Stop();
            //nav.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);

            // Shoot the bullets
            ShootBullet();
                    //nav.Stop();
            
        }

        // Transitions
        // Check the distance with the player tank
        if (dist > attackRange)
        {
            //nav.Stop();
            curState = FSMState.Chase;

        }
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
            nav.enabled = false;
        }
    }


    /*
     * Shoot Bullet
     */
    private void ShootBullet() {
        if (elapsedTime >= shootRate) {
			if ((bulletSpawnPoint) & (bullet)) {
            	// Shoot the bullet
            	Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
			}
            elapsedTime = 0.0f;
        }
    }

    // Apply Damage if hit by bullet
    public void ApplyDamage(int damage ) {
    	health -= damage;
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
