using UnityEngine;
using System.Collections;

public class healthPickup : MonoBehaviour {

    public int healthGiven = 10;

    void onTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            c.SendMessage("healthPickup", healthGiven);
            Destroy(gameObject);
        }
    }
}
