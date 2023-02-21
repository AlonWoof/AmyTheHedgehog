using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerSpringBounce : PlayerMode
	{

		public Vector3 springDirection = Vector3.up;
		public float springVelocityLeft = 0.0f;

		const float springVelDecay = 16.0f;
		const float airMoveSpeed = 4.0f;

		Vector3 desiredMovement;

		void OnEnable()
        {
			getBaseComponents();

			if (mPlayer.currentMode != PlayerModes.SPRING)
			{
				enabled = false;
				return;
			}

			mRigidBody.isKinematic = false;
			mRigidBody.velocity = Vector3.zero;
			mPlayer.clearAccel();
			mPlayer.clearSpeed();

			mPlayer.mAnimator.Play("Airborne");

		}

		// Start is called before the first frame update
		void Start()
	    {
			getBaseComponents();

			if (mPlayer.currentMode != PlayerModes.SPRING)
			{
				enabled = false;
				return;
			}
		}


	
	    // Update is called once per frame
	    void FixedUpdate()
	    {
			updateMovement();

			mPlayer.CalcVerticalVelocity();
			mPlayer.applyFriction();
			mPlayer.updatePosition();
			mPlayer.updateRotation();
		}

        private void Update()
        {
			//handleInput();
			mPlayer.checkStickPower();
			mPlayer.CalcSlope();
			//mPlayer.checkForJump();

		}

		void handleInput()
		{

			if (GameManager.Instance.playerInputDisabled)
				return;


			float h = InputFunctions.getLeftAnalogX();
			float v = InputFunctions.getLeftAnalogY();


			if (Mathf.Abs(h) == 0 && Mathf.Abs(v) == 0)
			{
				mPlayer.stickAngle = Vector3.zero;
				return;
			}

			Vector3 targetDirection = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1.0f);
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

			Vector3 camAngle = GameManager.Instance.mainCamera.transform.forward;
			camAngle.y = 0;
			camAngle.Normalize();

			mPlayer.stickAngle = (targetRotation * camAngle) * (targetDirection.magnitude);

			//Direction change penalty
			float dirChange = Vector3.Dot(mPlayer.direction, mPlayer.stickAngle.normalized);

			mPlayer.direction = mPlayer.stickAngle.normalized;

			if (mPlayer.acceleration.z < airMoveSpeed)
				mPlayer.acceleration.z += airMoveSpeed * Time.fixedDeltaTime;


			mPlayer.acceleration.z *= dirChange;

			if (dirChange < -0.5f)
				mPlayer.acceleration.z = Mathf.Lerp(mPlayer.acceleration.z, 0, 0.5f);
		}

		public void updateMovement()
		{
			Vector3 velo = springDirection * springVelocityLeft + (mPlayer.direction * mPlayer.acceleration.z);
			//mPlayer.setVelocityDirectly(velo);
			

			springVelocityLeft -= (springVelDecay * Time.fixedDeltaTime);

			mPlayer.acceleration.y = springVelocityLeft;

			mPlayer.groundNormal = springDirection;

			if (springVelocityLeft < 0.0f)
			{
				springVelocityLeft = 0.0f;
				mPlayer.changeCurrentMode(PlayerModes.NORMAL);
				//mPlayer.acceleration = mPlayer.WorldToPlayerSpace(mPlayer.speed);
				mPlayer.isOnGround = false;
			}
		}

		public void setSpringVelocity(Vector3 dir, float power)
		{
			mPlayer.clearAccel();
			mPlayer.clearSpeed();
			springDirection = dir;
			springVelocityLeft = power;
			//mPlayer.acceleration.y = 30.0f;

			mPlayer.acceleration.y = power;// (transform.rotation * (dir * power));
			mPlayer.speed += dir * power;
		}
	}
}
