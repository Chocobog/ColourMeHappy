using UnityEngine;
using System.Collections;

public class passedFirst : MonoBehaviour {

    public Canvas shooting; // allows us to disable this
    public Canvas movement; //keep this disabled if need be
    public Canvas retrieveFlag; // includes well done & go get the flag txt

    public void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player") {
            //movement.enabled = false;
            shooting.enabled = false;
            retrieveFlag.enabled = true;
            Destroy(this.gameObject);
        }

    }

}
