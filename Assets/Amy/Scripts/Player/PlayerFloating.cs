using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerFloating : PlayerMode
	{

		public float floatingTimer = 0.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
			getBaseComponents();
		}

        private void OnEnable()
        {
			getBaseComponents();

			if (mPlayer.currentMode != PlayerModes.FLOATING)
			{
				enabled = false;
				return;
			}

			mAnimator.CrossFade("Floating", 0.1f);
			floatingTimer = 1.0f;
		}

		void groundedCheck()
		{
			Vector3 start = transform.position + Vector3.up * 0.5f;
			Vector3 end = transform.position - Vector3.up * 0.1f;

			RaycastHit hitInfo = new RaycastHit();

			if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
			{
				//mPlayer.clearSpeed();
				//mPlayer.clearAccel();

				mPlayer.transform.position = hitInfo.point;
				mPlayer.resetGroundFlags();
				mAnimator.Play("Land");
				mPlayer.changeCurrentMode(PlayerModes.NORMAL);
				mPlayer.tpc.changeCameraMode(TPCMode.Normal);
			}
			else
			{
				//Fix for landing animation
				mPlayer.framesAirborne++;
				mPlayer.framesGrounded = 0;
				mPlayer.isOnGround = false;
			}
		}

		// Update is called once per frame
		void Update()
	    {
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			mPlayer.checkStickPower();
			mPlayer.CalcSlope();

			if (floatingTimer > 0.0f)
				floatingTimer -= Time.deltaTime;
		}

		void calcVerticalVelocity()
        {
			float gravityMult = mPlayer.mParam.gravityMult;
			float verticalVelocity = mPlayer.acceleration.y;

			verticalVelocity = Mathf.Lerp(verticalVelocity, Physics.gravity.y * gravityMult, Time.fixedDeltaTime * 1.5f);

			mPlayer.acceleration.y = verticalVelocity;

			if (Mathf.Abs(mPlayer.acceleration.y) < 0.01f)
				mPlayer.acceleration.y = 0.0f;

			if (Mathf.Abs(mPlayer.speed.y) < 0.01f)
				mPlayer.speed.y = 0.0f;
		}


		private void FixedUpdate()
		{
			if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
				return;

			calcVerticalVelocity();
			mPlayer.applyFriction();
			mPlayer.updatePosition();
			mPlayer.updateRotation();

			if(floatingTimer < 0.01f)
				groundedCheck();
		}
	}
}
