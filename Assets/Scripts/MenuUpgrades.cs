using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityStandardAssets.Characters;

/*
* Written by: Joshua Hurn
* Last Modified by: Jake Nye on 11/05/2016
*
* This class controls the upgrades purchased by the player
*/
public class MenuUpgrades : MonoBehaviour {
	public int menuScore = 0;       // stores player accumulated upgradePoints
	public int menuHealth = 0;      // stores player health stat
    public int menuMoveSpeed = 0;   // stores player movement speed
    public float menuReloadSpeed = 0; // stores player reloading speed
	public float menuFireRate = 0f;    // stores player firing rate
    public int menuStartingAmmo = 0;// stores player starting paintball/ammo capacity

    // Health Upgrade variables
    public Button upgrade;
	public Image upgradeImg5;
	public Text upgradePointsTxt;
	public Text upgradeCostTxt;
    public Text incHP;
    public Text notEnoughPointshp;
	public int upgradeCost = 500;

    // Movement speed Upgrade variables
    public Button upgradems;
    public Image upgradeImg5ms;
    public Text upgradeCostTxtms;
    public Text incMS;
    public Text notEnoughPointsms;
    public int upgradeCostms = 500;
    public float speed = 8.0f;

    // RapidFire upgrade variables
    public Button upgradeRaidFire;
    public Image upgradeImg5RapidFire;
    public Text upgradeCostTxtRapidFire;
    public Text incRF;
    public Text notEnoughPointsRF;
    public int upgradeCostRF = 500;

    // Reload speed upgrade variables
    public Button upgradeRS;
    public Image upgradeImg5ReloadSpeed;
    public Text upgradeCostTxtReloadSpeed;
    public Text incRS;
    public Text notEnoughPointsRS;
    public int upgradeCostRS = 500;

    // Startin ammo upgrade variables
    public Button upgradeSA;
    public Image upgradeImg5StartAmmo;
    public Text upgradeCostTxtStartAmmo;
    public Text incStartAmmo;
    public Text notEnoughPointsStartAmmo;
    public int upgradeCostStartAmmo = 500;

    //counters
    public int healthCounter;
    public int moveSpeedCounter;
    public int rapidFireCounter;
    public int reloadSpeedCounter;
    public int startAmmoCounter;

    //Images
    public Image[] healthImg;
    public Image[] moveSpeedImg;
    public Image[] rapidFireImg;
    public Image[] reloadImg;
    public Image[] startAmmoImg;

    // flags to apply one image at a time
    public bool healthAssignOnce = true;
    public bool moveSpeedAssignOnce = true;
    public bool rapidFireAssignOnce = true;
    public bool reloadSpeedAssignOnce = true;
    public bool startingAmmoAssignOnce = true;

    // Use this for initialization
    void Start () {
       //deleteScore();
     //if the file exists, load it
     if (File.Exists(Application.persistentDataPath + "/playerScore.dat"))
            loadScore();
		
        incHP.text = "+" + "0 HP";
        incMS.text = "+" + "0%";
        incRF.text = "-" + "0 Seconds";
        incRS.text = "-" + "0 Seconds";
        incStartAmmo.text = "+" + "0 Bullets";

        notEnoughPointshp.enabled = false;
        notEnoughPointsms.enabled = false;
        notEnoughPointsRF.enabled = false;
        notEnoughPointsRS.enabled = false;
        notEnoughPointsStartAmmo.enabled = false;

	}

