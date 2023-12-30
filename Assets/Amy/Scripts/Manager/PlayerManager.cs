using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

/* Copyright 2021 Jason Haden */
namespace Amy
{
    public enum PlayableCharacter
    {
        Amy,
        Cream,
        MAX
    }


    [System.Serializable]
    public class PlayerStatus
    {
        public float currentHealth = 25.0f;
        public float maxHealth = 25.0f;
        public float currentMood = 25.0f;
        public float maxMood = 25.0f;

        public float speedBonus = 0.0f;

        public float jumpTimeBonus = 0.0f;
        public float jumpPowerBonus = 0.0f;

        public float lungCapacity = 20.0f;
        public float dirtiness = 0.0f;


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

            return ns;
        }
    }

    [System.Serializable]
    public class ProgressData
    {
        public bool hasHammer = false;
        public bool hasCloth = false;
        public bool hasSlingshot = false;
    }

    public class PlayerManager : Singleton<PlayerManager>
	{

        //The current player instance.
        public Player mPlayerInstance;

        //The player's current status. Not sure how much of this should be on the player instance.
        public PlayerStatus AmyStatus;
        public PlayerStatus CreamStatus;

        //Unlockable/Story progression
        public ProgressData progress;

        public PlayableCharacter currentCharacter = PlayableCharacter.Amy;

        //This will be placed at the last safe place/exit
        public Transform playerCheckpoint;

        public int lastExit = 0;

        public float playerDirtiness = 0.0f;
        public float playerStress = 0.0f;

        public int ringBank = 0;
        int ringCount = 0;
        public float stealthIndex = 0.0f;

        public bool playerHasStealthCamo = false;

        

        private void Awake()
        {

            AmyStatus = GameManager.getSystemData().AmyParams.baseStats.makeCopy();
            CreamStatus = GameManager.getSystemData().CreamParams.baseStats.makeCopy();

            GameObject inst = new GameObject("CHECKPOINT");
            DontDestroyOnLoad(inst);
            playerCheckpoint = inst.transform;
        }

        public void Init()
        {
            Debug.Log("PlayerManager Initialized!");
        }

        // Start is called before the first frame update
        void Start()
    	{

    	}

    	// Update is called once per frame
    	void Update()
    	{
            handleStealthIndex();
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
                // Debug.Log("Destroying duplicate player...");
                // Destroy(mPlayerInstance.gameObject);
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

        /*
        public void PlayerDieRespawn(PlayerDie.DeathType type)
        {
            Timing.RunCoroutine(doPlayerRespawnSequence(type));
        }
        */

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
        /*
        //I would give this a more fitting name but I want to pay homage to SA1's hilarious kill function name (killHimP())
        public void killHer()
        {
            if (!mPlayerInstance)
                return;

            mPlayerInstance.GetComponent<PlayerDie>().deathType = PlayerDie.DeathType.Normal;
            mPlayerInstance.changeCurrentMode(PlayerModes.DIE);
        }


        //TODO: add different respawn situations.
        public IEnumerator<float> doPlayerRespawnSequence(PlayerDie.DeathType type)
        {
           
            GameManager.Instance.cameraInputDisabled = true;
            GameManager.Instance.playerInputDisabled = true;

            yield return 0f;

            MusicManager.Instance.fadeBGM(0.0f, 1.0f);

            while (mPlayerInstance.GetComponent<PlayerVoice>().voiceSource.isPlaying)
                yield return 0f;
            
            yield return Timing.WaitForSeconds(0.5f);

            UIManager.Instance.fadeScreen(false, 1.0f, false);


            yield return Timing.WaitForSeconds(1.1f);
            MusicManager.Instance.restartMusic();
            MusicManager.Instance.fadeBGM(1.0f, 0.01f);

            mPlayerInstance.transform.position = playerCheckpoint.transform.position;
            mPlayerInstance.transform.rotation = playerCheckpoint.transform.rotation;

            //Lose money for getting owned.
            subtractRings(35);

            getCurrentPlayerStatus().currentHealth = Mathf.Lerp(0.25f, 0.9f, getCurrentPlayerStatus().currentMood);
            getCurrentPlayerStatus().currentMood = Mathf.Clamp(getCurrentPlayerStatus().currentMood -= Random.Range(0.16f,0.3f),0.0f,1.0f);
            

            GameObject.Destroy(mPlayerInstance.gameObject);
            yield return 0f;

            PlayerManager.Instance.spawnPlayerAtCheckpoint();

            yield return Timing.WaitForSeconds(2.0f);

            UIManager.Instance.fadeScreen(true, 1.0f, false);

            yield return Timing.WaitForSeconds(1.1f);

            GameManager.Instance.playerInputDisabled = false;
            GameManager.Instance.cameraInputDisabled = false;

        }
        */

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
