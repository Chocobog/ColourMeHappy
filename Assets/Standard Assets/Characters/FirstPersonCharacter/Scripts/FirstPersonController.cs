using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
/*
* @Written by: Unity
* @Modified by: Joshua Hurn, Jake Nye
* @Last Modified: 01/05/2016
*
* This class controls the movement and shooting of the FPS object
* contains HUD and game over controls due to being single player game
*/
namespace UnityStandardAssets.Characters.FirstPerson
{

    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        public bool canMove; // if the player can move or not
        
        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        
        public GameObject gun; // gun of the player
        public GameObject bullet; // bullet for player
        public GameObject bulletSpawnPoint; // spawn location for player

        //Bullet shooting rate
        public float shootRate = 0.5f;
        protected float elapsedTime;

        //Spawning after death
        public Transform[] spawnPositions = new Transform[6];
        private Boolean respawn;

        public int totalHealth = 100;
        public int health = 100; // player health
        public int pickupHealth = 10; //when health picked up
        public bool damaged = false;
        public float gameTimeLeft = 600; //10 minute time limit

        public int pickupAmmo = 5; //when ammo picked up
        public int clipLimit = 10; //When reloading paintballs
        public int playerClip = 10; //ammo clip of player
        public int playerTotalAmmo = 50; //total ammo of player
        public int scoreAlly; //score of ally team
        public int scoreEnemy; //score of enemy team
        public int scoreLimit = 3;
        public int playerScore;
        public GameObject[] perksAvailable; //perks player can activate
        public string enemyFlagLocation = "At Base - "; //location of enemy flag

        //Perk images
        public Sprite Nimble;
        public Sprite rapid;
        public Sprite Rejuv;
        public Sprite shield;
        public Sprite radar;
        public bool assignNext;

        public List<Sprite> listOfPerks; //All perks in game
        public List<String> currentPerks; //Perks the player currently has

        //Perk effects
        public GameObject shieldEffect;
        public GameObject nimbleEffect;
        public GameObject rejuvenationEffect;
        public GameObject rapidFireEffect;
        public GameObject respawnEffect;

        public bool invincible = false; //Says if the player can take damage or not

        //Perk countdown timers
        private float rejuvCountdown;
        private float nimbleCountdown;
        private float shieldCountdown;
        private float radarCountdown;
        private float rapidCountdown;
        public float perkResetTimer = 30f;
        public int tempPerk = 0;

        //Gate to allow times for perks
        private bool rejuvCounter = false;
        private bool nimbleCounter = false;
        private bool shieldCounter = false;
        private bool rapidCounter = false;
        private bool radarCounter = false;

        //HUD
        public Text healthTxt;
        public Slider healthSlider;
        public Slider ammoSlider;
        public Text gameTimeLeftTxt;
        public Text playerAmmoTxt;
        public Text scoreAllyTxt;
        public Text perksAvailableTxt;
        public Image[] perksAvailableImg;
        public Image damageImage;
        public Text enemyFlagLocationTxt;
        public InputField inputField;
        public Text inputFieldText;

        //Flag Capture
        public Transform flag;
        public Transform flagMist;
        public Transform flagHolder;
        public string opposingFlag;
        public string allyFlag;