	// Update is called once per frame
	void Update () {
        //update visual
        upgradePointsTxt.text = menuScore.ToString();
        upgradeCostTxt.text = upgradeCost.ToString();
        upgradeCostTxtms.text = upgradeCostms.ToString();
        upgradeCostTxtRapidFire.text = upgradeCostRF.ToString();
        upgradeCostTxtReloadSpeed.text = upgradeCostRS.ToString();
        upgradeCostTxtStartAmmo.text = upgradeCostStartAmmo.ToString();

        // health 
        if (upgradeImg5.enabled)
            upgradeCostTxt.text = "MAX";
        // move speed
        if (upgradeImg5ms.enabled)
            upgradeCostTxtms.text = "MAX";
        //rapid fire
        if (upgradeImg5RapidFire.enabled)
            upgradeCostTxtRapidFire.text = "Max";
        //reload speed
        if (upgradeImg5ReloadSpeed.enabled)
            upgradeCostTxtReloadSpeed.text = "Max";
        //starting ammo
        if (upgradeImg5StartAmmo.enabled)
            upgradeCostTxtStartAmmo.text = "Max";

        //go through and show health images
        for (int i = 0; i < healthCounter; i++)
        {
            //if this image is not enabled
            if (!healthImg[i].enabled && healthAssignOnce)
            {
                menuScore -= upgradeCost;
                healthImg[i].enabled = true;
                healthAssignOnce = !healthAssignOnce;
                
                incHP.text = "+ " + (menuHealth) + "HP";
                upgradeCost += 200;
                savePlayerData();
            }
        }
        healthAssignOnce = !healthAssignOnce;

        //go through and show move speed images
        for (int i = 0; i < moveSpeedCounter; i++)
        {
            //if this image is not enabled
            if (!moveSpeedImg[i].enabled && moveSpeedAssignOnce)
            {
                menuScore -= upgradeCostms;
                moveSpeedImg[i].enabled = true;
                moveSpeedAssignOnce = !moveSpeedAssignOnce;
                
                incMS.text = "+ " + (menuMoveSpeed) + "%";
                upgradeCostms += 200;
                savePlayerData();
            }
        }
        moveSpeedAssignOnce = !moveSpeedAssignOnce;

        //go through and show rapid fire images
        for (int i = 0; i < rapidFireCounter; i++)
        {
            //if this image is not enabled
            if (!rapidFireImg[i].enabled && rapidFireAssignOnce)
            {
                menuScore -= upgradeCostRF;
                rapidFireImg[i].enabled = true;
                rapidFireAssignOnce = !rapidFireAssignOnce;
                
                incRF.text = "- " + (menuFireRate) + " Seconds";
                upgradeCostRF += 200;
                savePlayerData();
            }
        }
        rapidFireAssignOnce = !rapidFireAssignOnce;

        //go through and show reload speed images
        for (int i = 0; i < reloadSpeedCounter; i++)
        {
            //if this image is not enabled
            if (!reloadImg[i].enabled && reloadSpeedAssignOnce)
            {
                menuScore -= upgradeCostRS;
                reloadImg[i].enabled = true;
                reloadSpeedAssignOnce = !reloadSpeedAssignOnce;
                
                incRS.text = "- " + (menuReloadSpeed) + " Seconds";
                upgradeCostRS += 200;
                savePlayerData();
            }
        }
        reloadSpeedAssignOnce = !reloadSpeedAssignOnce;

        //go through and show starting ammo images
        for (int i = 0; i < startAmmoCounter; i++)
        {
            //if this image is not enabled
            if (!startAmmoImg[i].enabled && startingAmmoAssignOnce)
            {
                menuScore -= upgradeCostStartAmmo;
                startAmmoImg[i].enabled = true;
                startingAmmoAssignOnce = !startingAmmoAssignOnce;
                
                incStartAmmo.text = "+ " + (menuStartingAmmo) + " paintballs";
                upgradeCostStartAmmo += 200;
                savePlayerData();
            }
        }
        startingAmmoAssignOnce = !startingAmmoAssignOnce;

    }

	// handles the health upgrade system
	public void upgradeHealth() {
        //add one to the counter, update will take care of the image showing    
        if ((menuScore >= upgradeCost) && (menuScore != 0))
        {
            healthCounter++;
            menuHealth = menuHealth + 20;
        }

        // handles upgrade point shortages
        if ((menuScore == 0) || (menuScore < upgradeCost) && (upgradeImg5.enabled == false))
        {
            notEnoughPointshp.enabled = true;
        }
    }

