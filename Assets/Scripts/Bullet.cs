using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	//Explosion Effect
	public GameObject Explosion;
	
	public float speed = 200.0f;
	public float lifeTime = 3.0f;
	public int damage = 50;
    public string bulletOrigin;

    private Vector3 newPos;
	
	void Start()
	{
		Destroy(gameObject, lifeTime);
	}
	
	void Update()
	{
        //GameObject spawnOrigin = GetComponent(Bullet).spawn = gameObject;
		// future position if bullet doesn't hit any colliders
		newPos = transform.position + transform.forward * speed * Time.deltaTime;
		
		// see if bullet hits a collider
		RaycastHit hit;
		if (Physics.Linecast(transform.position, newPos, out hit))
		{
			if (hit.collider && hit.collider.gameObject.tag != bulletOrigin)
			{
				// create explosion and destroy bullet
				transform.position = hit.point;
                hit.point = hit.point + hit.normal * 0.2f;
				if (Explosion)
					Instantiate(Explosion, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
				Destroy(gameObject);
				
				// apply damage to object
				GameObject obj = hit.collider.gameObject;
				if (obj.tag == "Player" || obj.tag == "Enemy") 
					obj.SendMessage("ApplyDamage", damage);
			}
		}
		else
		{
			// didn't hit - move to newPos
			transform.position = newPos;
		}     
	}

    void spawnOrigin(GameObject g)
    {
        bulletOrigin = g.tag;
        Debug.Log(bulletOrigin);
    }
	
}