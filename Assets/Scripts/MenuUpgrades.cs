using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityStandardAssets.Characters;

/*
* Written by: Joshua Hurn
* Last Modified: 04/05/2016
*
* This class controls the upgrades purchased by the player
*/
public class MenuUpgrades : MonoBehaviour {

    public int menuScore;
    public int menuHealth;
    public int menuMoveSpeed;
    public int menuReloadSpeed;
    public int menuFireRate;
    public int menuStartingAmmo;

    // Use this for initialization
    void Start () {
        loadScore();
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void loadScore()
    {
        if (File.Exists(Application.persistentDataPath + "/playerScore.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerScore.dat", FileMode.Open); //open from this location

            PlayerData data = (PlayerData)bf.Deserialize(file);

            //load score for menu upgrades
            menuScore = data.finalScore;
            file.Close();
        }
    }

    public void savePlayerData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerScore.dat"); //save to this location

        PlayerData data = new PlayerData();

        //save values to player data for the player
        data.finalScore = menuScore;
        data.playerHealthMod = menuHealth;
        data.playerMoveSpeedMod = menuMoveSpeed;
        data.playerReloadSpeedMod = menuReloadSpeed;
        data.playerFireRateMod = menuReloadSpeed;
        data.playerStartingAmmoMod = menuStartingAmmo;

        bf.Serialize(file, data);
        file.Close();
    }
}
