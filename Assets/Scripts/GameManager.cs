using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private Vector2 screenSize;
	public GUIText OutOfBounds; // out of bounds text
    private float outOfBoundsTimer = 5.0f;
    public bool boundFlag;
    private string textTime;

    // Use this for initialization
    void Start() {
        OutOfBounds.enabled = false; // hide the level completed text object
		screenSize = new Vector2(Screen.width, Screen.height);
        boundFlag = false;
	}

	// OnTriggerEnter is called when a collider enters this trigger
	void OnTriggerEnter(Collider col) {
			//if player out of bounds
			if(col.gameObject.tag == "Player")
               boundFlag = true;
	}

    //onTriggerExit is called when a collider exits this trigger
    void OnTriggerExit(Collider col) {
        //if player in playing field
        if (col.gameObject.tag == "Player") {
            boundFlag = false;
            outOfBoundsTimer = 5.0f;
            OutOfBounds.enabled = false;
        }
    }

    // Update is called once per frame
    void Update() {
        if (boundFlag == true){
            //start counter
            outOfBoundsTimer -= 1 * Time.deltaTime;
            textTime = string.Format("{0:00}", outOfBoundsTimer);
            OutOfBounds.text = "Out of bounds, please return to the map in " + textTime;
            OutOfBounds.pixelOffset = new Vector2(0, 180); // ensure text is centred on screen
            OutOfBounds.enabled = true;
        }
    }
}
