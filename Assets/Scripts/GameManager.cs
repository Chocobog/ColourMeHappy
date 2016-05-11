using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

/*
* @Written By: Joshua Hurn
* @Last Modified: 23/04/2016
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

    //fade out effects
    public float flashSpeed = 5f;
    public Color flashColour;
    private float alphaFadeValue = 0;
    public Texture fader; //place holder texture

    GameObject playerTransform;


    // Use this for initialization
    void Start()
    {
        OutOfBounds.enabled = false; // hide the level completed text object
        boundFlag = false;

    }


    // OnTriggerEnter is called when a collider enters this trigger
    void OnTriggerEnter(Collider col)
    {
        //if player out of bounds
        if (col.gameObject.tag == "Player")
        {
            boundFlag = true;
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
        }
    }

    //onTriggerStay is called when a collider stays in this trigger
    void OnTriggerStay(Collider col)
    {
        if (boundFlag == true)
        {
            outOfBoundsTimer -= 1 * Time.deltaTime; //start counter
            textTime = string.Format("{0:0}", outOfBoundsTimer); //Show out of bounds message
            OutOfBounds.text = "Out of bounds, please return to the game in " + textTime + " seconds or you will be shot";
            OutOfBounds.pixelOffset = new Vector2(0, 180); // ensure text is centred on screen
            OutOfBounds.enabled = true;
            //if timer reaches 0 Destroy object and reset timer
            if (textTime.Equals("0"))
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player");
                playerTransform.SendMessage("outOfBounds");
                outOfBoundsTimer = 5.0f;
            }
        }
    }

    //controls the GUI elements on the screen
    void OnGUI()
    {
        //fades the screen out while the player is out of bounds
        if (boundFlag == true)
        {
            //how long it takes to fade out - set at about 5-6 seconds
            alphaFadeValue += Mathf.Clamp01(Time.deltaTime / 12);
            GUI.color = new Color(0, 0, 0, alphaFadeValue);
            //fader set to null, just place holder
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fader);
        }
        else 
        {
            //reset fader
            alphaFadeValue = 0;
        }
    }

}
