using UnityEngine;
using System.Collections;

/*
* @Written by: Joshua Hurn
* @Last Modified: 13/04/2016
*
* This class controls the music playing in the game. It will randomly choose
* audio after the previous song has completed.
*/

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour {

    //Set how many songs to be played
    public AudioClip[] AudioClips = new AudioClip[2];


    void Update()
    {
        //GetComponent<AudioSource>().loop = true;
        StartCoroutine(playGameMusic());
    }

    //Play music in main level
    IEnumerator playGameMusic()
    {
        //If not playing choose new song randomly
        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().clip = AudioClips[Random.Range(0, AudioClips.Length)];
            GetComponent<AudioSource>().Play();
        }
         yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);
        
    }
}
