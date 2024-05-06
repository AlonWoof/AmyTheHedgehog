using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerHurt : PlayerMode
	{

		public float knockPower = 0.0f;
		public Vector3 knockOrigin;

		float hurtAnimTimer = 0.75f;
		float verticalVelocity;
		int framesAirborne = 0;


		private void Awake()
		{
			getBaseComponents();
		}

		private void OnEnable()
		{
			if (mPlayer.currentMode != PlayerModes.HURT)
			{
				enabled = false;
				return;
			}

			mRigidBody.isKinematic = false;
			//grounderIK.enabled = false;
			//mPlayer.isGrounded = false;
			mPlayer.isOnGround = false;
			verticalVelocity = 0.0f;
			framesAirborne = 0;

		}

		public void setKnockBackDirectional(Vector3 dir, float power = 5.0f)
        {
			knockPower = power;
			knockOrigin = (transform.position - dir) + (Vector3.up * 2.0f);
			mPlayer.acceleration.y = power * 0.65f;
			hurtAnimTimer = 0.25f;


			mAnimator.Play("Hurt");
		}

		public void setKnockBack(Vector3 origin, float power = 5.0f)
		{


			knockPower = power;
			knockOrigin = origin;
			mPlayer.acceleration.y = power * 0.65f;
			hurtAnimTimer = 0.25f;


			Vector3 dir = Helper.getDirectionTo(transform.position, knockOrigin);
			dir.y = 0.0f;
			mPlayer.setAngleInstantly(dir.normalized);

			mAnimator.Play("Hurt");
		}

		// Update is called once per frame
		void LateUpdate()
		{
			if (mPlayer.currentMode != PlayerModes.HURT)
				return;

			if (GameManager.Instance.gamePaused)
				return;

			Vector3 dir = Helper.getDirectionTo(transform.position, knockOrigin);
			dir.y = 0.0f;

			mPlayer.direction = dir.normalized;

			if (hurtAnimTimer > 0.0f)
				hurtAnimTimer -= Time.deltaTime;

			if (knockPower > 0.5f)
			{
				//mPlayer.acceleration.y = knockPower * 0.25f;
			}

			mPlayer.acceleration.z = -knockPower;
			

			float knockDecay = 8.0f;

			checkForKillTrigger();

			if (knockPower > 0.0f)
			{
				knockPower -= (knockDecay * Time.fixedDeltaTime);
			}

			knockPower = Mathf.Clamp(knockPower, 0.0f, 128.0f);

			mPlayer.groundNormal = Vector3.up;

			updateIsGrounded();
		}


		private void FixedUpdate()
		{

			if (GameManager.Instance.gamePaused)
				return;

			mPlayer.CalcVerticalVelocity();
			mPlayer.applyFriction();
			mPlayer.updatePosition();
			mPlayer.updateRotation();
			updateIsGrounded();
		}

		void updateIsGrounded()
		{

			Vector3 start = transform.position + (0.25f * transform.up);
			Vector3 end = transform.position - (transform.up * 0.15f);
			RaycastHit hitInfo = new RaycastHit();

			Debug.DrawLine(start, end, Color.cyan, 0.1f);

			float angle = Vector3.Angle(Vector3.up, mPlayer.groundNormal);

			

			if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
			{
				float groundAngle = Vector3.Angle(Vector3.up, hitInfo.normal);

				//Debug.Log("Floop Angle: " + groundAngle);

				if (groundAngle > 30)
					return;


				if (!mPlayer.isOnGround && knockPower < 0.1f)
				{
					//mPlayer.isBallMode = false;
					mPlayer.changeCurrentMode(PlayerModes.NORMAL);
					mAnimator.Play("Hurt_Land");

					knockPower *= 0.9f;
					mPlayer.acceleration *= 0.5f;

					mPlayer.isOnGround = true;
				}

				if(hurtAnimTimer < 0.01f)
                {
					mPlayer.changeCurrentMode(PlayerModes.NORMAL);
					mAnimator.Play("Hurt_Land");
					mPlayer.isOnGround = true;
				}



				mPlayer.groundNormal = hitInfo.normal;
				framesAirborne = 0;

			}
			else
			{
				framesAirborne++;

				if (framesAirborne > 5)
				{
					if (mPlayer.isOnGround)
					{

						mPlayer.isOnGround = false;
						mPlayer.groundNormal = Vector3.up;

					}
				}
			}

		}

		//Because this pissed me off SO MUCH in Sonic Heroes...
		void checkForKillTrigger()
        {
			Vector3 yeetDirection = -transform.forward;

			Vector3 start = transform.position + yeetDirection * 5.0f;
			Vector3 end = start + (Vector3.down * 128.0f);

			RaycastHit hitInfo = new RaycastHit();

			Debug.DrawLine(start, end, Color.yellow, 1.0f);

			if(Physics.Linecast(start, end, out hitInfo))
            {
				if(hitInfo.collider.GetComponentInChildren<FallingKillPlane>())
                {
					knockPower *= 0.95f;

					Debug.DrawLine(start, end, Color.red, 10.0f);
				}
            }
        }

		// Update is called once per frame
		void Update()
	    {
	        
	    }
	}
}
