using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerFly : PlayerMode
	{

		public float start_altitude;

		const float max_height = 4.0f;

		public float max_fly = 5.0f;
		public float fly_left = 0.0f;

		Vector3 desiredVelo = Vector3.zero;

		//Special animation offset.
		float hoverOffset = 0.0f;

		float moveMult;

		Vector3 mDesiredMovement;
		Vector3 mCurrentMovement;
		

		// Start is called before the first frame update
		void Start()
	    {

		}

        private void OnEnable()
        {
			getBaseComponents();


			if (mPlayer.currentMode != PlayerModes.FLY)
			{
				enabled = false;
				return;
			}

			//mPlayer.clearAccel();
			//mPlayer.clearSpeed();

			//Unless we add Tails, Cream is the only one who can fly
			if (mPlayer.mChara != PlayableCharacter.Cream)
			{
				mPlayer.changeCurrentMode(PlayerModes.NORMAL);
			}

			//Though it would maybe be nice to have a cute boy in the mix... especially seeing that cute backsack when he flies~
			//But let's not get carried away, now. Two is already twice as much as I intended originally.
			start_altitude = transform.position.y;

			//mRigidBody.velocity = Vector3.Lerp(mRigidBody.velocity, 3.0f, 0.5f);
			//mRigidBody.velocity = Vector3.up * 3.0f;

			mPlayer.isOnGround = false;
			mPlayer.framesAirborne += 10;
			mPlayer.framesGrounded = 0;

			fly_left = max_fly;
			mAnimator.CrossFade("Fly_Basic", 0.25f);
			mPlayer.tpc.changeCameraMode(TPCMode.CreamFlying);

		}

		void handleInput()
		{

			if (GameManager.Instance.playerInputDisabled)
				return;

			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			float h = InputFunctions.getLeftAnalogX();
			float v = InputFunctions.getLeftAnalogY();


			if (Mathf.Abs(h) == 0 && Mathf.Abs(v) == 0)
			{
				mDesiredMovement = Vector3.zero;

				return;
			}

			Vector3 targetDirection = Vector3.ClampMagnitude(new Vector3(h, 0, v), 1.0f);
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

			Vector3 camAngle = GameManager.Instance.mainCamera.transform.forward;
			camAngle.y = 0;
			camAngle.Normalize();


			mDesiredMovement = (targetRotation * camAngle) * (targetDirection.magnitude);

			mPlayer.direction = mDesiredMovement.normalized;

		}

		private void LateUpdate()
		{

			Vector3 horizMovement = mRigidBody.velocity;
			float vertMovement = mRigidBody.velocity.y;

			horizMovement.y = 0.0f;

			//mAnimator.SetFloat("fly_speed", horizMovement.magnitude / 4.0f);
			//mAnimator.SetFloat("animSpeed", 1.0f + horizMovement.magnitude / 4.0f);
			//mAnimator.SetFloat("y_accel", vertMovement);

			mAnimator.SetFloat("fly_z", horizMovement.magnitude * 0.25f);
			mAnimator.SetFloat("fly_y", vertMovement * 0.3f);
			mAnimator.SetFloat("fly_speed", Mathf.Clamp(mRigidBody.velocity.magnitude * 0.5f, 1.0f, 4.0f));

			mPlayer.hipBoneTransform.position += hoverOffset * (Vector3.up * 0.1f);

		}

		private void FixedUpdate()
        {
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			calculateVerticalVelocity();
			groundedCheck();
			checkIfUnderwater();
		}

        void groundedCheck()
		{
			Vector3 start = transform.position + Vector3.up * 0.5f;
			Vector3 end = transform.position - Vector3.up * 0.1f;

			RaycastHit hitInfo = new RaycastHit();

			if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
			{
				mPlayer.clearSpeed();
				mPlayer.clearAccel();

				mPlayer.changeCurrentMode(PlayerModes.NORMAL);
				mPlayer.tpc.changeCameraMode(TPCMode.Normal);
			}
			else
            {
				//Fix for landing animation
				mPlayer.framesAirborne++;
            }
		}

		public void checkIfUnderwater()
		{

			if (mPlayer.getWaterDepth() >= mPlayer.headOffsetFromGround)
			{
				mPlayer.clearSpeed();
				mPlayer.clearAccel();

				mPlayer.changeCurrentMode(PlayerModes.SWIMMING);
				mPlayer.tpc.changeCameraMode(TPCMode.Normal);
			}
		}

		void calculateVerticalVelocity()
		{

			const float flyStaminaDrain = 0.05f;
			const float flySpeed = 4.0f;
			const float flySpeed_fast = 6.0f;

			Vector3 cpos = transform.position;
			Vector3 velo = mRigidBody.velocity;


			desiredVelo = (mPlayer.transform.forward * (mDesiredMovement.magnitude * flySpeed));

			if (mDesiredMovement.magnitude > 0.1f)
			{
				fly_left -= Time.fixedDeltaTime * mDesiredMovement.magnitude;
				mPlayer.getStatus().currentStamina -= (Time.fixedDeltaTime * mDesiredMovement.magnitude) * flyStaminaDrain;

				if (fly_left < 0.0f)
					fly_left = 0.0f;
			}

			if (cpos.y < start_altitude + max_height)
			{

				if (Input.GetButton("Jump"))
				{
					desiredVelo.y = 3.0f;
				}
			}


			if (!Input.GetButton("Jump") || fly_left <= 0.0f)
			{
				desiredVelo.y = -8.0f;

				if (fly_left > 0.1f)
					desiredVelo.y = -4.0f;
			}

			if(Input.GetButtonDown("Attack"))
            {
				mPlayer.changeCurrentMode(PlayerModes.BUTTSLAM);
				return;
            }

			if (!GameManager.Instance.usingController)
			{
				if (Input.GetButtonDown("Attack"))
				{
					mPlayer.changeCurrentMode(PlayerModes.BUTTSLAM);
					return;
				}
			}
			else
			{
				if (Input.GetAxis("Shoot") > 0.5f)
				{
					mPlayer.changeCurrentMode(PlayerModes.BUTTSLAM);
					return;
				}
			}


			mPlayer.setVelocityDirectly(Vector3.Lerp(mRigidBody.velocity, desiredVelo, Time.deltaTime * 2.0f));
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mPlayer.direction, Vector3.up), Time.deltaTime * 3.0f);

		}

		// Update is called once per frame
		void Update()
	    {
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			handleInput();

			Quaternion newRot = Quaternion.LookRotation(mPlayer.direction);
			Quaternion forwardLean = newRot * (Quaternion.FromToRotation(Vector3.forward, transform.forward) * Quaternion.Euler(new Vector3(20, 0, 0)));

			Quaternion finalRot = Quaternion.Lerp(newRot, forwardLean, mDesiredMovement.magnitude);

			transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 4.0f);

			hoverOffset = Mathf.Sin(Time.time);
		}


	}
}
