using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
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

			//Unless we add Tails, Cream is the only one who can fly
			if (mPlayer.mChara != PlayableCharacter.Cream)
			{
				mPlayer.changeCurrentMode(PlayerModes.NORMAL);
			}

			//Though it would maybe be nice to have a cute boy in the mix... especially seeing that cute backsack when he flies~
			//But let's not get carried away, now. Two is already twice as much as I intended originally.

			start_altitude = transform.position.y;

			mRigidBody.velocity = Vector3.Lerp(mRigidBody.velocity, Vector3.zero, 0.5f);

			fly_left = max_fly;
			mAnimator.CrossFade("Fly_Basic", 0.25f);

		}

		void handleInput()
		{

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

			horizMovement.y = 0.0f;

			mAnimator.SetFloat("fly_speed", horizMovement.magnitude / 4.0f);
			mAnimator.SetFloat("animSpeed", 1.0f + horizMovement.magnitude / 4.0f);

			mPlayer.hipBoneTransform.position += hoverOffset * (Vector3.up * 0.1f);
		}

		private void FixedUpdate()
        {
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
				mPlayer.changeCurrentMode(PlayerModes.NORMAL);
			}
		}

		public void checkIfUnderwater()
		{

			if (mPlayer.getWaterDepth() >= mPlayer.headOffsetFromGround)
			{
				mPlayer.changeCurrentMode(PlayerModes.SWIMMING);
			}
		}

		void calculateVerticalVelocity()
		{
			Vector3 cpos = transform.position;
			Vector3 velo = mRigidBody.velocity;

			desiredVelo = (mPlayer.transform.forward * (mDesiredMovement.magnitude * 4.0f));

			if (mDesiredMovement.magnitude > 0.1f)
			{
				fly_left -= Time.fixedDeltaTime * mDesiredMovement.magnitude;

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

			mPlayer.setVelocityDirectly(Vector3.Lerp(mRigidBody.velocity, desiredVelo, Time.fixedDeltaTime * 2.0f));
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mPlayer.direction, Vector3.up), Time.deltaTime * 3.0f);

		}

		// Update is called once per frame
		void Update()
	    {
			handleInput();

			Quaternion newRot = Quaternion.LookRotation(mPlayer.direction);
			Quaternion forwardLean = newRot * (Quaternion.FromToRotation(Vector3.forward, transform.forward) * Quaternion.Euler(new Vector3(20, 0, 0)));

			Quaternion finalRot = Quaternion.Lerp(newRot, forwardLean, mDesiredMovement.magnitude);

			transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 4.0f);

			hoverOffset = Mathf.Sin(Time.time);
		}
	}
}
