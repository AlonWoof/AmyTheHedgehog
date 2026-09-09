using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;

/* Copyright 2026 Jennifer Haden */
namespace Amy
{
    public enum PlayableCharacter
    {
        Amy,
        Cream,
        YoungAmy,
        Yume,
        None
    }

    [System.Flags]
    public enum PlayerStatusFX
    {
        None = 0,
        Relaxed = 1,
        Scared = 2,
        Dirty = 4,
        Horny = 8,
        Sick = 16,
        GoodFood = 32,
        RecentOrgasm = 64,
        Invisible = 128
    }

    public enum PlayerNPCLocation
    {
        None,
        Bed,
        WatchTV,
        Shower,
        MAX
    }

    public enum ExitLevelType
    {
        NORMAL,
        WARP
    }

    [System.Serializable]
    public class DayEvents
    {
        public int luckyNumber = 0;

        public int[] dayNumbers = new int[5];
    };

    [System.Serializable]
    public class PlayerStatus
    {
        public float currentHealth = 25.0f;
        public float maxHealth = 25.0f;
        //public float currentMood = 25.0f;
        //public float maxMood = 25.0f;
        public float maxMagic = 25.0f;
        public float currentMagic = 25.0f;

        public float speedBonus = 0.0f;

        public float jumpTimeBonus = 0.0f;
        public float jumpPowerBonus = 0.0f;

        public float lungCapacity = 20.0f;
        public float dirtiness = 0.0f;

        public float baseMeleeDamage = 10.0f;
        public float baseRangedDamage = 5.0f;

        public int statusFX;
        public int currentVibes;
        public float scaredTimeLeft = 0.0f;
        public float goodFoodTimeLeft = 0.0f;
        public float recentOrgasmTimeLeft = 0.0f;
        public float sickTimeLeft = 0.0f;

        public float timeSpentResting = 0.0f;
        public float sleepTimeLeft = 0.0f;

        public PlayerNPCLocation npcLocation = PlayerNPCLocation.None;

        public List<ItemData> items;

        public PlayerStatus makeCopy()
        {
            PlayerStatus ns = new PlayerStatus();

            ns.currentHealth = currentHealth;
            ns.maxHealth = maxHealth;
            //ns.currentMood = currentMood;
            //ns.maxMood = maxMood;

            ns.speedBonus = speedBonus;

            ns.jumpTimeBonus = jumpTimeBonus;
            ns.jumpPowerBonus = jumpPowerBonus;

            ns.lungCapacity = lungCapacity;
            ns.dirtiness = dirtiness;

            ns.scaredTimeLeft = scaredTimeLeft;
            ns.goodFoodTimeLeft = goodFoodTimeLeft;
            ns.recentOrgasmTimeLeft = recentOrgasmTimeLeft;

            ns.statusFX = statusFX;
            ns.currentVibes = currentVibes;

            ns.items = new List<ItemData>();

            foreach(ItemData i in items)
            {
                ns.items.Add(ItemData.getItemData(i.getHash()));
            }

            return ns;
        }

        public void clampValues()
        {
            //if (currentMood != currentMood)
            //    currentMood = 0;

            if (currentHealth != currentHealth)
                currentHealth = 0;

            if (currentMagic != currentMagic)
                currentMagic = 0;

            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            currentMagic = Mathf.Clamp(currentMagic, 0, maxMagic);
            //currentMood = Mathf.Clamp(currentMood, 0, maxMood);
            dirtiness = Mathf.Clamp01(dirtiness);
        }

        public bool addItem(string itemName)
        {
            return addItem(Animator.StringToHash(itemName.ToLower()));
        }

        public bool addItem(int itemHash)
        {
            ItemData i = ItemData.getItemData(itemHash);

            if (i == null)
                return false;

            if(items.Count >= 10)
            {
                return false;
            }

            items.Add(i);
            return true;
        }

        public bool checkStatusEffect(PlayerStatusFX fx)
        {
            return ((statusFX & (int)fx) == (int)fx);
        }

        public bool checkStatusEffect(int fx)
        {
            return ((statusFX & fx) == fx);
        }

        public void setStatusEffect(PlayerStatusFX fx)
        {
            statusFX |= (int)fx;
        }

