using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AIPlayer : Player
	{

		public float virtualAnalogX = 0.0f;
		public float virtualAnalogY = 0.0f;

		public float desiredVirtualAnalogX = 0.0f;
		public float desiredVirtualAnalogY = 0.0f;

		public bool virtualJumpDown = false;
		public bool virtualJumpHeld = false;

		public bool virtualAttackDown = false;
		public bool virtualAttackHeld = false;

		public void handleVirtualButtons()
        {
			virtualJumpDown = false;
			virtualAttackDown = false;

			virtualAnalogX = Mathf.Lerp(virtualAnalogX, desiredVirtualAnalogX, 0.25f);
			virtualAnalogY = Mathf.Lerp(virtualAnalogY, desiredVirtualAnalogY, 0.25f);
		}

		public void pressJump()
        {
			virtualJumpDown = true;
			virtualJumpHeld = true;
        }

		public void pressAttack()
        {
			virtualAttackDown = true;
			virtualAttackHeld = true;
        }

		public void releaseJump()
        {
			virtualJumpHeld = false;
        }

		public void releaseAttack()
        {
			virtualAttackHeld = false;
        }

		public void testFollowFunction()
        {
			virtualAnalogX = 0.0f;
			virtualAnalogY = 0.0f;
			virtualJumpDown = false;


			Player pl = PlayerManager.Instance.mPlayerInstance;

			if (!pl)
				return;

			Vector3 mpos = transform.position;
			Vector3 tpos = pl.transform.position;

			if (tpos.y - mpos.y > 2)
            {
				if(!virtualJumpDown)
					virtualJumpDown = true;

				virtualJumpHeld = true;
            }
			else if(tpos.y - mpos.y > 0.8f && virtualJumpDown)
            {
				virtualJumpHeld = true;
			}

			if (Vector3.Distance(mpos,tpos) > 2.0f || pl.acceleration.magnitude > 1.0f)
            {

				mpos.y = 0;
				tpos.y = 0;
				Vector3 dir = Helper.getDirectionTo(mpos, tpos);

				virtualAnalogY = dir.z;
				virtualAnalogX = dir.x;
			}

			

        }

		public override void checkStickPower()
		{

			if (stickTimeout > 0.0f)
			{
				stickTimeout -= Time.deltaTime;
				return;
			}

			stickPower = 0.0f;


			if (GameManager.Instance.playerInputDisabled)
				return;

			float h = virtualAnalogX;
			float v = virtualAnalogY;

			//h = InputFunctions.getLeftAnalogX();
			//v = InputFunctions.getLeftAnalogY();

			if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
				return;

			stickAngle = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1.0f);
			stickPower = stickAngle.magnitude;

			//Debug.Log(stickPower);

			Vector3 targetDirection = stickAngle;
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

			//Vector3 camAngle = GameManager.Instance.mainCamera.transform.forward;
			Vector3 camAngle = Camera.main.transform.forward;
			camAngle.y = 0;
			camAngle.Normalize();

			Vector3 mDir = (targetRotation * Vector3.forward) * (targetDirection.magnitude);

			//mPlayer.acceleration.z += ((mDir * mPlayer.mParam.forwardAccel) * Time.deltaTime);

			float slopePenalty = Mathf.Clamp(slopeAmount, 0, 2.0f);

			prev_direction = direction;
			direction = mDir.normalized;

			float turningFactor = (1 - Vector3.Dot(direction.normalized, prev_direction.normalized)) * Helper.AngleDir(direction.normalized, prev_direction.normalized, transform.up);

			turningFactor *= 16.0f;

			if (Mathf.Abs(turningFactor) > 0.01f)
				leanAmount += turningFactor * 1.5f;

			leanAmount = Mathf.Clamp(leanAmount, -1.0f, 1.0f);

			//if(Mathf.Abs(turningFactor) > 0.5f)
			//Debug.Log("TURNING: " + turningFactor);

			//Debug.Log(Vector3.Dot(direction, prev_direction));

			float dirChange = Mathf.Clamp01(Vector3.Dot(direction, prev_direction));



			float forward_accel = (targetDirection.magnitude * mParam.forwardAccel);
			forward_accel += (slopeAmount * mParam.forwardAccel);

			if (getStatus().checkStatusEffect(PlayerStatusFX.Tired))
				forward_accel *= 0.48f;

			//forward_accel *= dirChange;

			if (isOnGround)
				acceleration.z *= dirChange;

			acceleration.z += forward_accel * Time.deltaTime;

			if (PlayerManager.Instance.isSmallRoom)
				acceleration.z = Mathf.Clamp(acceleration.z, 0, 1.5f);
		}

		public override void checkForJump()
		{

			if (virtualJumpDown && canJump(false))
				Jump(false);

			if (!virtualJumpHeld && !isHammerJumping && framesAirborne > 5)
				jumpTimer = 0.0f;

			if (jumpTimer > 0.0f)
				jumpTimer -= Time.deltaTime;
		}

		public override void checkForFlying()
		{
			if (GameManager.Instance.playerInputDisabled || GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			if (getAltitudeFromGround() > mParam.height && mChara == PlayableCharacter.Cream)
			{
				if (virtualJumpDown)
				{
					//mVoice.playVoiceDelayed(Random.Range(0.05f, 0.1f), mVoice.jumping);

					changeCurrentMode(PlayerModes.FLY);
				}
			}
		}


		public override void checkForButtSlamAttack()
		{
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (isOnGround)
				return;

			if (framesAirborne < 10)
				return;

			if (virtualAttackDown)
				changeCurrentMode(PlayerModes.BUTTSLAM);

		}

		public override void checkForGroundAttack()
		{
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			if (acceleration.magnitude > 1.0f || !isOnGround)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			if (virtualAttackDown)
				groundAttack();
		}

		public override void checkForRunningGroundAttack()
		{
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			if (acceleration.magnitude < 1.0f || !isOnGround)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			if (!virtualAttackHeld && hammerJumpCharge > 0.01f && hammerJumpCharge < 0.9f)
			{
				runningGroundAttack();
				hammerJumpCharge = 0.0f;
			}
		}

		public override void checkForEarSpinAttack()
		{
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			if (acceleration.magnitude < 3.0f || !isOnGround)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			//Get this poor girl some rest jeez...
			if (getStatus().checkStatusEffect(PlayerStatusFX.Tired))
				return;


			if (virtualAttackDown)
			{
				earSpinAttack();
			}
		}

		public override void changeCurrentMode(PlayerModes newMode)
		{
			if (newMode == currentMode)
				return;

			//Get rid of modes banned for AI
			newMode = verifyMode(newMode);

			lastMode = currentMode;
			currentMode = newMode;

			refreshMode();
			updateExpression();
			isBallMode = false;

			if (mChara == PlayableCharacter.Cream)
				updateEars();
		}

		public PlayerModes verifyMode(PlayerModes newMode)
        {
			if (newMode == PlayerModes.KILLED)
			{
				//modeKilled.enabled = false;
				//newMode = PlayerModes.HURT;
			}

			if (newMode == PlayerModes.FIRSTPERSON)
			{
				newMode = PlayerModes.NORMAL;
			}


			return newMode;
        }

		public override void checkForInteract()
        {
			
        }

		public override void checkForAirAttack()
		{
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			if (!canAirAttack)
				return;

			//Get this poor girl some rest jeez...
			if (getStatus().checkStatusEffect(PlayerStatusFX.Tired))
				return;

			//if (isAttacking && !isOnGround)
			//updateHoming();

			if (virtualAttackDown)
				airHammerAttack();
		}

		protected override void debugControls()
		{

			return;

			if (!GameManager.Instance.debugMode)
				return;

			if (Input.GetKeyDown(KeyCode.Keypad5))
			{

				if (currentMode == PlayerModes.DEBUG_MOVE)
				{
					changeCurrentMode(PlayerModes.NORMAL);
				}
				else
				{
					changeCurrentMode(PlayerModes.DEBUG_MOVE);
				}
			}

			if (Input.GetKeyDown(KeyCode.Keypad7))
			{


				GameObject dSource = new GameObject("Dmg");

				Damage dmg = dSource.AddComponent<Damage>();

				dSource.transform.position = transform.position + Vector3.up * 0.5f + transform.forward;

				dmg.damageAmount = 5;
				dmg.damageType = DamageType.Neutral;
				dmg.source = dSource;

				takeDamage(dmg);

			}

			if (Input.GetKeyDown(KeyCode.Keypad9))
			{
				if (currentMode == PlayerModes.NORMAL)
				{
					changeCurrentMode(PlayerModes.RUBBING);
				}
			}
		}

	}
}
