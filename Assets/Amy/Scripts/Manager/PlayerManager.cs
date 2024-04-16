using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;

/* Copyright 2021 Jennifer Haden */
namespace Amy
{
    public enum PlayableCharacter
    {
        Amy,
        Cream,
        None
    }

    [System.Flags]
    public enum PlayerStatusFX
    {
        None = 0,
        Relaxed = 1,
        Scared = 2,
        Tired = 4,
        Dirty = 8,
        Horny = 16,
        Sick = 32,
        GoodFood = 64
    }

    [System.Serializable]
    public class PlayerStatus
    {
        public float currentHealth = 25.0f;
        public float maxHealth = 25.0f;
        public float currentMood = 25.0f;
        public float maxMood = 25.0f;
        public float maxStamina = 25.0f;
        public float currentStamina = 25.0f;

        public float speedBonus = 0.0f;

        public float jumpTimeBonus = 0.0f;
        public float jumpPowerBonus = 0.0f;

        public float lungCapacity = 20.0f;
        public float dirtiness = 0.0f;

        public int statusFX;
        public int currentVibes;
        public float scaredTimeLeft = 0.0f;
        public float goodFoodTimeLeft = 0.0f;
        public float sickTimeLeft = 0.0f;

        public float timeSpentResting = 0.0f;

        public PlayerStatus makeCopy()
        {
            PlayerStatus ns = new PlayerStatus();

            ns.currentHealth = currentHealth;
            ns.maxHealth = maxHealth;
            ns.currentMood = currentMood;
            ns.maxMood = maxMood;

            ns.speedBonus = speedBonus;

            ns.jumpTimeBonus = jumpTimeBonus;
            ns.jumpPowerBonus = jumpPowerBonus;

            ns.lungCapacity = lungCapacity;
            ns.dirtiness = dirtiness;

            ns.scaredTimeLeft = scaredTimeLeft;
            ns.goodFoodTimeLeft = goodFoodTimeLeft;

            ns.statusFX = statusFX;
            ns.currentVibes = currentVibes;

            return ns;
        }

        public void clampValues()
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            currentMood = Mathf.Clamp(currentMood, 0, maxMood);
            dirtiness = Mathf.Clamp01(dirtiness);
        }

        public bool checkStatusEffect(PlayerStatusFX fx)
        {
            return ((statusFX & (int)fx) == (int)fx);
        }

        public void setStatusEffect(PlayerStatusFX fx)
        {
            statusFX |= (int)fx;
        }

        public void unSetStatusEffect(PlayerStatusFX fx)
        {
            statusFX &= ~(int)fx;
        }


        public bool checkVibe(VibeType v)
        {
            return ((currentVibes & (int)v) == (int)v);
        }

        public void setVibe(VibeType v)
        {
            currentVibes |= (int)v;
        }

