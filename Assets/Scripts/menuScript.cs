using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


/*
 * @Written by: Jake Nye
 * @Date modified: 11/04/2016
 *
 * This class instantiates all canvas / button elements so that they may be used in the functions.
 * This class handles all of the menu functionality. This includes the following: starting the game, opening up the game options,
 * player & weapon upgrades, navigating through the menu interface, viewing the credits and of course exiting the game.
 *
*/
public class menuScript : MonoBehaviour {

    // instantiating all <Canvas> && <Button> elements that are to be used as connectors to the methods coded in this class.
    public Canvas startMenu;
    public Canvas quitMenu;
    public Canvas optionsMenu;
    public Canvas upgradesMenu;
    public Button start;
    public Button options;
    public Button upgrades;
    public Button backToStart;
    public Button exit;



	// Use this for initialization
	void Start () {
        quitMenu = quitMenu.GetComponent<Canvas>();
        start = start.GetComponent<Button>();
        exit = exit.GetComponent<Button>();
        quitMenu.enabled = false;
        optionsMenu.enabled = false;
        upgradesMenu.enabled = false;
        
	}

    // disables the startmenu items enables the upgradeMenu items
    public void upgradesPress() {
        startMenu.enabled = false;
        upgradesMenu.enabled = true;
    }

    // shows options menu item list
    public void optionsPress() {
        optionsMenu.enabled = true;
        quitMenu.enabled = false;
        startMenu.enabled = false;
        upgradesMenu.enabled = false;

    }

    // disables the options menu and enables the start menu
    public void backToStartMenu() {
        optionsMenu.enabled = false;
        startMenu.enabled = true;
        upgradesMenu.enabled = false;
    }

    // handles the Exit btn click
    public void ExitPress() {
        quitMenu.enabled = true;
        start.enabled = false;
        exit.enabled = false;

    }

    // exits the exit prompt
    public void noPress() {
        quitMenu.enabled = false;
        start.enabled = true;
        exit.enabled = true;
    }

    // starts the game
    public void startLevel() {
        SceneManager.LoadScene("MainLevel");
    }

    // exits the game
    public void exitGame() {
        Application.Quit();
    }


    //
	// Update is called once per frame
	void Update () {
	    
	}
}