    //handles the move speed upgrade system
    public void upgradeMoveSpeed()
    {
        if ((menuScore >= upgradeCostms) && (menuScore != 0))
        {
            moveSpeedCounter++;
            menuMoveSpeed = menuMoveSpeed + ((int)speed * 1);
        }

        // handles upgrade point shortages
        if ((menuScore == 0) || (menuScore < upgradeCostms) && (upgradeImg5ms.enabled == false))
        {
            notEnoughPointsms.enabled = true;
        }
    }

    //handles the raid fire upgrade system
    public void upgradeRapidFire()
    {
        if ((menuScore >= upgradeCostRF) && (menuScore != 0))
        {
            rapidFireCounter++;
            menuFireRate -= 0.1f;
        }

        // handles upgrade point shortages
        if ((menuScore == 0) || (menuScore < upgradeCostRF) && (upgradeImg5RapidFire.enabled == false))
        {
            notEnoughPointsRF.enabled = true;
        }
    }

    //handles the reload speed upgrade system
    public void upgradeReloadSpeed()
    {
        if ((menuScore >= upgradeCostRS) && (menuScore != 0))
        {
            reloadSpeedCounter++;
            menuReloadSpeed -= 0.1f;
        }

        // handles upgrade point shortages
        if ((menuScore == 0) || (menuScore < upgradeCostRS) && (upgradeImg5ReloadSpeed.enabled == false))
        {
            notEnoughPointsRS.enabled = true;
        }
    }

    //handles the starting ammo upgrade system
    public void upgradeStartAmmo()
    {
        if ((menuScore >= upgradeCostStartAmmo) && (menuScore != 0))
        {
            startAmmoCounter++;
            menuStartingAmmo += 5;
        }

        // handles upgrade point shortages
        if ((menuScore == 0) || (menuScore < upgradeCostStartAmmo) && (upgradeImg5StartAmmo.enabled == false))
        {
            notEnoughPointsStartAmmo.enabled = true;
        }
    }


    //Load what the player currently has and show this
	public void loadScore()
	{
		if (File.Exists(Application.persistentDataPath + "/playerScore.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerScore.dat", FileMode.Open); //open from this location

			PlayerData data = (PlayerData)bf.Deserialize(file); //instantiate player data

            //load score for menu upgrades
            menuScore = data.finalScore;
            menuHealth = data.playerHealthMod;
            menuMoveSpeed = data.playerMoveSpeedMod;
            menuReloadSpeed = data.playerReloadSpeedMod;
            menuStartingAmmo = data.playerStartingAmmoMod;

            healthCounter = data.counterHealth;
            moveSpeedCounter = data.counterMoveSpeed;
            rapidFireCounter = data.counterRapidFire;
            reloadSpeedCounter = data.counterReloadSpeed;
            startAmmoCounter = data.counterStartingAmmo;

			file.Close();
		}
	}

    //saves the players upgrades for the player to load in game
	public void savePlayerData()
	{
        deleteScore();
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create(Application.persistentDataPath + "/playerScore.dat"); //save to this location

		PlayerData data = new PlayerData(); //instantiate player data

        //save values to player data for the player
        data.finalScore = menuScore;
        data.playerHealthMod = menuHealth;
        data.playerMoveSpeedMod = menuMoveSpeed;
        data.playerReloadSpeedMod = menuReloadSpeed;
        data.playerFireRateMod = menuReloadSpeed;
        data.playerStartingAmmoMod = menuStartingAmmo;

        data.counterHealth = healthCounter;
        data.counterMoveSpeed = moveSpeedCounter;
        data.counterRapidFire = rapidFireCounter;
        data.counterReloadSpeed = rapidFireCounter;
        data.counterStartingAmmo = startAmmoCounter;

        bf.Serialize(file, data);
		file.Close();
	}

    //Developers method, used only when wanting to reset the score of the person, may be used for expansion in the game later on
    public void deleteScore()
    {
        File.Delete(Application.persistentDataPath + "/playerScore.dat"); //delete from this location
    }
}