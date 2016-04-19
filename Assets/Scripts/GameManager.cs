using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
* @Written By: Joshua Hurn
* @Last Modified: 04/04/2016
*
* This class managed the out of bounds areas that players are not allowed to enter
* if they enter these areas they are given a 5 second count to return to the playable area
* before being killed.
*/
public class GameManager : MonoBehaviour
{

    public GUIText OutOfBounds; // out of bounds text
    private float outOfBoundsTimer = 5.0f; // time player is allowed in out of bounds area
    private bool boundFlag; // triggered if they go out of bounds
    private string textTime; // text to tell player they have gone out of bounds

    //private bool outOfBoundsFlash;
    //private Image boundsImage;
    //private float flashSpeed = 5.0f;
    //private Color flashColour = new Color(1f, 0f, 0f, 0.1f);


    // Use this for initialization
    void Start()
    {
        OutOfBounds.enabled = false; // hide the level completed text object
        boundFlag = false;
        //outOfBoundsFlash = false;
    }

    // OnTriggerEnter is called when a collider enters this trigger
    void OnTriggerEnter(Collider col)
    {
        //if player out of bounds
        if (col.gameObject.tag == "Player")
        {
            boundFlag = true;
            //outOfBoundsFlash = true;
        }
    }

    //onTriggerExit is called when a collider exits this trigger
    void OnTriggerExit(Collider col)
    {
        //if player in playing field
        if (col.gameObject.tag == "Player")
        {
            boundFlag = false;
            outOfBoundsTimer = 5.0f;
            OutOfBounds.enabled = false;
            //outOfBoundsFlash = false;
        }
    }

    //onTriggerStay is called when a collider stays in this trigger
    void OnTriggerStay(Collider col)
    {
        if (boundFlag == true)
        {
            outOfBoundsTimer -= 1 * Time.deltaTime; //start counter
            textTime = string.Format("{0:0}", outOfBoundsTimer); //Show out of bounds message
            OutOfBounds.text = "Out of bounds, please return to the map in " + textTime + " seconds";
            OutOfBounds.pixelOffset = new Vector2(0, 180); // ensure text is centred on screen
            OutOfBounds.enabled = true;
            //if timer reaches 0 Destroy object and reset timer
            if (textTime.Equals("0"))
            {
                Destroy(col.gameObject);
                outOfBoundsTimer = 5.0f;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (outOfBoundsFlash){
        //  boundsImage.color = flashColour;
        //} else {
        //  boundsImage.color = Color.Lerp(boundsImage.color, Color.clear, flashSpeed * Time.deltaTime);
        //}
    }

}
