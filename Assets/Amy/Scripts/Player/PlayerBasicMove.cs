using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerBasicMove : PlayerMode
	{

		public float idleCounter = 0.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
			getBaseComponents();
	    }
	
	    // Update is called once per frame
	    void Update()
	    {

			if (GameManager.Instance.gamePaused)
				return;

			handleInput();
			mPlayer.checkStickPower();
			mPlayer.CalcSlope();
			mPlayer.checkForJump();
			mPlayer.checkForInteract();


			if (mPlayer.mChara == PlayableCharacter.Amy)
			{
				mPlayer.checkForGroundAttack();
				mPlayer.checkForRunningGroundAttack();
				mPlayer.checkForHammerJump();
				mPlayer.checkForAirAttack();
				mPlayer.checkForSlingshot();
				
			}

			if(mPlayer.mChara == PlayableCharacter.Cream)
            {
				mPlayer.checkForFlying();
			}
		}

        private void FixedUpdate()
        {
			if (GameManager.Instance.gamePaused)
				return;

			mPlayer.CalcVerticalVelocity();
			mPlayer.applyFriction();
			mPlayer.updatePosition();
			mPlayer.updateRotation();
		}

        private void LateUpdate()
        {
			if (GameManager.Instance.gamePaused)
				return;

			mPlayer.checkIfUnderwater();


			idleCounter += Time.deltaTime;

			if (mPlayer.acceleration.magnitude > 0.01f)
				idleCounter = 0.0f;

			if (!mPlayer.isOnGround || mPlayer.isSliding)
				idleCounter = 0.0f;


			if (idleCounter > 5.0f)
			{
				PlayerStatus pstats = mPlayer.getStatus();

				if (pstats.currentMood == pstats.maxMood)
				{
					if (pstats.currentHealth < pstats.maxHealth)
					{
						pstats.currentHealth += (Time.deltaTime * 0.5f);
					}
				}
			}
		}

        void handleInput()
        {
			if (GameManager.Instance.playerInputDisabled)
				return;


			if (Input.GetButtonDown("LockOn") && idleCounter > 5.0f && mPlayer.areaDetector.getNearbyActorCount() == 0)
            {
				//if(mPlayer.getStatus().currentMood < mPlayer.getStatus().maxMood && mPlayer.acceleration.magnitude < 0.001f)
				// {
				if (PlayerManager.Instance.lastOrgasmCooldown <= 0.0f)
				{
					mPlayer.changeCurrentMode(PlayerModes.RUBBING);
				}
                //}
            }
		}


	}
}