        public void unSetVibe(VibeType v)
        {
            currentVibes &= ~(int)v;
        }
    }

    [System.Serializable]
    public class ProgressData
    {
        public bool hasHammer = false;
        public bool hasCloth = false;
        public bool hasSlingshot = false;

        public ProgressData makeCopy()
        {
            ProgressData ret = new ProgressData();

            ret.hasHammer = hasHammer;
            ret.hasCloth = hasCloth;
            ret.hasSlingshot = hasSlingshot;

            return ret;
        }
    }


    public class PlayerManager : Singleton<PlayerManager>
	{

        //The current player instance.
        public Player mPlayerInstance;

        //The player's current status. Not sure how much of this should be on the player instance.
        public PlayerStatus AmyStatus;
        public PlayerStatus CreamStatus;

        //Unlockable/Story progression
        public bool hasHammer = false;
        public bool hasCloth = false;
        public bool hasSlingshot = false;
        public List<StoryFlag> cutsceneFlags;
        public System.DateTime lastSaveTime;


        public PlayableCharacter currentCharacter = PlayableCharacter.Amy;

        //This will be placed at the last safe place/exit
        public Transform playerCheckpoint;

        public int lastExit = 0;

        public float playerDirtiness = 0.0f;
        public float playerStress = 0.0f;
        public float lastOrgasmCooldown = 0.0f;

        public int ringBank = 0;
        int ringCount = 0;
        public float stealthIndex = 0.0f;

        public bool playerHasStealthCamo = false;

        //Scene flags
        public bool isHubRoom = false;
        public bool isNightTime = false;
        public bool ringLeftChannel = false;
        

        private void Awake()
        {

            AmyStatus = GameManager.getSystemData().AmyParams.baseStats.makeCopy();
            CreamStatus = GameManager.getSystemData().CreamParams.baseStats.makeCopy();

            GameObject inst = new GameObject("CHECKPOINT");
            DontDestroyOnLoad(inst);
            playerCheckpoint = inst.transform;

            SaveGame.loadGame(0);
        }



        public void Init()
        {
            Debug.Log("PlayerManager Initialized!");
        }

        // Start is called before the first frame update
        void Start()
    	{
            
    	}

        void setNewGameState()
        {
            ringBank = 0;
            hasHammer = false;
            hasCloth = false;
            hasSlingshot = false;

            cutsceneFlags = new List<StoryFlag>();
            AmyStatus = GameManager.getSystemData().AmyParams.baseStats.makeCopy();
            CreamStatus = GameManager.getSystemData().CreamParams.baseStats.makeCopy();
        }

    	// Update is called once per frame
    	void Update()
    	{
            handleStealthIndex();

            if (lastOrgasmCooldown > 0.0f)
                lastOrgasmCooldown -= Time.deltaTime;

            if (mPlayerInstance)
                updatePlayerStatus(mPlayerInstance);

            if (currentCharacter != PlayableCharacter.Amy)
                processSleeping(AmyStatus);

            if (currentCharacter != PlayableCharacter.Cream)
                processSleeping(CreamStatus);
        }


        public PlayerStatus getCurrentPlayerStatus()
        {
            return getCharacterStatus(currentCharacter);
        }

        public PlayerStatus getCharacterStatus(PlayableCharacter chara)
        {
            switch(chara)
            {
                case PlayableCharacter.Amy:
                    return AmyStatus;

                case PlayableCharacter.Cream:
                    return CreamStatus;
            }

            return AmyStatus;
        }

        void handleStealthIndex()
        {
            /*
            if (!mPlayerInstance)
                return;

            PlayerBasicMove basicMove = mPlayerInstance.GetComponent<PlayerBasicMove>();

            if (!basicMove)
                return;

            stealthIndex = 0.0f;


            //Jumping makes you more visible
            if (basicMove.jumpTimer > 0.2f)
                stealthIndex -= 0.25f;

            //Crouching makes you have a lower profile
            if (basicMove.isCrouching)
                stealthIndex += 0.45f;

            //Hide in the water
            if (mPlayerInstance.getWaterDepth() > 1.0f)
                stealthIndex = 0.8f;

            //Invisibility, full stealth
            if (playerHasStealthCamo)
                stealthIndex = 1.0f;

            float oneMinus = 1.0f - stealthIndex;

            Color stealthColor = Color.green;

            if (stealthIndex < 0.75f)
                stealthColor = Color.yellow;

            if (stealthIndex < 0)
                stealthColor = Color.red;

            Circle.DrawEllipse(mPlayerInstance.transform.position + Vector3.up * 0.5f, Vector3.up, mPlayerInstance.transform.forward, oneMinus * 3.0f, oneMinus * 3.0f, 32, stealthColor);
            */
        }

        public void givePlayerStealthCamo()
        {
            /*
            if (!mPlayerInstance)
                return;

            if (!mPlayerInstance.GetComponent<StealthCamo>())
                mPlayerInstance.gameObject.AddComponent<StealthCamo>();

            mPlayerInstance.GetComponent<StealthCamo>().enabled = true;
            */
        }

        public void updatePlayerStatus(Player pl)
        {
            PlayerStatus pStats = pl.getStatus();

            processStatusFX(pl);
            processVibes(pl);
            processMood(pl);
            processHealing(pl);

        }

        public void processStatusFX(Player pl)
        {
            PlayerStatus pStats = pl.getStatus();
            const float scaredStaminaDrain = 0.25f;
            const float sickStaminaDrain = 0.35f;

            if (currentCharacter == pl.mChara)
                pStats.timeSpentResting = 0.0f;

            if (pStats.checkStatusEffect(PlayerStatusFX.Scared))
            {
                if(pStats.currentStamina > 0.02f)
                {
                    pStats.currentStamina -= (scaredStaminaDrain * Time.deltaTime);
                }
                else if(pStats.currentHealth > 0.0f)
                {
                    pStats.currentHealth -= scaredStaminaDrain * Time.deltaTime;
                    pl.updateHealth();
                }

                if(pStats.scaredTimeLeft > 0.0f)
                {
                    pStats.scaredTimeLeft -= Time.deltaTime;
                }
                else
                {
                    pStats.unSetStatusEffect(PlayerStatusFX.Scared);
                    pl.updateExpression();
                }
            }

            if((pStats.currentStamina / pStats.maxStamina) < 0.125f)
            {
                if(!pStats.checkStatusEffect(PlayerStatusFX.Tired))
                {
                    pStats.setStatusEffect(PlayerStatusFX.Tired);
                }
            }
            else if ((pStats.currentStamina / pStats.maxStamina) > 0.25f)
            {
                if (pStats.checkStatusEffect(PlayerStatusFX.Tired))
                {
                    pStats.unSetStatusEffect(PlayerStatusFX.Tired);
                }
            }

            if(pStats.checkStatusEffect(PlayerStatusFX.Sick))
            {
                //Shouldn't be out and about while sick, young lady....
                if (pl.acceleration.magnitude > 2.0f && !pStats.checkVibe(VibeType.Safe))
                {
                    if (pStats.currentStamina > 0.02f)
                    {
                        pStats.currentStamina -= (sickStaminaDrain * Time.deltaTime);
                    }
                    else if (pStats.currentHealth > 0.0f)
                    {
                        pStats.currentHealth -= sickStaminaDrain * Time.deltaTime;
                        pl.updateHealth();
                    }
                }
            }

            if(pStats.checkStatusEffect(PlayerStatusFX.Dirty))
            {
                if(pStats.dirtiness < 0.1f)
                {
                    pStats.unSetStatusEffect(PlayerStatusFX.Dirty);
                }

                if(Time.frameCount % 3600 == 0)
                {
                    //1/64 chance of getting sick every 30 seconds while dirty.
                    if(Random.Range(0, 64) == 4)
                    {
                        pStats.setStatusEffect(PlayerStatusFX.Sick);
                        pStats.sickTimeLeft = Random.Range(Helper.minutesToSeconds(18), Helper.minutesToSeconds(36));
                    }
                }
            }
        }

        public void processVibes(Player pl)
        {
            PlayerStatus pStats = pl.getStatus();

            if (pStats.checkVibe(VibeType.Dark) && !pStats.checkVibe(VibeType.Safe))
            {
                if(pl.mChara == PlayableCharacter.Cream)
                {
                    inflictScaredStatus(pl, 0.25f);
                }
            }

            if (pStats.checkVibe(VibeType.Scary) && !pStats.checkVibe(VibeType.Safe))
            {
                if (pl.mChara == PlayableCharacter.Cream)
                {
                    inflictScaredStatus(pl, 0.25f);
                }
            }

            if(pStats.checkVibe(VibeType.Safe))
            {
                
            }

            if(pStats.checkVibe(VibeType.Dirty))
            {
                const float baseDirtPerSpeed = 0.02f;

                if(pl.acceleration.magnitude > 0.1f)
                {
                    pStats.dirtiness += Time.deltaTime * (baseDirtPerSpeed * pl.acceleration.magnitude);
                }

                if(pStats.dirtiness > 1.0f && !pStats.checkStatusEffect(PlayerStatusFX.Dirty))
                {
                    pStats.setStatusEffect(PlayerStatusFX.Dirty);
                }
            }
        }

        public void processMood(Player pl)
        {
            PlayerStatus pStats = pl.getStatus();

            float moodFac = pStats.currentMood / pStats.maxMood;
            float healthFac = pStats.currentHealth / pStats.maxHealth;
            float staminaFac = pStats.currentStamina / pStats.currentStamina;

            float genkiAverage = (staminaFac + healthFac + healthFac) * 0.3333333f;

            if (genkiAverage > 0.95f)
                genkiAverage = 1.0f;

            float targetMood = genkiAverage * pStats.maxMood;

            if (pStats.checkStatusEffect(PlayerStatusFX.Relaxed))
                targetMood *= 1.2f;

            if (pStats.checkStatusEffect(PlayerStatusFX.GoodFood))
                targetMood *= 1.5f;

            if (pStats.checkStatusEffect(PlayerStatusFX.Tired))
                targetMood *= 0.75f;

            if (pStats.checkStatusEffect(PlayerStatusFX.Sick))
                targetMood *= 0.65f;

            if (pStats.checkStatusEffect(PlayerStatusFX.Scared))
                targetMood *= 0.5f;

            if (pStats.checkStatusEffect(PlayerStatusFX.Dirty))
                targetMood *= 0.8f;

            if (pStats.checkStatusEffect(PlayerStatusFX.Horny))
                targetMood *= 0.9f;

            if (pStats.checkVibe(VibeType.Safe))
                targetMood = Mathf.Clamp(targetMood, pStats.maxMood * 0.25f, pStats.maxMood);

            pStats.currentMood = Mathf.Lerp(pStats.currentMood, targetMood, Time.deltaTime * 1.2f);
            pStats.clampValues();
        }

        public void processHealing(Player pl)
        {
            PlayerStatus pStats = pl.getStatus();

            const float baseHealFac = 0.075f;

            float moodFac = pStats.currentMood / pStats.maxMood;
            float healthFac = pStats.currentHealth / pStats.maxHealth;

            if (healthFac < 0.999f)
            {
                if (pStats.currentStamina > 0.01f)
                {
                    pStats.currentHealth += (Time.deltaTime * baseHealFac) * moodFac;
                    pStats.currentStamina -= (Time.deltaTime * baseHealFac);
                }
            }
            else
            {
                pStats.currentHealth = pStats.maxHealth;
            }
        }



        public void processSleeping(PlayerStatus pStats, float time = 1.0f)
        {
            const float staminaHealRate = 0.1f;
            const float healthHealRate = 0.1f;
            pStats.timeSpentResting += (Time.deltaTime * time);

            if (pStats.checkStatusEffect(PlayerStatusFX.Scared) || pStats.checkStatusEffect(PlayerStatusFX.Tired))
            {
                pStats.unSetStatusEffect(PlayerStatusFX.Scared);
                pStats.unSetStatusEffect(PlayerStatusFX.Tired);
            }

            if(pStats.currentStamina < pStats.maxStamina)
            {
                pStats.currentStamina += staminaHealRate * (Time.deltaTime * time);
                pStats.clampValues();
            }
            else if(pStats.currentHealth < pStats.maxHealth)
            {
                pStats.currentHealth += healthHealRate * (Time.deltaTime * time);
                pStats.clampValues();
            }

            if(pStats.checkStatusEffect(PlayerStatusFX.Sick))
            {
                pStats.sickTimeLeft -= (Time.deltaTime * time);

                if(pStats.sickTimeLeft < 0.0f)
                {
                    pStats.unSetStatusEffect(PlayerStatusFX.Sick);
                }
            }
        }


        public Player getPlayer(bool search = true)
        {
            if (mPlayerInstance != null)
                return mPlayerInstance;

            if (search)
                mPlayerInstance = FindObjectOfType<Player>();

            if (mPlayerInstance == null)
            {
                //Debug.LogWarning("    WARNING: No Player Found! ");
                return null;
            }

            return mPlayerInstance;
        }

        public void spawnPlayerAtExit()
        {
            Exit mExit = null;

            foreach (Exit e in FindObjectsOfType<Exit>())
            {
                if (e.exitNumber == lastExit)
                    mExit = e;
            }

            if (mExit == null)
            {
                GameObject inst = new GameObject();
                inst.transform.position = Vector3.up;
                mExit = inst.AddComponent<Exit>();
            }

            playerCheckpoint.transform.position = mExit.transform.position;
            playerCheckpoint.transform.rotation = mExit.transform.rotation;

            
            spawnPlayerAtCheckpoint();

            if (mExit.altCheckpoint)
            {
                playerCheckpoint.transform.position = mExit.altCheckpoint.transform.position;
                playerCheckpoint.transform.rotation = mExit.transform.rotation;
            }
        }

        public Player spawnPlayerAtCheckpoint()
        {
            Debug.Log("Respawning Player...");

            if (mPlayerInstance != null)
            {
                //Debug.Log("Destroying duplicate player...");
                //Destroy(mPlayerInstance.gameObject);
                mPlayerInstance.transform.position = playerCheckpoint.transform.position;
                mPlayerInstance.direction = playerCheckpoint.transform.forward;
                mPlayerInstance.changeCurrentMode(PlayerModes.NORMAL);
                return mPlayerInstance;
            }


            mPlayerInstance = Player.Spawn(playerCheckpoint.transform.position, playerCheckpoint.transform.forward, currentCharacter);
            
            //saveGame.lastScene = SceneManager.GetActiveScene().name;
            // saveGame.lastExit = lastExit;

            return mPlayerInstance;
        }


        public void PlayerDieRespawn(PlayerKilled.DeathType type)
        {
            Timing.RunCoroutine(doPlayerRespawnSequence(type));
        }
        

        public void addRings(int n)
        {
            ringCount += n;

            //why can't i hold all these rings
            if (ringCount > 99999)
                ringCount = 99999;
        }

        public void subtractRings(int n)
        {
            ringCount -= n;

            if (ringCount < 0)
                ringCount = 0;
        }

        public int getRings()
        {
            return ringCount;
        }
        
        //I would give this a more fitting name but I want to pay homage to SA1's hilarious kill function name (killHimP())
        public void killHer()
        {
            if (!mPlayerInstance)
                return;

            mPlayerInstance.GetComponent<PlayerKilled>().deathType = PlayerKilled.DeathType.Normal;
            mPlayerInstance.changeCurrentMode(PlayerModes.KILLED);
        }

        public void inflictScaredStatus(Player pl, float time = 5.0f)
        {
            PlayerStatus pStats = pl.getStatus();
            
            if (!pStats.checkStatusEffect(PlayerStatusFX.Scared) && pStats.scaredTimeLeft < 0.01f)
            {
                pStats.setStatusEffect(PlayerStatusFX.Scared);
                pl.mVoice.playVoice(pl.mVoice.scared, true);
                pl.updateExpression();
            }

            pStats.scaredTimeLeft = time;
        }


        //TODO: add different respawn situations.
        public IEnumerator<float> doPlayerRespawnSequence(PlayerKilled.DeathType type)
        {
           
            GameManager.Instance.cameraInputDisabled = true;
            GameManager.Instance.playerInputDisabled = true;

            yield return 0f;

            MusicManager.Instance.fadeBGM(0.0f, 1.0f);

            if (mPlayerInstance)
            {
                while (mPlayerInstance.GetComponent<PlayerVoice>().voiceSource.isPlaying)
                    yield return 0f;
            }
            
            yield return Timing.WaitForSeconds(0.5f);

            UIManager.Instance.fadeScreen(false, 1.0f, false);


            yield return Timing.WaitForSeconds(1.1f);
            //MusicManager.Instance.restartMusic();


            if (mPlayerInstance)
                GameObject.Destroy(mPlayerInstance.gameObject);

            //Lose money for getting owned.
            ringCount = 0;

            if(type == PlayerKilled.DeathType.Falling || type == PlayerKilled.DeathType.Drowned)
            {
                getCurrentPlayerStatus().currentHealth -= getCurrentPlayerStatus().maxHealth * 0.25f;
                
                if(getCurrentPlayerStatus().currentHealth > 0.0f)
                {
                    spawnPlayerAtCheckpoint();
                    yield return 0f;

                    PlayerManager.Instance.spawnPlayerAtCheckpoint();

                    yield return Timing.WaitForSeconds(1.0f);

                    UIManager.Instance.fadeScreen(true, 1.0f, false);

                    yield return Timing.WaitForSeconds(1.1f);

                    GameManager.Instance.playerInputDisabled = false;
                    GameManager.Instance.cameraInputDisabled = false;

                    yield break;
                }
            }


            getCurrentPlayerStatus().currentHealth = getCurrentPlayerStatus().maxHealth * 0.5f;
            getCurrentPlayerStatus().currentStamina = getCurrentPlayerStatus().maxStamina * 0.5f;

            if(type == PlayerKilled.DeathType.Corrupted)
            {
                getCurrentPlayerStatus().currentStamina = 0.0f;
                getCurrentPlayerStatus().unSetStatusEffect(PlayerStatusFX.Scared);
            }

           



            yield return 0f;


            processSleeping(getCurrentPlayerStatus());

            if (currentCharacter == PlayableCharacter.Amy)
                currentCharacter = PlayableCharacter.Cream;
            else if (currentCharacter == PlayableCharacter.Cream)
                currentCharacter = PlayableCharacter.Amy;

            Timing.RunCoroutine(setupWakeupScene());

        }

        public void wakeupScene()
        {
            
            Timing.RunCoroutine(setupWakeupScene());
        }
        
        public IEnumerator<float> setupWakeupScene()
        {
            yield return 0f;
            UIManager.Instance.fadeScreen(false, 0.5f, false);
            yield return Timing.WaitForSeconds(0.5f);

            SceneManager.LoadScene("AmyRoom");
            yield return Timing.WaitForSeconds(0.2f);

            if(currentCharacter == PlayableCharacter.Amy)
                GameObject.Instantiate(GameManager.Instance.systemData.Cutscene_AmyWakeup);

            if(currentCharacter == PlayableCharacter.Cream)
                GameObject.Instantiate(GameManager.Instance.systemData.Cutscene_CreamWakeup);

            UIManager.Instance.fadeScreen(true, 3.0f);
        }

        public void characterSwitch(PlayableCharacter newChar)
        {
            Timing.RunCoroutine(doCharacterSwitch(newChar));
        }

        IEnumerator<float> doCharacterSwitch(PlayableCharacter newChar)
        {
            GameManager.Instance.playerInputDisabled = true;
            GameManager.Instance.cameraInputDisabled = true;

            UIManager.Instance.fadeScreen(false, 0.5f, false);
            yield return Timing.WaitForSeconds(0.6f);

            playerCheckpoint.transform.position = mPlayerInstance.transform.position;
            playerCheckpoint.transform.rotation = mPlayerInstance.transform.rotation;

            currentCharacter = newChar;
            GameObject.Destroy(mPlayerInstance.gameObject);
            mPlayerInstance = null;
            
            spawnPlayerAtCheckpoint();

            foreach (CharacterEventSwitcher c in FindObjectsOfType<CharacterEventSwitcher>())
                c.switchAllObjects();

            yield return Timing.WaitForSeconds(0.5f);

            UIManager.Instance.fadeScreen(true, 0.5f, false);
            yield return Timing.WaitForSeconds(0.5f);

            GameManager.Instance.playerInputDisabled = false;
            GameManager.Instance.cameraInputDisabled = false;
        }


    }

}
