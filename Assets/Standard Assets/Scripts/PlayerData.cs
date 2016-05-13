using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
/*
* Written by: Joshua Hurn
* Last Modified: 04/05/2016
*
* This class saves the players data and allows the upgrades script to give modifications to the player
*/
[Serializable]
public class PlayerData {
    //save player data
    public int finalScore;
    public int playerHealthMod;
    public int playerMoveSpeedMod;
    public float playerReloadSpeedMod;
    public float playerFireRateMod;
    public int playerStartingAmmoMod;

    //save where the counter is for the images
    public int counterHealth;
    public int counterMoveSpeed;
    public int counterRapidFire;
    public int counterReloadSpeed;
    public int counterStartingAmmo;
}
