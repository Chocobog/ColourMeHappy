using UnityEngine;
using System.Collections;


/*
* @Written by: AJ Abotomey
* @Modified by: Joshua Hurn
* @Last Modified: 23/04/2016
*
* This class controls the bullets shot from the players gun. Inflicts damage where needed and applies splatter effect on contact
*/
public class Bullet : MonoBehaviour
{
	public GameObject Explosion; //Splash Effect
    public float speed = 200.0f; //speed of bullet
	public float lifeTime = 3.0f; //lifetime of bullet
	public int damage = 25; //damage from bullet
    private Vector3 newPos; //position of the bullet
    public GameObject shooter; // who shot the bullet
	
    //Initialisation
	void Start()
	{
		Destroy(gameObject, lifeTime);
        damage = 25;
    }

    //Updated every frame
    void Update()
	{
		// future position if bullet doesn't hit any colliders
		newPos = transform.position + transform.forward * speed * Time.deltaTime;

        // see if bullet hits a collider
        RaycastHit hit;
        if (Physics.Linecast(transform.position, newPos, out hit))
        {
            //if the bullet hits the person who shot the bullet do nothing
            if (hit.collider && hit.collider.gameObject.tag != shooter.tag)
            {
               // create explosion and destroy bullet
                transform.position = hit.point;
                //push forward on the object so that it doesn't cause visual issues
                hit.point = hit.point + hit.normal * 0.2f;
                if (Explosion)
                {
                    Instantiate(Explosion, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                    Destroy(gameObject);
                }

                //apply damage to object
               GameObject obj = hit.collider.gameObject;
                if (obj.tag == "Player" || obj.tag == "Enemy" || obj.tag == "Ally" || obj.tag == "perkGuard")
                {
                    if (shooter.tag == "Player" && obj.tag == "Ally" || shooter.tag == "Ally" && obj.tag == "Player")
                    {
                        return;
                    }
                    else
                    {
                        obj.SendMessage("takeDamage", damage);
                        obj.SendMessage("defeatedBy", shooter);
                    }
                }
            }
        }
        else
            transform.position = newPos; // didn't hit - move to newPos
    }

    /*
    * Returns who the bullet belongs to (who shot the bullet)
    * @Gameobject g: Gameobject of who shot the bullet
    */
    void spawnOrigin(GameObject g)
    {
        shooter = g;
    }

}