using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour {

    public int ammoGiven = 5;

    void onTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Player")
        {
            c.SendMessage("ammoPickup", ammoGiven);
            Destroy(gameObject);
        }
    }
}
