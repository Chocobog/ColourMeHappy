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
    public int finalScore;
    public int playerHealthMod;
    public int playerMoveSpeedMod;
    public int playerReloadSpeedMod;
    public int playerFireRateMod;
    public int playerStartingAmmoMod;

    //public Image[] healthLoadIng;
}
