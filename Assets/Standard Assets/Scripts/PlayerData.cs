using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
/*
* Written by: Joshua Hurn
* Last Modified: Jake Nye 04/05/2016
*
* This class saves the players data and allows the upgrades script to give modifications to the player,
 * also includes the variable that triggers the tutorial on select of 'Play'.
*/
[Serializable]
public class PlayerData {
    //save player data
    public int finalScore;
    public int playerHealthMod;
    public int playerMoveSpeedMod;
	public float playerReloadSpeedMod = 0.0f;
    public float playerFireRateMod = 0.0f;
    public int playerStartingAmmoMod;
	public bool playerTutorialCompleted = false;

    //save where the counter is for the images
    public int counterHealth;
    public int counterMoveSpeed;
    public int counterRapidFire;
    public int counterReloadSpeed;
    public int counterStartingAmmo;
}
