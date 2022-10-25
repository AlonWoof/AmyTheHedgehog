using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
namespace Amy
{
    public enum PlayableCharacter
    {
        Amy,
        Cream
    }


    [System.Serializable]
    public class PlayerStatus
    {
        public float currentHealth = 1.0f;
        public float currentMood = 1.0f;

        public float speedBonus = 0.0f;

        public float jumpTimeBonus = 0.0f;
        public float jumpPowerBonus = 0.0f;

    }


	public class PlayerManager : Singleton<PlayerManager>
	{

        //The current player instance.
        public Player mPlayerInstance;

        //The player's current status. Not sure how much of this should be on the player instance.
        public PlayerStatus status;

        public PlayableCharacter currentCharacter = PlayableCharacter.Amy;

        //This will be placed at the last safe place/exit
        public Transform playerCheckpoint;

        public int lastExit = 0;

        public float playerDirtiness = 0.0f;
        public int ringCount = 0;
        public float stealthIndex = 0.0f;

        public bool playerHasStealthCamo = false;

        

        private void Awake()
        {
            status = new PlayerStatus();

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

        void handleStealthIndex()
        {
            if (!mPlayerInstance)
                return;

            PlayerBasicMove basicMove = mPlayerInstance.GetComponent<PlayerBasicMove>();

            stealthIndex = 0.0f;

            if (playerHasStealthCamo)
                stealthIndex = 1.0f;

            if (basicMove.jumpTimer > 0.2f)
                stealthIndex -= 0.25f;
        }

        public void givePlayerStealthCamo()
        {
            if (!mPlayerInstance)
                return;

            if (!mPlayerInstance.GetComponent<StealthCamo>())
                mPlayerInstance.gameObject.AddComponent<StealthCamo>();

            mPlayerInstance.GetComponent<StealthCamo>().enabled = true;
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

            //getDOFObjects();

            if (mPlayerInstance != null)
            {
                // Debug.Log("Destroying duplicate player...");
                // Destroy(mPlayerInstance.gameObject);
                mPlayerInstance.transform.position = playerCheckpoint.transform.position;
                mPlayerInstance.mDirection = playerCheckpoint.transform.forward;
                return mPlayerInstance;
            }


            mPlayerInstance = Player.Spawn(playerCheckpoint.transform.position, playerCheckpoint.transform.forward, currentCharacter);
            
            //saveGame.lastScene = SceneManager.GetActiveScene().name;
            // saveGame.lastExit = lastExit;

            return mPlayerInstance;
        }

    }

}
