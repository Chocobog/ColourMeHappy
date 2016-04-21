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

    public static Bullet Instance;

    /// <summary>
    /// A single paint decal to instantiate
    /// </summary>
    public Transform PaintPrefab;

    private int MinSplashs = 5;
    private int MaxSplashs = 15;
    private float SplashRange = 2f;

    private float MinScale = 0.25f;
    private float MaxScale = 2.5f;


    private Vector3 newPos;
	
	void Start()
	{
		Destroy(gameObject, lifeTime);
	}

    void Awake()
    {
        if (Instance != null) Debug.LogError("More than one Painter has been instanciated in this scene!");
        Instance = this;

        if (PaintPrefab == null) Debug.LogError("Missing Paint decal prefab!");
    }

    void Update()
	{
		// future position if bullet doesn't hit any colliders
		newPos = transform.position + transform.forward * speed * Time.deltaTime;

        // see if bullet hits a collider
        RaycastHit hit;
        if (Physics.Linecast(transform.position, newPos, out hit))
        {
            //if the bullet hits the person who shot the bullet do nothing
            if (hit.collider && hit.collider.gameObject.tag != bulletOrigin)
            {
               // create explosion and destroy bullet
                transform.position = hit.point;
                //push forward on the object so that it doesn't cause visual issues
                hit.point = hit.point + hit.normal * 0.2f;
                if (Explosion)
                    Instantiate(Explosion, hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));
                Destroy(gameObject);

                //apply damage to object
               GameObject obj = hit.collider.gameObject;
                if (obj.tag == "Player" || obj.tag == "Enemy")
                    obj.SendMessage("ApplyDamage", damage);
            }
        }

        //      // Raycast
        //      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //      RaycastHit hit;

        //      if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        //      {
        //          // Paint!
        //          // Step back a little for a better effect (that's what "normal * x" is for)
        //          Paint(hit.point + hit.normal * (SplashRange / 4f));
        //      }
        //      else
        //{
        //	// didn't hit - move to newPos
        //	transform.position = newPos;
        //}     
    }

    void spawnOrigin(GameObject g)
    {
        bulletOrigin = g.tag;
        Debug.Log(bulletOrigin);
    }

    //public void Paint(Vector3 location)
    //{
    //    //DEBUG
    //    //mHitPoint = location;
    //    //mRaysDebug.Clear();
    //    //mDrawDebug = true;

    //    int n = -1;

    //    int drops = Random.Range(MinSplashs, MaxSplashs);
    //    RaycastHit hit;

    //    // Generate multiple decals in once
    //    while (n <= drops)
    //    {
    //        n++;

    //        // Get a random direction (beween -n and n for each vector component)
    //        var fwd = transform.TransformDirection(Random.onUnitSphere * SplashRange);

    //        //mRaysDebug.Add(new Ray(location, fwd));

    //        // Raycast around the position to splash everwhere we can
    //        if (Physics.Raycast(location, fwd, out hit, SplashRange))
    //        {
    //            // Create a splash if we found a surface
    //            var paintSplatter = GameObject.Instantiate(PaintPrefab,
    //                                                       hit.point,

    //                                                       // Rotation from the original sprite to the normal
    //                                                       // Prefab are currently oriented to z+ so we use the opposite
    //                                                       Quaternion.FromToRotation(Vector3.back, hit.normal)
    //                                                       ) as Transform;

    //            // Random scale
    //            var scaler = Random.Range(MinScale, MaxScale);

    //            paintSplatter.localScale = new Vector3(
    //                paintSplatter.localScale.x * scaler,
    //                paintSplatter.localScale.y * scaler,
    //                paintSplatter.localScale.z
    //            );

    //            // Random rotation effect
    //            var rater = Random.Range(0, 359);
    //            paintSplatter.transform.RotateAround(hit.point, hit.normal, rater);


    //            // TODO: What do we do here? We kill them after some sec?
    //            Destroy(paintSplatter.gameObject, 25);
    //        }

    //    }
    //}

}