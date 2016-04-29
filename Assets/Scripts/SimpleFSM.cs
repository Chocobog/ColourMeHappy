using UnityEngine;
using System.Collections;

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
	public float shootRate = 3.0f;
	protected float elapsedTime;

    // Whether the NPC is destroyed or not
    protected bool bDead;
    public int health = 100;

	// Ranges for chase and attack
	public float chaseRange = 35.0f;
	public float attackRange = 20.0f;
	public float attackRangeStop = 10.0f;


    private NavMeshAgent nav;
    public int waypointLocation = 0;

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

        //Get the tanks nav mesh
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

        // Go to dead state if no health left
        if (health <= 0)
            curState = FSMState.Dead;

        Animator animator = GetComponent<Animator>();
        //animator for walking on the ground
        animator.SetBool("grounded", true);
    }

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

		// Transitions
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

        // NavMeshAgent move code goes here
        nav.SetDestination(GameObject.FindGameObjectWithTag("Player").transform.position);

		// Transitions
        // Check the distance with player tank
        // When the distance is near, transition to attack state
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
    protected void UpdateAttackState() {

		// Transitions
		// Check the distance with the player tank
        float dist = Vector3.Distance(transform.position, playerTransform.position);
		if (dist > attackRange) {
            nav.Resume();
			curState = FSMState.Chase;
		}
        // Transition to patrol if the tank is too far
        else if (dist >= chaseRange) {
            nav.Resume();
			curState = FSMState.Patrol;
		}

        if (dist <= attackRangeStop)
        {
            nav.Stop();
        }

        // Shoot the bullets
        ShootBullet();
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
	}

}
