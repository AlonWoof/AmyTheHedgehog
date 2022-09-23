using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
namespace Amy
{
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

        //This will be placed at the last safe place/exit
        public Transform mLastCheckPoint;

        private void Awake()
        {
            status = new PlayerStatus();
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
    	    
    	}
	}

}