        //damage effects
        public float flashSpeed = 5f;
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f);

        //death timer
        private float respawnCountdown = 10f; // time until player can respawn
        private string textTime; // respawn timer passed to respawnInfo
        public GUIText respawnInfo; // text on screen to tell player time left until respawn

        //banner display
        public Image victory;
        public Image defeat;
        public Image overTime;

        public float overtimeCountdown = 3f;
        private string overTimeText;
        public bool isOvertime = false;
        private float alphaFadeValue = 0;
        public Texture fader; //place holder texture

        //Reload animation
        public Transform back;
        public Transform forward;
        public Transform clip;
        public bool unloading;
        public bool reloading;
        public float delayReset = 1.5f;
        public float delay = 1.5f;
        public string delayTextTime;

        //exit game interface
        public Canvas pauseMenu;
        public Canvas quitOptions;

        //Loading screen on exit
        private AsyncOperation async;
        public Image[] loadingScreen;
        public Text[] tips;
        public Slider progressBar;
        public Image progressBackground;
        public Image progressFill;

        // Use this for initialization
        private void Start()
        {
            //Cursor.visible = false;
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
            respawn = false;
            respawnInfo.enabled = false;
            canMove = true;
            unloading = false;
            reloading = false;

            opposingFlag = "RedFlag";
            allyFlag = "BlueFlag";

            //Update perks that can be used in game
            listOfPerks.Add(Nimble);
            listOfPerks.Add(Rejuv);
            listOfPerks.Add(rapid);
            listOfPerks.Add(shield);
            listOfPerks.Add(radar);

            //Set to max perk time
            rejuvCountdown = perkResetTimer;
            nimbleCountdown = perkResetTimer;
            shieldCountdown = perkResetTimer;
            radarCountdown = perkResetTimer;
            rapidCountdown = perkResetTimer;

            /////TESTING////////
            addPerkGUI(radar);
        }

        // Update is called once per frame
        private void Update()
        {
            //set game time presentation
            int minutes = Mathf.FloorToInt(gameTimeLeft / 60F);
            int seconds = Mathf.FloorToInt(gameTimeLeft - minutes * 60);
            string displayTime = string.Format("{0:0}:{1:00}", minutes, seconds);

            //update HUD elements
            healthTxt.text = health.ToString();
            healthSlider.value = health;
            ammoSlider.value = playerClip;
            gameTimeLeftTxt.text = "Time left: " + displayTime;
            playerAmmoTxt.text = playerClip.ToString() + " / " + playerTotalAmmo.ToString();
            scoreAllyTxt.text = scoreAlly.ToString();
            enemyFlagLocationTxt.text = enemyFlagLocation;

            gameTimeLeft -= Time.deltaTime;
            //when time is up or if score limit is reached end the game
            if (displayTime.Equals("0:00") || scoreAlly == scoreLimit || scoreEnemy == scoreLimit)
            {
                if (scoreAlly != scoreEnemy)
                    gameOver(); //end the game
                else if (isOvertime == false) //first time through, show overtime message
                    overTime.enabled = true; //currently drawing and going into overtime
                if (overTime.enabled == true)
                {
                    overtimeCountdown -= 1 * Time.deltaTime; //start counter
                    overTimeText = string.Format("{0:0}", overtimeCountdown); //easier text for comparison
                    if (overTimeText.Equals("0"))
                    {
                        //stop showing overtime banner
                        overTime.enabled = false;
                        overtimeCountdown = 60f;
                        isOvertime = true;
                    }
                }
                gameTimeLeft = 0;
            }

            //Open pause menu on ESC
            if (Input.GetKeyDown(KeyCode.Escape))
                openPauseMenu();

            //Use your perk when C is pressed
            if (Input.GetKeyDown(KeyCode.C) && currentPerks.Count != 0)
                usePerk();

            //Open cheat code window when enter pressed
            if (Input.GetKeyDown(KeyCode.Return))
            {
                inputField.gameObject.SetActive(true);
                inputField.ActivateInputField(); //set focus to input field
                canMove = false; //stop moving while entering code
            }   

            //enter overtime due to draw
            if (isOvertime)
            {
                overtimeCountdown -= 1 * Time.deltaTime; //start counter
                overTimeText = string.Format("{0:0}", overtimeCountdown); //easier text for comparison
                //if overtime ends
                if (overTimeText.Equals("0"))
                {
                    isOvertime = false;
                    gameOver();
                }
                //if in overtime and any team scores, end game
                if(scoreAlly > scoreEnemy || scoreEnemy > scoreAlly)
                {
                    overTimeText.Equals("0");
                }
            }

            //player is damaged from bullet
            if (damaged) {
                damageImage.color = flashColour;
            } else
            {
                damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }
            damaged = false;

            //rejuv activated
            if (rejuvCounter)
            {
                rejuvCountdown -= 1 * Time.deltaTime; //start countdown
                if ((int)rejuvCountdown != 0)
                {
                    if ((int)rejuvCountdown %5 == 0 && health < totalHealth && (int)rejuvCountdown != tempPerk)
                    {
                        tempPerk = (int)rejuvCountdown;
                    //within 5 HP radius, give full health
                    if (health >= totalHealth - 10)
                        health = totalHealth;
                    //give 5 HP
                    else
                        health += 10;
                    }
                }
                else
                {
                    //perk ended, reset back to default
                    rejuvCounter = false;
                    rejuvCountdown = perkResetTimer;
                    rejuvenationEffect.SetActive(false);
                }
            }

            //nimble activated
            if (nimbleCounter)
            {
                nimbleCountdown -= 1 * Time.deltaTime; //start countdown
                if ((int)nimbleCountdown != 0)
                    m_RunSpeed *= 2; //double the run speed
                else
                {
                    //perk ended, reset back to default
                    m_RunSpeed /= 2;
                    nimbleCounter = false;
                    nimbleCountdown = perkResetTimer;
                    nimbleEffect.SetActive(false);
                }
            }

            //rapid fire activated
            if (rapidCounter)
            {
                rapidCountdown -= 1 * Time.deltaTime; //start countdown
                if ((int)rapidCountdown != 0)
                    shootRate /= 2; //half time needed to shoot
                else
                {
                    //perk ended, reset back to default
                    shootRate *= 2;
                    rapidCounter = false;
                    rapidCountdown = perkResetTimer;
                    rapidFireEffect.SetActive(false);
                }
            }

            //shield activated 
            if (shieldCounter)
            {
                shieldCountdown -= 1 * Time.deltaTime; //start countdown
                if ((int)shieldCountdown != 0)
                    invincible = true; //player cannot be hurt
                else
                {
                    //perk ended, reset back to default
                    invincible = false;
                    shieldCounter = false;
                    shieldCountdown = perkResetTimer;
                    shieldEffect.SetActive(false);
                }

            }

            //radar shows enemies on the minimap
            if (radarCounter)
            {
                radarCountdown -= 1 * Time.deltaTime; //start countdown
                if ((int)radarCountdown != 0)
                {
                    //Instantiated here incase enemy is respawned. This will then show location of that new enemy
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    //filter through and activate all enemy markers
                    for(int i = 0; i < enemies.Length; i++)
                    {
                        enemies[i].SendMessage("markerActivate", true);
                    }
                }
                else
                {
                    //Perk ended, reset back to default
                    radarCounter = false;
                    radarCountdown = perkResetTimer;
                    //Instantiated here incase enemy is respawned. This will then show location of that new enemy
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    //filter through and deactivate all enemy markers
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        enemies[i].SendMessage("markerDeactivate", false);
                    }
                }
            }

            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            //Player shooting via mouse click
            if (Input.GetButton("Fire1"))
            {
                if (elapsedTime >= shootRate)
                {
                    //Reset the time
                    elapsedTime = 0.0f;

                    //Shoot bullet if ammo is available and not unloading/reloading
                    if ((bulletSpawnPoint) && (bullet) && playerClip > 0 || playerTotalAmmo > 0 && unloading == false && reloading == false)
                    {
                        //update bullet count
                        playerClip--;
                        GameObject spawnOrigin = (GameObject)Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
                        spawnOrigin.SendMessage("spawnOrigin", gameObject);
                        //Clip is empty and player needs to reload
                        if (playerClip == 0)
                        {
                            if (playerTotalAmmo == 0) { /*do nothing*/ }
                            //update player clip and minus total ammo
                            else if (playerTotalAmmo > clipLimit)
                            {
                                unloading = true;
                                playerClip += clipLimit;
                                playerTotalAmmo -= playerClip;
                            }
                            //if total ammo is less or equal to standard clip amount, put all ammo in last clip
                            else if (playerTotalAmmo <= clipLimit)
                            {
                                playerClip += playerTotalAmmo;
                                playerTotalAmmo = 0;
                            }
                        }
                    }
                    else
                    {
                        //show no ammo to player
                    }
                }
            }
            elapsedTime += Time.deltaTime;

            float percentageComplete = Time.time / 1f;
            Vector3 temp = clip.transform.position; //temp holder 
            //Animation for unloading ammoTube
            if (unloading == true)
            {
                delay -= 1 * Time.deltaTime; //start counter
                delayTextTime = string.Format("{0:0}", delay); //easier text for comparison
                temp.z = back.transform.position.z;
                clip.transform.position = Vector3.Lerp(temp, back.transform.position, percentageComplete / 25f); //slowly unload
                if (delayTextTime.Equals("0"))
                {
                    unloading = false;
                    reloading = true;
                    delay = delayReset;
                }
            }
            //Animation for reloading ammoTube
            if (reloading)
            {
                delay -= 1 * Time.deltaTime; //start counter
                delayTextTime = string.Format("{0:0}", delay); //easier text for comparison
                temp.z = forward.transform.position.z;
                clip.transform.position = Vector3.Lerp(temp, forward.transform.position, percentageComplete / 25f); //slowly reload
                if (delayTextTime.Equals("0"))
                {
                    unloading = false;
                    reloading = false;
                    delay = delayReset;
                }
            }

            // Update the time
            

            //check if player is alive or dead
            if (health <= 0)
                respawn = true;
            else
                respawn = false;
                
            if (respawn)
            {
                canMove = false;
                respawnEffect.SetActive(true); //effect for respawning
                //move player back to 1st spawn position
                if (transform.position != spawnPositions[0].position)
                    transform.position = spawnPositions[0].position;
                respawnCountdown -= 1 * Time.deltaTime; //start counter
                textTime = string.Format("{0:0}", respawnCountdown); //Show respawn message
                respawnInfo.text = "Respawn in " + textTime;
                respawnInfo.pixelOffset = new Vector2(0, 180); // ensure text is centred on screen
                respawnInfo.enabled = true;
                //if respawn time is up respawn player
                if (textTime.Equals("0"))
                {
                    canMove = true; //allow movement
                    health = 100;
                    respawnCountdown = 10.0f; //reset
                    respawnInfo.enabled = false; //turn off GUI element
                    respawnEffect.SetActive(false); //effect for respawning
                }                    
            }
        }

        /*
        *When obtained perk compare against list to update GUI element
        *@String s: name passed from onTriggerEnter with orb
        */
        public void getPerk(String s)
        {
            for (int i = 0; i < listOfPerks.Count; i++) {
                if (s.Equals(listOfPerks[i].name))
                {
                    addPerkGUI(listOfPerks[i]);
                    assignNext = false;
                }
            }
        }

        /* 
        * When perk effect added to player, adds it to the next available spot on the HUD
        * to show the player what perk they have
        * @Sprite s: sprite effect obtained by the player
        */
        private void addPerkGUI(Sprite s)
        {
            //loop through images to find next available image
            for (int i = 0; i < perksAvailableImg.Length; i++)
            {
                //Add perk image to next available image slot
                if (!perksAvailableImg[i].enabled == true && assignNext == false)
                {
                    perksAvailableImg[i].overrideSprite = s;
                    perksAvailableImg[i].enabled = true;
                    assignNext = true;
                    currentPerks.Add(s.name);
                }
            }
        }

        //When player uses their perk
        public void usePerk()
        {
            //Grab first perk in the list
            switch (currentPerks[0])
            {
                case "Rejuv":
                    rejuvenationEffect.SetActive(true);
                    UpdatePerkList();
                    rejuvCounter = true;
                    break;
                case "Nimble":
                    nimbleEffect.SetActive(true);
                    UpdatePerkList();
                    nimbleCounter = true;
                    break;
                case "rapid":
                    rapidFireEffect.SetActive(true);
                    UpdatePerkList();
                    rapidCounter = true;
                    break;
                case "radar":
                    UpdatePerkList();
                    radarCounter = true;
                    break;
                case "shield":
                    shieldEffect.SetActive(true);
                    UpdatePerkList();
                    shieldCounter = true;
                    break;
                default:
                    Debug.Log(currentPerks[0]);
                    break;
                
            }
        }

        //Removes perk just used and updates list, calls method to update GUI
        private void UpdatePerkList() {
            //Remove the perk after using it
            currentPerks.RemoveAt(0);
            //Update the list
            for (int i = 0; i < currentPerks.Count; i++) {
                if(currentPerks[i] == null)
                {
                    currentPerks.Insert(i, currentPerks[i+1]);
                    currentPerks.RemoveAt(i+1);   
                } 
            }
            UpdatePerkGUI();
        }

        //Updates the GUI perk elements on the screen
        private void UpdatePerkGUI()
        {
            //Reset images on screen
            for (int i = 0; i < perksAvailableImg.Length; i++)
            {
                perksAvailableImg[i].enabled = false;
            }

            //Update images on screen from current list
            for (int j = 0; j < currentPerks.Count; j++)
            {
                for (int k = 0; k < listOfPerks.Count; k++)
                {
                    if (currentPerks[j].Equals(listOfPerks[k].name))
                    {
                        Sprite s = listOfPerks[k];
                        perksAvailableImg[j].overrideSprite = s;
                        perksAvailableImg[j].enabled = true;
                    }
                }
            }
        }

        //When time limit reaches 0 the game will end
        public void gameOver()
        {
            overtimeCountdown = 0;
            //disable player input
            //disable enemy input
            if (scoreAlly > scoreEnemy)
            {
                //you win
                victory.enabled = true;
            }
            else
            {
                //you lose
                defeat.enabled = true;
            }
            //open menu saying play again or main menu
        }

        //Passed from the bullet script to inflict damage on the player
        public void takeDamage(int damage)
        {
            //shield is not active then allow damage to come to the player
            if (!invincible)
            {
                damaged = true;
                health -= damage;
            }
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        //Currently empty method as scores are not given to the enemy
        public void defeatedBy(string s) {}

        private void FixedUpdate()
        {
            if (canMove)
            {
                float speed;
                GetInput(out speed);
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

                // get a normal for the surface that is being touched to move along it
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                   m_CharacterController.height / 2f);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;


                if (m_CharacterController.isGrounded)
                {
                    m_MoveDir.y = -m_StickToGroundForce;

                    if (m_Jump)
                    {
                        m_MoveDir.y = m_JumpSpeed;
                        PlayJumpSound();
                        m_Jump = false;
                        m_Jumping = true;
                    }
                }
                else
                {
                    m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                }
                m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

                ProgressStepCycle(speed);
                UpdateCameraPosition(speed);
            }
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
           
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);
                // normalize input if it exceeds 1 in combined length:
                if (m_Input.sqrMagnitude > 1)
                {
                    m_Input.Normalize();
                }

                // handle speed change to give an fov kick
                // only if the player is going to a run, is running and the fovkick is to be used
                if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
                {
                    StopAllCoroutines();
                    StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
                }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }

        //When player collides with an object
        public void OnTriggerEnter(Collider c)
        {
            //if this is not the players team flag then take the flag
            if (c.gameObject.tag == opposingFlag)
            {
                //Add flag to player 
                flag = c.transform;
                flag.transform.parent = transform;
                flag.transform.position = flagHolder.transform.position;
                //update location of flag
                enemyFlagLocation = "Taken - ";

                //Add mist to player
                flagMist = c.transform;
                flagMist.transform.parent = transform;
                flagMist.transform.position = flagHolder.transform.position;
                
                
            }

            //if player returns to flag with enemy flag
            if (c.gameObject.tag == allyFlag && flag != null)
            {
                //Destroy the flag gameObject
                Destroy(flag.gameObject);
                //Destroy(flagMist.gameObject);
                scoreAlly++;
                //update location of flag
                enemyFlagLocation = "At Base -";
            }
        }

        //when player picks up ammo
        public void ammoPickup (int i)
        {
            playerTotalAmmo += i;
            //reload if this is the only ammo the player has
            if (playerClip == 0)
                playerClip = 0;
                //reload();
        }

        //When player picks up health
        public void healthPickup (int i)
        {
            //if health is not max
            if (health != totalHealth)
            {
                if (health + i > totalHealth)
                    health = totalHealth;
                else
                    health += i;
            }
        }

        //controls the GUI elements on the screen
        void OnGUI()
        {
            //fades the screen out while the player is in overtime mode
            if (isOvertime == true)
            {
                Debug.Log("running in overtime");
                //how long it takes to fade out - set at about 1 minute
                alphaFadeValue += Mathf.Clamp01(Time.deltaTime / (overtimeCountdown * 10) * 1);
                GUI.color = new Color(0, 0, 0, alphaFadeValue);
                //fader set to null, just place holder
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fader);
            }
            else
            {
                //reset fader
                alphaFadeValue = 0;
            }
        }

        //Opens pause menu when player hits ESC
        public void openPauseMenu()
        {
            pauseMenu.enabled = true;
        }

        //Load the main menu
        public void exitToMainMenu()
        {
            StartCoroutine(LoadScreen("MainMenu"));
        }

        //return back to the game - resume selected on pause menu
        public void backToGame() {
            pauseMenu.enabled = false;
        }

        //return back to pause menu - no selected on quit menu
        public void returnToPause()
        {
            quitOptions.enabled = false;
            pauseMenu.enabled = true;
        }

        //Open quit menu to confirm exit
        public void confirmExit()
        {
            pauseMenu.enabled = false;
            quitOptions.enabled = true;
        }

        /*
        * This function gets a string from the player input field and compares it to some set string.
        * These strings are cheat codes and are used souly for testing purposes.
        * @String s: string that is passed from the input field
        */
        public void cheatCodes(string s)
        {
            string code = inputFieldText.text;
            canMove = true; //after done entering cheat allow player to move again
            inputField.gameObject.SetActive(false);
            if (code.Equals("Score"))
                playerScore = 1000;
        }

        //show loading screen while menu loads in the backgrund
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
            //Load mainmenu in background while loading screen is shown
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
}