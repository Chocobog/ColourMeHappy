using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


/*
 * @Written by: Jake Nye
 * @Modified by: Joshua Hurn
 * @Date modified: 30/04/2016
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
	public Canvas creditsPage;
	public Canvas howToPlayPage;

	public Button how2play;
	public Button credits;
    public Button start;
    public Button options;
    public Button upgrades;
    public Button backToStart;
    public Button exit;

    //Loading screen
    private AsyncOperation async;
    public Image[] loadingScreen;
    public Text[] tips;
    public Slider progressBar;
    public Image progressBackground;
    public Image progressFill;

    public bool tutorialPlayed; //tutorial checker
    public Canvas playTutorialPrompt; // canvas to start tut or first play

    // Use this for initialization
    void Start () {
        loadPlayerData(); //load data 
        quitMenu = quitMenu.GetComponent<Canvas>();
        start = start.GetComponent<Button>();
        exit = exit.GetComponent<Button>();
        quitMenu.enabled = false;
        optionsMenu.enabled = false;
        upgradesMenu.enabled = false;
		creditsPage.enabled = false;
        howToPlayPage.enabled = false;
        progressBackground.enabled = false;
        progressFill.enabled = false;


        //shooting.enabled = false;

	}

    // handles the displaying and vertical movement of the credits canvas
    public void rollCredits() {
        startMenu.enabled = false;
		creditsPage.enabled = true;
	}

	// disables the startmenu items enables the howToPlay menu items
	public void howtoplay() {
		howToPlayPage.enabled = true;
		startMenu.enabled = false;
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
		creditsPage.enabled = false;
		howToPlayPage.enabled = false;
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

    // Load the first level
    public void startLevel() {
        if (tutorialPlayed)
            StartCoroutine(LoadScreen("MainLevel"));
        else
            playTutorialPrompt.enabled = true;
    }

    //function that handles tutorial play
    public void playTutorial()
    {
        tutorialPlayed = true;
        savePlayerData();
        StartCoroutine(LoadScreen("Tutorial"));
    }

    // handles the tutorial prompting
    public void backToMenu()
    {
        playTutorialPrompt.enabled = false;
    }

    //ignore the tutorial and play the game
    public void ignoreTutorial()
    {
        tutorialPlayed = true;
        savePlayerData();
        StartCoroutine(LoadScreen("MainLevel"));
    }

    /*
    * show loading screen while level loads in the backgrund
    * @String level: level to load
    */
    IEnumerator LoadScreen(string level)
    {
        //Get random load screen and tip
        int Loadindex = Random.Range(0, loadingScreen.Length);
        int tipIndex = Random.Range(0, tips.Length);
        //show loading screen and tip
        loadingScreen[Loadindex].enabled = true;
        tips[tipIndex].enabled = true;
        progressBackground.enabled = true;
        progressFill.enabled = true;
        //Load main level in background while loading screen is shown
        async = SceneManager.LoadSceneAsync(level);
        //Progress bar while level is being loaded
        while (!async.isDone)
        {
            //Update progress bar
            progressBar.value = (int)(async.progress * 100);
            yield return null;
        }
    }

    // loads the player data of the player from previous games
    public void loadPlayerData()
    {
        if (File.Exists(Application.persistentDataPath + "/tutorial.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/tutorial.dat", FileMode.Open); //open from this location

            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            //load values
            tutorialPlayed = data.playerTutorialCompleted;
        }
    }

    //saves the players upgrades for the player to load in game
    public void savePlayerData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (!File.Exists(Application.persistentDataPath + "/tutorial.dat"))
        {
            file = File.Create(Application.persistentDataPath + "/tutorial.dat");
        }//save to this location
        else
        {
            file = File.OpenWrite(Application.persistentDataPath + "/tutorial.dat");
        }

        PlayerData data = new PlayerData(); //instantiate player data

        //save values to player data for the player
        data.playerTutorialCompleted = tutorialPlayed;

        bf.Serialize(file, data);
        file.Close();
    }

    // exits the game
    public void exitGame()
    {
        Application.Quit();
    }

}
