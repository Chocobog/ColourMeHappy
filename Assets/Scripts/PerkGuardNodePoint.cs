using UnityEngine;
using System.Collections;
/*
 * Written by: Jake Nye
 * Date: 29/4/2016 
 * 
 * Comments: This script constructs the gizmos for the water rock 
 *           monster's waypoints that are used wihtin the FSM.
*/
public class PerkGuardNodePoint : MonoBehaviour {

    // visual aid to assist in testing the monster guards movement
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f);
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}
