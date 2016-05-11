﻿using UnityEngine;
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
	public int menuScore = 10000;       // stores player accumulated upgradePoints
	public int menuHealth;      // stores player health stat
    public int menuMoveSpeed;   // stores player movement speed
    public int menuReloadSpeed; // stores player reloading speed
	public int menuFireRate;    // stores player firing rate
    public int menuStartingAmmo;// stores player starting paintball/ammo capacity

    // Health Upgrade variables
    public Button upgrade;
	public Image upgradeImg1;
	public Image upgradeImg2;
	public Image upgradeImg3;
	public Image upgradeImg4;
	public Image upgradeImg5;
	//public ArrayList[] tiers;
	public Text upgradePointsTxt;
	public Text upgradeCostTxt;
    public Text incHP;
    public Text notEnoughPointshp;
	public int upgradeCost = 500;
	public int counter;
	//public Image[] upgradeLvl;
    public int percent = 8;

    // Movement Speed Upgrade variables
    public Button upgradems;
    public Image upgradeImg1ms;
    public Image upgradeImg2ms;
    public Image upgradeImg3ms;
    public Image upgradeImg4ms;
    public Image upgradeImg5ms;
    public Text upgradeCostTxtms;
    public Text incMS;
    public Text notEnoughPointsms;
    public int upgradeCostms = 500;
    public float speed = 8.0f;
    public int counterms;

    public Image[] healthImg;
    public bool assignOnce = true;

    // Use this for initialization
    void Start () {
		loadScore();
        upgradePointsTxt.text = menuScore.ToString();
        upgradeCostTxt.text = upgradeCost.ToString ();
        upgradeCostTxtms.text = upgradeCostms.ToString();
		
        incHP.text = "+" + "0HP";
        incMS.text = "+" + "0%";
        notEnoughPointshp.enabled = false;
        notEnoughPointsms.enabled = false;

        //health
        upgradeImg1.enabled = false;
		upgradeImg2.enabled = false;
		upgradeImg3.enabled = false;
		upgradeImg4.enabled = false;
		upgradeImg5.enabled = false;

        //moveSpeed
        upgradeImg1ms.enabled = false;
        upgradeImg2ms.enabled = false;
        upgradeImg3ms.enabled = false;
        upgradeImg4ms.enabled = false;
        upgradeImg5ms.enabled = false;



	}

	// Update is called once per frame
	void Update () {
        // 
        upgradePointsTxt.text = menuScore.ToString();
        upgradeCostTxt.text = upgradeCost.ToString();
        upgradeCostTxtms.text = upgradeCostms.ToString();

		// health 
        if (upgradeImg5.enabled == true)
        {
            upgradeCostTxt.text = "MAX";
        }
        // move speed
        if (upgradeImg5ms.enabled == true) {
            upgradeCostTxtms.text = "MAX";
        }

    }

	// handles the health upgrade system
	public void upgradeHealth() {
        //upgradeImg1.enabled = true;
        //menuScore = menuScore - upgradeCost;
        //Debug.Log (menuScore);
        
		if ((menuScore >= upgradeCost) && (menuScore != 0)) {

			switch (counter) {
			case 0:
                    upgradeImg1.enabled = true;
                    menuScore = menuScore - upgradeCost;
                    //upgradeImg1.enabled = true;
                    //Debug.Log();

                    // add new health %
                    menuHealth = menuHealth + menuHealth / 100 * percent;
				Debug.Log (menuHealth);
                    incHP.text = "+ " + (menuHealth - 100) + "HP";
                    //percent = percent + percent;
                    // increase upgradeCost
                    upgradeCost = upgradeCost + (upgradeCost / 2);
				break;
			case 1:
				upgradeImg2.enabled = true;
				menuScore = menuScore - upgradeCost;
				//upgradeImg1.enabled = true;
				//Debug.Log();

				// add new health %
				menuHealth = menuHealth + (menuHealth / 100 * (percent*2));
				Debug.Log (menuHealth);
                    incHP.text = "+ " + (menuHealth - 100) + "HP";
                    //percent = percent + percent;
                    // increase upgradeCost
                    upgradeCost = upgradeCost + (upgradeCost / 2);
				break;
			case 2:
				upgradeImg3.enabled = true;
				menuScore = menuScore - upgradeCost;
				//upgradeImg1.enabled = true;
				//Debug.Log();

				// add new health %
				menuHealth = menuHealth + (menuHealth / 100 * (percent*3));
				Debug.Log (menuHealth);
                    incHP.text = "+ " + (menuHealth - 100) + "HP";
                    //percent = percent + percent;
                    // increase upgradeCost
                    upgradeCost = upgradeCost + (upgradeCost / 2);
				break;
			case 3:
				upgradeImg4.enabled = true;
                    menuScore = menuScore - upgradeCost;
				//upgradeImg1.enabled = true;
				//Debug.Log();

				// add new health %
				menuHealth = menuHealth + (menuHealth / 100 * (percent*4));
				Debug.Log (menuHealth);
                    incHP.text = "+ " + (menuHealth - 100) + "HP";
                    //percent = percent + percent;
                    // increase upgradeCost
                    upgradeCost = upgradeCost + (upgradeCost / 2);
				break;
			case 4:
				upgradeImg5.enabled = true;
				menuScore = menuScore - upgradeCost;
                    //upgradeImg1.enabled = true;
                    //Debug.Log();

                    // add new health %
                    menuHealth = menuHealth + (menuHealth / 100 * (percent * 5));
				Debug.Log (menuHealth);
                    incHP.text = "+ " + (menuHealth - 100) + "HP";
                    //percent = percent + percent;
                    // increase upgradeCost
                    upgradeCost = upgradeCost + (upgradeCost / 2);
				break;
			default:
                    Debug.Log("Only 5 upgrade tiers!!");
				break;
			}
			// increment counter
			counter++;
            
            for (int i = 0; i < healthImg.Length; i++)
            {
                if (!healthImg[i].enabled && assignOnce )
                {
                    healthImg[i].enabled = true;
                    //healthImg[i].overrideSprite = healthIMG;
                    assignOnce = !assignOnce;
                }
            }
            assignOnce = !assignOnce;


		}

        // handles upgrade point shortages
        if ((menuScore == 0) || (menuScore < upgradeCost) && (upgradeImg5.enabled == false))
        {
            notEnoughPointshp.enabled = true;
        }
        if (menuScore >= upgradeCost || (menuScore >= upgradeCost && upgradeImg5.enabled == true))
        {
            notEnoughPointshp.enabled = false;
            Debug.Log("also works?");
        }

    }


    // handles the movement speed upgrades
    public void upgradeMoveSpeed() {
        if ((menuScore >= upgradeCostms) && (menuScore != 0))
        {
            switch (counterms)
            {
                case 0:
                    upgradeImg1ms.enabled = true;
                    menuScore = menuScore - upgradeCostms;
                    //upgradeImg1.enabled = true;
                    //Debug.Log();

                    // add new speed %
                    menuMoveSpeed = menuMoveSpeed + ((int)speed * 1);
                    Debug.Log(menuMoveSpeed);
                    incMS.text = "+ " + ((int)speed*1) + "%";
                    //percent = percent + percent;
                    // increase upgradeCost
                    upgradeCostms = upgradeCostms + (upgradeCostms / 2);
                    break;
                case 1:
                    upgradeImg2ms.enabled = true;
                    menuScore = menuScore - upgradeCostms;
                    //upgradeImg1.enabled = true;
                    //Debug.Log();

                    // add new speed %
                    menuMoveSpeed = menuMoveSpeed + ((int)speed * 2 / 2);
                    Debug.Log(menuMoveSpeed);
                    incMS.text = "+ " + ((int)speed * 2) + "%";
                    //percent = percent + percent;
                    // increase upgradeCost
                    upgradeCostms = upgradeCostms + (upgradeCostms / 2);
                    break;
                case 2:
                    upgradeImg3ms.enabled = true;
                    menuScore = menuScore - upgradeCostms;
                    //upgradeImg1.enabled = true;
                    //Debug.Log();

                    // add new speed %
                    menuMoveSpeed = menuMoveSpeed + ((int)speed * 3 / 2);
                    Debug.Log(menuMoveSpeed);
                    incMS.text = "+ " + ((int)speed * 3) + "%";
                    //percent = percent + percent;
                    // increase upgradeCost
                    upgradeCostms = upgradeCostms + (upgradeCostms / 2);
                    break;
                case 3:
                    upgradeImg4ms.enabled = true;
                    menuScore = menuScore - upgradeCostms;
                    //upgradeImg1.enabled = true;
                    //Debug.Log();

                    // add new speed %
                    menuMoveSpeed = menuMoveSpeed + ((int)speed * 4 / 2) ;
                    Debug.Log(menuMoveSpeed);
                    incMS.text = "+ " + ((int)speed * 4) + "%";
                    //percent = percent + percent;
                    // increase upgradeCost
                    upgradeCostms = upgradeCostms + (upgradeCostms / 2);
                    break;
                case 4:
                    upgradeImg5ms.enabled = true;
                    menuScore = menuScore - upgradeCostms;
                    //upgradeImg1.enabled = true;
                    //Debug.Log();

                    // add new speed %
                    menuMoveSpeed = menuMoveSpeed + ((int)speed * 5 / 2);
                    Debug.Log(menuMoveSpeed);
                    incMS.text = "+ " + ((int)speed * 5) + "%";
                    //percent = percent + percent;
                    // increase upgradeCost
                    upgradeCostms = upgradeCostms + (upgradeCostms / 2);
                    break;
                default:
                    Debug.Log("Only 5 upgrade tiers!!");
                    break;
            }
            // increment counter
            counterms++;
        }

        // handles upgrade point shortages
        if ((menuScore == 0) || (menuScore < upgradeCostms) && (upgradeImg5ms.enabled == false))
        {
            notEnoughPointsms.enabled = true;
        }
        if (menuScore >= upgradeCostms || (menuScore >= upgradeCostms && upgradeImg5ms.enabled == true))
        {
            notEnoughPointsms.enabled = false;
            Debug.Log("works!");
        }

    }



	public void loadScore()
	{
		if (File.Exists(Application.persistentDataPath + "/playerScore.dat"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerScore.dat", FileMode.Open); //open from this location

			PlayerData data = (PlayerData)bf.Deserialize(file); //instantiate player data

			//load score for menu upgrades
			menuScore = data.finalScore;
			file.Close();

            //healthImg = data.healthLoadIng;
		}
	}

	public void savePlayerData()
	{
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

        //data.healthLoadIng = healthImg;

		bf.Serialize (file, data);
		file.Close();
	}
}
