using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerBasicMove : PlayerMode
	{

		public float idleCounter = 0.0f;
		public float groundedTimer = 0.0f;

		public GameObject dbg_safePos;

	    // Start is called before the first frame update
	    void Start()
	    {
			getBaseComponents();

			dbg_safePos = new GameObject("SafeSphere");

		}

        private void OnEnable()
        {
			getBaseComponents();

			if (mPlayer.currentMode != PlayerModes.NORMAL)
            {
				enabled = false;
				return;
            }

			idleCounter = 0.0f;
        }

        // Update is called once per frame
        void Update()
	    {

			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			handleInput();
			mPlayer.checkStickPower();
			mPlayer.CalcSlope();
			mPlayer.checkForJump();
			


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
				mPlayer.checkForEarSpinAttack();
				mPlayer.checkForButtSlamAttack();
			}
		}

        private void FixedUpdate()
        {
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			mPlayer.CalcVerticalVelocity();
			mPlayer.applyFriction();
			mPlayer.updatePosition();
			mPlayer.updateRotation();
		}

        private void LateUpdate()
        {
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			mPlayer.checkIfUnderwater();


			idleCounter += Time.deltaTime;
			groundedTimer += Time.deltaTime;

			if (mPlayer.acceleration.magnitude > 0.01f)
				idleCounter = 0.0f;

			if (!mPlayer.isOnGround || mPlayer.isSliding)
				idleCounter = 0.0f;


			if (!mPlayer.isOnGround)
				groundedTimer = 0.0f;
		}



        void handleInput()
        {
			if (GameManager.Instance.playerInputDisabled)
				return;

			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			if (groundedTimer > 5.0f)
			{
				mPlayer.lastSafeGroundPosition = transform.position;
				dbg_safePos.transform.position = mPlayer.lastSafeGroundPosition;
			}

			if(idleCounter > 0.5f)
            {
				if (Input.GetButtonDown("View"))
				{
					mPlayer.changeCurrentMode(PlayerModes.FIRSTPERSON);
				}
			}

			float masturbateTime = 60.0f;

			if (mPlayer.getStatus().checkStatusEffect(PlayerStatusFX.Horny))
				masturbateTime = 10.0f;

			if (mPlayer.getStatus().currentHealth < mPlayer.getStatus().maxHealth * 0.75f)
				masturbateTime = 10.0f;

			if (idleCounter > masturbateTime)
            {
				if (mPlayer.modeRubbing.canMasturbate())
				{
					UIManager.Instance.contextButton.setActionText("Rub It?");
					if (Input.GetButtonDown("Action"))
					{
						if(mPlayer.modeRubbing.shouldMasturbate())
							mPlayer.changeCurrentMode(PlayerModes.RUBBING);
					}
				}
            }
		}


	}
}
