using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
/*
 * This class enables the user to load the main menu scene on collision with the final checkpoint
 * after getting so far through the tutorial walkthrough.
 *
*/
public class finalCheckPoint : MonoBehaviour {

    //last canvaas used as a prerequisite to continue to the main menu
    public Canvas tutFinished;

    //Loading screen
    private AsyncOperation async;
    public Image[] loadingScreen;
    public Text[] tips;
    public Slider progressBar;
    public Image progressBackground;
    public Image progressFill;

    // allows us to on collision with the checkpoint to register the player tag and 
    // load the main menu scene.
    public void OnTriggerEnter(Collider col) {
        if (col.gameObject.tag == "Player" && tutFinished.enabled == true) {

            StartCoroutine(LoadScreen("MainMenu"));

        }
    }

    // Allows us to start the coroutine of loading images and tips to be used
    // in the loading process of the main menu
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


}