        public void unSetStatusEffect(PlayerStatusFX fx)
        {
            statusFX &= ~(int)fx;
        }

        public void clearStatusEffects()
        {
            statusFX = 0;
        }

        public bool checkVibe(VibeType v)
        {
            return ((currentVibes & (int)v) == (int)v);
        }

        public bool checkVibe(int v)
        {
            return ((currentVibes & v) == v);
        }

        public void setVibe(VibeType v)
        {
            currentVibes |= (int)v;
        }

        public void unSetVibe(VibeType v)
        {
            currentVibes &= ~(int)v;
        }

        public float getCondition()
        {
            float healthFac = currentHealth / maxHealth;
            float staminaFac = currentMagic / maxMagic;

            //Average of health and stamina.
            return (healthFac + staminaFac) * 0.5f;
        }

        public void determineNPCLocation()
        {
            if(sleepTimeLeft > 0.0f || checkStatusEffect(PlayerStatusFX.Sick))
            {
                npcLocation = PlayerNPCLocation.Bed;
            }
            else
            {
                int r = Random.Range((int)PlayerNPCLocation.WatchTV, (int)PlayerNPCLocation.MAX);
                npcLocation = (PlayerNPCLocation)r;
            }
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
        public bool hasGlowBracelet = false;
        public bool hasUkiwa = false;
        public List<StoryFlag> storyFlags;
        public System.DateTime lastSaveTime;

        public int AmySuikaAffection = 0;
        public int CreamSuikaAffection = 0;


        public PlayableCharacter currentCharacter = PlayableCharacter.Amy;

        //This will be placed at the last safe place/exit
        public Transform playerCheckpoint;

        public int lastExit = 0;
        public ExitLevelType exitType;

        public float playerDirtiness = 0.0f;
        public float playerStress = 0.0f;
        public float lastOrgasmCooldown = 0.0f;

        public int ringBank = 0;
        public int ringCount = 0;

        public float totalSeconds = 0.0f;
        public int totalMinutes = 0;
        public int totalHours = 0;
        public int days = 0;

        public int daysTilMenstruation = 14;

        public float stealthIndex = 0.0f;

        public int saveFileSlot = -1;

        //Scene flags
        public bool isHubWorld = false;
        public bool isSmallRoom = false;
        public bool isNightTime = false;
        public bool isRaceLevel = false;
        public bool ringLeftChannel = false;

        public bool itemMenuOpen = false;

        public int universeNumber = 0;
        public DayEvents todayEvents;


        private void Awake()
        {
            AmyStatus = GameManager.getSystemData().AmyParams.baseStats.makeCopy();
            CreamStatus = GameManager.getSystemData().CreamParams.baseStats.makeCopy();
            todayEvents = new DayEvents();
            storyFlags = new List<StoryFlag>();

            randomizeDayEvents();
            PlayerManager.Instance.universeNumber = Random.Range(1, int.MaxValue);

            GameObject inst = new GameObject("CHECKPOINT");
            DontDestroyOnLoad(inst);
            playerCheckpoint = inst.transform;

            SaveGame.readConfigFile();
            SaveGame.loadGame(saveFileSlot);

            SaveGame.readSADXNudeModData();
        }

        public static string getPlayerNPCLocationString(PlayerNPCLocation loc)
        {

            switch(loc)
            {
                case PlayerNPCLocation.None:
                    return "None";
                case PlayerNPCLocation.Bed:
                    return "Sleeping";
                case PlayerNPCLocation.Shower:
                    return "Showering";
                case PlayerNPCLocation.WatchTV:
                    return "Watching TV";
                case PlayerNPCLocation.MAX:
                    return "Fix yer damn code, Jenny";
            }    

            return "UNKNOWN";
        }

        public void Init()
        {
            Debug.Log("PlayerManager Initialized!");
        }

        public void advanceDay()
        {
            days++;
            randomizeDayEvents();
            //SystemColors.
           // processSleeping(AmyStatus, )
           //
        }

        public void randomizeDayEvents()
        {

            todayEvents.luckyNumber = Random.Range(1, int.MaxValue);

            for(int i = 0; i < 5; i++)
                todayEvents.dayNumbers[i] = Random.Range(1, int.MaxValue);


        }

        public bool isBadDay()
        {
            //She's too young.
            if (currentCharacter == PlayableCharacter.Cream)
                return false;

            //if (isPrologue())
           //     return false;

            if (daysTilMenstruation > 0)
                return false;

            return true;
        }

        public bool getStoryFlag(string name)
        {
            return getStoryFlag(Animator.StringToHash(name));
        }

        //Important flags...
        //SADX_NUDE - has SADX nude mod installed
        //TAILS_CUNNY - has trans tails enabled in sadx nude mod

        public bool getStoryFlag(int hash)
        {

            foreach(StoryFlag f in storyFlags)
            {
                if (f.sceneHash == hash)
                    return f.isFinished;
            }

            return false;
        }

        public void setStoryFlag(string name, bool value)
        {
            setStoryFlag(Animator.StringToHash(name), value);
        }

        public void setStoryFlag(int hash, bool value)
        {
            foreach (StoryFlag f in storyFlags)
            {
                if (f.sceneHash == hash)
                {
                    f.isFinished = value;
                    return;
                }
            }

            storyFlags.Add(new StoryFlag(hash, value));
        }

        public bool isPrologue()
        {
            return !getStoryFlag("PROLOGUE_DONE");
        }

        public void checkStoryFlags()
        {
            //if (isHubWorld && isPrologue())
                //setStoryFlag("PROLOGUE_DONE", true);

            if (getStoryFlag("AMYDECAL_CLEAR"))
            {
                GameManager.Instance.debugMode = true;
                setStoryFlag("DEVCOMMENT", true);
            }
        }

        public void decidePlayerNPCLocation()
        {
            AmyStatus.determineNPCLocation();
            CreamStatus.determineNPCLocation();
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


            updatePlayTime();
            handleItemMenu();

            
        }

        void updateCameraMode()
        {
            if (!mPlayerInstance)
                return;

            if (!mPlayerInstance.tpc)
                return;
        
            if(GameManager.Instance.gamePaused)
            {
                if (mPlayerInstance.tpc.mode != TPCMode.PauseMenu)
                    mPlayerInstance.tpc.changeCameraMode(TPCMode.PauseMenu);
            }
            else
            {
                if (mPlayerInstance.tpc.mode == TPCMode.PauseMenu)
                    mPlayerInstance.tpc.changeCameraMode(TPCMode.Normal);
            }
        }

        void handleItemMenu()
        {
            ItemMenu itm = UIManager.Instance.itemMenu;

            if(canOpenItemMenu())
            {
                if (Input.GetButtonDown("Select") || Input.GetKeyDown(KeyCode.Tab))
                {
                    if (!itemMenuOpen)
                    {
                        openItemMenu();
                    }
                    else
                    {
                        closeItemMenu();
                    }
                }
            }
        }


        void updatePlayTime()
        {
            if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
                return;

            //If there's no player, are we really playing?
            if (!mPlayerInstance)
                return;

            totalSeconds += Time.deltaTime;

            while(totalSeconds > 60.0f)
            {
                totalSeconds -= 60.0f;
                totalMinutes++;
            }

            while(totalMinutes > 60)
            {
                totalMinutes -= 60;
                totalHours++;
            }
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
            
            if (!mPlayerInstance)
                return;

            PlayerBasicMove basicMove = mPlayerInstance.GetComponent<PlayerBasicMove>();

            if (!basicMove)
                return;

            stealthIndex = 0.0f;


            //Jumping makes you more visible
            //if (basicMove.jumpTimer > 0.2f)
            //    stealthIndex -= 0.25f;

            //Crouching makes you have a lower profile
            //if (basicMove.isCrouching)
            //    stealthIndex += 0.45f;

            //Hide in the water
            if (mPlayerInstance.getWaterDepth() > 1.0f)
                stealthIndex = 0.8f;

            //Invisibility, full stealth
            if (getCurrentPlayerStatus().checkStatusEffect(PlayerStatusFX.Invisible))
                stealthIndex = 1.0f;

            float oneMinus = 1.0f - stealthIndex;

            Color stealthColor = Color.green;

            if (stealthIndex < 0.75f)
                stealthColor = Color.yellow;

            if (stealthIndex < 0)
                stealthColor = Color.red;

            Circle.DrawEllipse(mPlayerInstance.transform.position + Vector3.up * 0.5f, Vector3.up, mPlayerInstance.transform.forward, oneMinus * 1.5f, oneMinus * 1.5f, 32, stealthColor);
            
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
            const float scaredHealthDrain = 0.25f;
            const float sickStaminaDrain = 0.35f;
            const float embarassedStaminaDrain = 0.1f;
            const float dirtyAmyStaminaDrain = 0.1f;
            const float goodFoodStaminaHeal = 0.2f;
            const float orgasmHealthHeal = 0.2f;

            if (currentCharacter == pl.mChara)
                pStats.timeSpentResting = 0.0f;

            if (pStats.checkStatusEffect(PlayerStatusFX.Scared))
            {
                if(pStats.currentHealth > 0.0f)
                {
                    pStats.currentHealth -= scaredHealthDrain * Time.deltaTime;
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

            if(pStats.checkStatusEffect(PlayerStatusFX.RecentOrgasm))
            {
                if (pStats.recentOrgasmTimeLeft > 0.0f)
                {
                    pStats.recentOrgasmTimeLeft -= Time.deltaTime;
                    pStats.currentHealth += orgasmHealthHeal * Time.deltaTime;
                    pl.updateHealth();
                }
                else
                {
                    pStats.unSetStatusEffect(PlayerStatusFX.RecentOrgasm);
                }
             
            }


            if (pStats.checkStatusEffect(PlayerStatusFX.GoodFood))
            {
                if (pStats.goodFoodTimeLeft > 0.0f)
                {
                    pStats.goodFoodTimeLeft -= Time.deltaTime;

                    pStats.currentMagic += goodFoodStaminaHeal * Time.deltaTime;
                    pl.updateHealth();
                }
                else
                {
                    pStats.unSetStatusEffect(PlayerStatusFX.GoodFood);
                }

            }


            if(pStats.checkStatusEffect(PlayerStatusFX.Sick))
            {
                //Shouldn't be out and about while sick, young lady....
                if (pl.acceleration.magnitude > 2.0f && !pStats.checkVibe(VibeType.Safe))
                {
                    if (pStats.currentMagic > 0.02f)
                    {
                        pStats.currentMagic -= (sickStaminaDrain * Time.deltaTime);
                    }
                    else if (pStats.currentHealth > 0.0f)
                    {
                        pStats.currentHealth -= sickStaminaDrain * Time.deltaTime;
                        pl.updateHealth();
                    }
                }
            }

            if(pStats.checkStatusEffect(PlayerStatusFX.Horny))
            {
                if (Time.frameCount % 360 == 0)
                {
                    if(Random.Range(0, 100) > 50)
                        pl.cunnyDrip();
                }

                if(pl.areaDetector.visibleNPCCount > 0)
                {
                    //Hazukashii yo... they can all see my omanko dripping...
                    pStats.currentHealth -= (embarassedStaminaDrain * pl.areaDetector.visibleNPCCount) * Time.deltaTime;
                }

                if(pl.modeRubbing.karadaMesh)
                {
                    if(pl.modeRubbing.karadaMesh.sharedMesh.blendShapeCount > 0)
                        pl.modeRubbing.karadaMesh.SetBlendShapeWeight(0, 100);
                }
            }
            else
            {
                if (pl.modeRubbing.karadaMesh)
                {
                    if (pl.modeRubbing.karadaMesh.sharedMesh.blendShapeCount > 0)
                        pl.modeRubbing.karadaMesh.SetBlendShapeWeight(0, 0);
                }
            }

            if(pStats.checkStatusEffect(PlayerStatusFX.Dirty))
            {
                if(Time.frameCount % 3600 == 0)
                {
                    //1/64 chance of getting sick every 30 seconds while dirty.
                    if(Random.Range(0, 64) == 4)
                    {
                        pStats.setStatusEffect(PlayerStatusFX.Sick);
                        pStats.sickTimeLeft = Random.Range(Helper.minutesToSeconds(18), Helper.minutesToSeconds(36));
                    }
                }

                if (pStats.dirtiness < 0.1f)
                {
                    pStats.unSetStatusEffect(PlayerStatusFX.Dirty);
                }

                if(currentCharacter == PlayableCharacter.Amy)
                {
                    pStats.currentMagic -= (dirtyAmyStaminaDrain * Time.deltaTime);
                }
            }
            else
            {
                if (pStats.dirtiness > 0.9f)
                {
                    pStats.setStatusEffect(PlayerStatusFX.Dirty);
                }
            }




        }

        public void processVibes(Player pl)
        {
            PlayerStatus pStats = pl.getStatus();

            if (pStats.checkVibe(VibeType.Dark) && !pStats.checkVibe(VibeType.Safe))
            {
                if (pl.mChara == PlayableCharacter.Cream && !hasGlowBracelet)
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
                const float baseDirtPerSpeed = 0.002f;

                Vector3 spd = pl.acceleration;
                spd.y = 0;

                if (spd.magnitude > 0.1f)
                {

                    pStats.dirtiness += Time.deltaTime * (baseDirtPerSpeed * spd.magnitude);
                }
            }
        }


        //Doing away with this other axis soon.  We don't need 3.
        public void processMood(Player pl)
        {
            /*
            PlayerStatus pStats = pl.getStatus();

            float moodFac = pStats.currentMood / pStats.maxMood;
            float healthFac = pStats.currentHealth / pStats.maxHealth;
            float staminaFac = pStats.currentMagic / pStats.maxMagic;

            float genkiAverage = (staminaFac + healthFac) * 0.5f;
            

            if (genkiAverage > 0.95f)
                genkiAverage = 1.0f;

            if (genkiAverage != genkiAverage)
                genkiAverage = 0.0f;

            genkiAverage = Mathf.Clamp01(genkiAverage);

            float targetMood = genkiAverage * pStats.maxMood;

            if (pStats.checkStatusEffect(PlayerStatusFX.Relaxed))
                targetMood *= 1.2f;

            if (pStats.checkStatusEffect(PlayerStatusFX.GoodFood))
                targetMood *= 1.2f;

            if (pStats.checkStatusEffect(PlayerStatusFX.RecentOrgasm))
                targetMood *= 1.2f;

            if (pStats.checkStatusEffect(PlayerStatusFX.Tired))
                targetMood *= 0.5f;

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
            */
        }

        public string getMoodLabel()
        {
            PlayerStatus pStats = getCurrentPlayerStatus();

            float fac = pStats.getCondition();

            if (fac > 0.8f)
            {
                return "<color=#FFEF08>Happy!</color>";
            }
            else if(fac > 0.5f)
            {
                return "<color=#F3CC7A>OK!</color>";
            }
            else if (fac > 0.1f)
            {
                return "<color=#E17555>Bad...</color>";
            }
            else if(fac < 0.1f)
            {
                return "<color=#B09790>No more please...</color>";
            }

            return "???";
        }

        public void processHealing(Player pl)
        {
            PlayerStatus pStats = pl.getStatus();

            const float baseHealFac = 0.125f;

            //float moodFac = pStats.currentMood / pStats.maxMood;
            float healthFac = pStats.currentHealth / pStats.maxHealth;

            if (healthFac < 0.99999f)
            {
                if (pStats.currentMagic > 0.01f)
                {
                   // pStats.currentHealth += (Time.deltaTime * baseHealFac) * moodFac;
                   //pStats.currentMagic -= (Time.deltaTime * baseHealFac);
                }
            }
            else
            {
                pStats.currentHealth = pStats.maxHealth;
            }

            pl.updateHealth();
        }



        public void processSleeping(PlayerStatus pStats, float time = 1.0f)
        {
            pStats.timeSpentResting += (Time.deltaTime * time);

            if (pStats.sleepTimeLeft > 0.0f)
            {
                pStats.sleepTimeLeft -= (Time.deltaTime * time);
            }

            if(pStats.sleepTimeLeft < 0.0f)
            {
                pStats.currentHealth = pStats.maxHealth;
                pStats.currentMagic = pStats.maxMagic;
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

            //Just gonna throw this in here...
            checkStoryFlags();

            if (mPlayerInstance != null)
            {
                //Debug.Log("Destroying duplicate player...");
                Destroy(mPlayerInstance.tpc);
                Destroy(mPlayerInstance.gameObject);
                //mPlayerInstance.transform.position = playerCheckpoint.transform.position;
               // mPlayerInstance.direction = playerCheckpoint.transform.forward;
               // mPlayerInstance.changeCurrentMode(PlayerModes.NORMAL);
                //return mPlayerInstance;
            }

            if (FindObjectOfType<ThirdPersonCamera>())
            {
                Destroy(FindObjectOfType<ThirdPersonCamera>().gameObject);
            }

            if (FindObjectOfType<CameraWaterFX>())
            {
                FindObjectOfType<CameraWaterFX>().isInWater = false;
            }

            Vector3 pos = playerCheckpoint.transform.position;

            if (PlayerManager.Instance.isBadDay())
            {
                int rng = Random.Range(0, 100);

                //UH OH
                if (rng % 8 == 0)
                {
                    pos += new Vector3(Random.Range(-64, 64), Random.Range(-64, 64), Random.Range(-64, 64));
                }

            }



            mPlayerInstance = Player.Spawn(pos, playerCheckpoint.transform.forward, currentCharacter);


            //saveGame.lastScene = SceneManager.GetActiveScene().name;
            // saveGame.lastExit = lastExit;

            if (!getStoryFlag("PROLOGUE_DONE"))
                exitType = ExitLevelType.NORMAL;

            if(exitType == ExitLevelType.WARP)
            {
                mPlayerInstance.startWarpExit();
            }

            exitType = ExitLevelType.NORMAL;
            

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

        public void subtractTotalRings(int n)
        {
            if(ringCount < n)
            {
                ringCount = 0;
                n -= ringCount;
            }
            else
            {
                ringCount -= n;
                return;
            }

            ringBank -= n;

            ringBank = Mathf.Clamp(ringBank, 0, 999999);
        }

        public int getRings()
        {
            return ringCount;
        }

        public void transferRingsToBank()
        {
            ringBank += ringCount;
            ringCount = 0;
        }

        public int getTotalRings()
        {
            return ringCount + ringBank;
        }

        public bool canOpenItemMenu()
        {
            if (GameManager.Instance.playerInputDisabled)
                return false;

            if (!mPlayerInstance)
                return false;

            if (mPlayerInstance.currentMode != PlayerModes.NORMAL)
                return false;

            if (GameManager.Instance.gamePaused)
                return false;

            return true;
        }

        public void openItemMenu()
        {

            ItemMenu itm = UIManager.Instance.itemMenu;

            itm.openMenu();
        }

        public void closeItemMenu()
        {

            ItemMenu itm = UIManager.Instance.itemMenu;

            itm.closeMenu();
        }

        public void useItem(string itemName)
        {
            useItem(Animator.StringToHash(itemName.ToLower()));
        }

        public void useItem(int itemHash)
        {
            PlayerStatus pstats = mPlayerInstance.getStatus();

            foreach (ItemData i in pstats.items)
            {
                if (i.getHash() == itemHash)
                {
                    useItem(i);
                }
            }
        }

        public void useItem(ItemData item)
        {
            item.useItem();

            if(item.consumable)
            {
                mPlayerInstance.getStatus().items.Remove(item);
                mPlayerInstance.getStatus().items.TrimExcess();
            }
        }
        
        //I would give this a more fitting name but I want to pay homage to SA1's hilarious kill function name (killHimP())
        public void killHer()
        {
            if (!mPlayerInstance)
                return;

            mPlayerInstance.GetComponent<PlayerKilled>().deathType = PlayerKilled.DeathType.Normal;
            mPlayerInstance.changeCurrentMode(PlayerModes.KILLED);
        }

        public void addStealthCamo(Player pl)
        {
            PlayerStatus pStats = pl.getStatus();

            if(!pStats.checkStatusEffect(PlayerStatusFX.Invisible))
            {
                pStats.setStatusEffect(PlayerStatusFX.Invisible);

                StealthCamo camo = pl.GetComponentInChildren<StealthCamo>();

                if (!camo)
                    camo = pl.gameObject.AddComponent<StealthCamo>();

                camo.enabled = true;
            }
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

            GameManager.Instance.disableInput();

            yield return 0f;

            MusicManager.Instance.fadeBGM(0.0f, 1.0f);

            if (mPlayerInstance)
            {
                while (mPlayerInstance.GetComponent<PlayerVoice>().voiceSource.isPlaying)
                    yield return 0f;
            }

            //An exception for race stages

            Racetrack r = FindObjectOfType<Racetrack>();

            if (r)
            {
                UIManager.Instance.fadeScreen(false, 0.25f, false);
                yield return Timing.WaitForSeconds(0.25f);

                mPlayerInstance.transform.position = mPlayerInstance.lastSafeGroundPosition;
                mPlayerInstance.getStatus().currentHealth = mPlayerInstance.getStatus().maxHealth;
                mPlayerInstance.changeCurrentMode(PlayerModes.NORMAL);
                GameManager.Instance.enablePlayerInput();
                GameManager.Instance.enableCameraInput();
                mPlayerInstance.tpc.centerBehindPlayer();
                mPlayerInstance.tpc.lockPosition = false;
                UIManager.Instance.fadeScreen(true, 0.25f, false);
                yield return Timing.WaitForSeconds(0.25f);

                yield break;
            }


            yield return Timing.WaitForSeconds(0.5f);

            GameManager.Instance.fadeGameAudio(false, 1.0f);
            UIManager.Instance.fadeScreen(false, 1.0f, false);


            yield return Timing.WaitForSeconds(1.1f);
            


            if (mPlayerInstance)
                GameObject.Destroy(mPlayerInstance.gameObject);


            if(type == PlayerKilled.DeathType.Falling || type == PlayerKilled.DeathType.Drowned || isPrologue())
            {
                getCurrentPlayerStatus().currentHealth -= getCurrentPlayerStatus().maxHealth * 0.25f;

                //Falling and drowning are "soft deaths", they don't instakill you.
                //If we still have health left, don't die, just respawn.
                if (getCurrentPlayerStatus().currentHealth > 0.0f || isPrologue())
                {
                    //An exception for the prologue because there's no bed to wake up to.
                    if (isPrologue())
                        getCurrentPlayerStatus().currentHealth = getCurrentPlayerStatus().maxHealth * 0.5f;

                    spawnPlayerAtCheckpoint();
                    yield return 0f;

                    yield return Timing.WaitForSeconds(0.5f);

                    mPlayerInstance.tpc.centerBehindPlayer();

                    yield return Timing.WaitForSeconds(0.5f);

                    UIManager.Instance.fadeScreen(true, 1.0f, false);
                    MusicManager.Instance.restartMusic();
                    MusicManager.Instance.fadeBGM(1.0f, 0.025f);
                    GameManager.Instance.fadeGameAudio(true, 1.0f);

                    yield return Timing.WaitForSeconds(1.1f);

                    GameManager.Instance.enableInput();
                    yield break;
                }

            }


            getCurrentPlayerStatus().currentHealth = getCurrentPlayerStatus().maxHealth * 0.1f;
            getCurrentPlayerStatus().currentMagic = getCurrentPlayerStatus().maxMagic * 0.1f;
            getCurrentPlayerStatus().sleepTimeLeft = Helper.minutesToSeconds(15.0f);

            if (type == PlayerKilled.DeathType.Corrupted)
            {
                getCurrentPlayerStatus().clearStatusEffects();
                getCurrentPlayerStatus().setStatusEffect(PlayerStatusFX.Sick);
                getCurrentPlayerStatus().sleepTimeLeft = Helper.minutesToSeconds(30.0f);
            }

            yield return 0f;

            while (GameManager.Instance.gameSFXVolume > -80.0f)
            {
                GameManager.Instance.gameSFXVolume = Mathf.Lerp(GameManager.Instance.gameSFXVolume, -81.0f, Time.unscaledDeltaTime * 3);

                yield return 0f;
            }

            if (ringBank > 0)
            {
                GameObject inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_ReduceMoneyScene);

                ReduceMoneyScene reduceMoney = inst.GetComponentInChildren<ReduceMoneyScene>();

                //Lose money for getting owned.
                CoroutineHandle ch = reduceMoney.startReduceMoney(50);

                while (ch.IsRunning)
                {
                    yield return 0f;
                }

                Destroy(inst);
            }

            processSleeping(getCurrentPlayerStatus());

            // if (currentCharacter == PlayableCharacter.Amy)
            //    currentCharacter = PlayableCharacter.Cream;
            // else if (currentCharacter == PlayableCharacter.Cream)
            //    currentCharacter = PlayableCharacter.Amy;

            GameManager.Instance.loadScene("SleepScreen");


            //Timing.RunCoroutine(setupWakeupScene(false));

        }

        public void createBlankSave()
        {
            ringBank = 0;
            days = 1;
            daysTilMenstruation = 13;
            currentCharacter = PlayableCharacter.Amy;


            //Progress flags
            hasHammer = false;
            hasCloth = false;
            hasSlingshot = false;

            universeNumber = Random.Range(0, int.MaxValue);

            AmyStatus = GameManager.getSystemData().AmyParams.baseStats.makeCopy();
            CreamStatus = GameManager.getSystemData().CreamParams.baseStats.makeCopy();

            storyFlags = new List<StoryFlag>();

        }

        

        //TODO: Replace this with intro cutscene and prologue area.
        public void startNewGame()
        {
            createBlankSave();

            GameManager.Instance.loadScene("DEMO_DevCommentaryScreen", true);
            //GameManager.Instance.loadScene("Jungle", true);

        }

        public void wakeupScene(bool whiteFade = false)
        {
            Timing.RunCoroutine(setupWakeupScene(whiteFade), gameObject);
        }
        
        public IEnumerator<float> setupWakeupScene(bool whiteFade)
        {
            yield return 0f;
            UIManager.Instance.fadeScreen(false, 0.5f, whiteFade);
            GameManager.Instance.fadeGameAudio(false, 0.5f);
            yield return Timing.WaitForSeconds(0.5f);


            yield return Timing.WaitForSeconds(3.0f);

            SceneManager.LoadScene("AmyRoom");
            yield return Timing.WaitForSeconds(1.0f);

            while (GameManager.Instance.gameSFXVolume < -0.01f)
            {
                GameManager.Instance.gameSFXVolume = Mathf.Lerp(GameManager.Instance.gameSFXVolume, 0.0f, Time.unscaledDeltaTime * 3);

                yield return 0f;
            }

            GameObject inst = null;

            if (currentCharacter == PlayableCharacter.Amy)
                inst = GameObject.Instantiate(GameManager.Instance.systemData.Cutscene_AmyWakeup);

            if(currentCharacter == PlayableCharacter.Cream)
                inst = GameObject.Instantiate(GameManager.Instance.systemData.Cutscene_CreamWakeup);

            if (inst == null)
                Debug.Log("BUT WHY?!?!!?");

            yield return Timing.WaitForSeconds(0.5f);

            UIManager.Instance.fadeScreen(true, 3.0f, whiteFade);
            GameManager.Instance.fadeGameAudio(true, 3.0f);
        }

        public void characterSwitch(PlayableCharacter newChar)
        {
            Timing.RunCoroutine(doCharacterSwitch(newChar));
        }

        IEnumerator<float> doCharacterSwitch(PlayableCharacter newChar)
        {
            GameManager.Instance.disableInput();

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

            GameManager.Instance.enableInput();
        }

        public void killEnemiesPowerup(Transform user, float range = 32.0f)
        {
            Timing.RunCoroutine(doKillEnemiesPowerup(user, range));
        }

        public IEnumerator<float> doKillEnemiesPowerup(Transform user, float range)
        {
            foreach(Enemy e in FindObjectsOfType<Enemy>())
            {
                if(Vector3.Distance(user.transform.position, e.transform.position) < range)
                {
                    Damage instaKill = new Damage();
                    instaKill.damageAmount = 9999;

                    e.takeDamage(instaKill);

                    yield return Timing.WaitForSeconds(Random.Range(0.2f, 1.0f));
                }
            }
        }


    }

}
