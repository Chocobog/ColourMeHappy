using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections;

public class passedFourth : MonoBehaviour {

    //Canvas
    public Canvas movement;
    public Canvas Shooting;
    public Canvas retrieveFlag;
    public Canvas usingPerks;
    public Canvas incEnemies;
    public Canvas tutFinished;

    public GameObject OoBWall;

    public Collider collide;

    // method Start() gets called once the script is invoked
    public void Start() {
        
    }

    //Ontrigger call to register end of tutorial!
    public void OnTriggerEnter(Collider col) {
        Debug.Log("collider hit...");
        if (col.gameObject.tag == "Player" && usingPerks.enabled == true) {
            Debug.Log("collider in effect");
            OoBWall.SetActive(false);
            usingPerks.enabled = false;
            tutFinished.enabled = true;

            //StartCoroutine(LoadLevel());
        }
    }

}
