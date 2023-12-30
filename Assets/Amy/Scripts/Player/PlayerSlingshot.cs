using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{


	public class PlayerSlingshot : PlayerMode
	{

		public enum state
		{
			READY,
			AIM,
			FIRE,
			RELOAD
		}


		public float aim_y;
		public float aim_x;

		public SlingshotAimer aimer;
		public state currentState;

		Transform weapon_node;

		Vector3 bulletStartPos;

		LayerMask colMask;

		float moveTimeout = 0.0f;
		float timeOut = 0.5f;
		const float maxTimeOut = 1.0f;

		float afterFireTimeout = 0.0f;
		float quickReloadTimeout = 0.0f;

		bool ammoLoaded = false;
		bool leftSide = false;

	    // Start is called before the first frame update
	    void Start()
	    {
			colMask = LayerMask.GetMask("Collision");

			getBaseComponents();

			if (!aimer)
			{
				aimer = GetComponentInChildren<SlingshotAimer>();
			}

			if(!weapon_node)
            {
				weapon_node = mPlayer.getBoneByName("weapon");
            }

			if (mPlayer.currentMode != PlayerModes.SLINGSHOT)
			{
				enabled = false;
				aimer.gameObject.SetActive(false);
				aimer.slingshot_model.SetActive(false);
				return;
			}

		}

        private void OnEnable()
        {
			getBaseComponents();

			if (!aimer)
			{
				aimer = GetComponentInChildren<SlingshotAimer>(true);
			}

			if (mPlayer.currentMode != PlayerModes.SLINGSHOT)
			{
				enabled = false;
				aimer.slingshot_model.SetActive(false);
				return;
			}

			GameManager.Instance.changeCameraBlendMode(GameManager.blend_mode.fast);

			mAnimator.Play("Slingshot_Start");



			aimer.slingshot_model.SetActive(true);

			aim_x = (Quaternion.LookRotation(transform.forward).eulerAngles.y);
			aim_y = 0;

			if(Input.GetAxis("Shoot") > 0.1f)
            {
				currentState = state.AIM;
				ammoLoaded = true;
				GrabSling();
            }
			else
            {
				currentState = state.READY;
				ReleaseSling();
            }

			aimer.gameObject.SetActive(true);
			moveTimeout = 0.75f;
			timeOut = 0.5f;
		}

        private void OnDisable()
        {
			aimer.gameObject.SetActive(false);
			aimer.slingshot_model.SetActive(false);
		}

        private void Update()
        {
			handleInput();

			Quaternion dirRot = Quaternion.LookRotation(mPlayer.direction.normalized, Vector3.up);
			transform.rotation = Quaternion.Lerp(transform.rotation, dirRot, 0.5f);

			if(timeOut > 0.0f)
            {
				timeOut -= Time.deltaTime;

			}
		}

        // Update is called once per frame
        void FixedUpdate()
	    {
			if(!aimer)
            {
				aimer = GetComponentInChildren<SlingshotAimer>(true);

				return;
            }

			
			if(leftSide)
            {
				aimer.cam_node.transform.localPosition = Vector3.Lerp(aimer.cam_node.transform.localPosition, new Vector3(-0.5f, 0, -0.75f), 0.125f);
			}
			else
            {
				aimer.cam_node.transform.localPosition = Vector3.Lerp(aimer.cam_node.transform.localPosition, new Vector3(0.5f, 0, -0.75f), 0.125f);
			}

			aimer.transform.rotation = Quaternion.Euler(-aim_y, aim_x, 0);

			Vector3 lookDir = aimer.transform.forward;
			lookDir.y = 0;

			mPlayer.direction = lookDir.normalized;
			mPlayer.updateRotation();

			Quaternion dirRot = Quaternion.LookRotation(mPlayer.direction.normalized, Vector3.up);

			transform.rotation = Quaternion.Lerp(transform.rotation, dirRot, 0.5f);

			calcAimTarget();

			if(afterFireTimeout > 0.0f)
            {
				afterFireTimeout -= Time.fixedDeltaTime;
            }

			if (quickReloadTimeout > 0.0f)
            {
				quickReloadTimeout -= Time.fixedDeltaTime;
			}

			mAnimator.SetFloat("aim_y", (aim_y/60.0f));
			mAnimator.SetFloat("shoot", Input.GetAxis("Shoot"));

			mPlayer.CalcVerticalVelocity();
			mPlayer.applyFriction();
			mPlayer.updatePosition();
			mPlayer.CalcSlope();

			//To avoid getting all disoriented.
			mPlayer.tpc.centerBehindPlayer();
	    }

		void calcAimTarget()
        {
			bulletStartPos = weapon_node.transform.position + weapon_node.transform.up * 0.1f;

			Vector3 targetPos;

			RaycastHit hitInfo = new RaycastHit();

			if (Physics.Linecast(aimer.transform.position + aimer.transform.forward * 0.5f, aimer.transform.position + (aimer.transform.forward * 64.0f), out hitInfo, colMask))
            {
				targetPos = hitInfo.point;
            }
			else
            {
				targetPos = aimer.transform.position + aimer.transform.forward * 64.0f;

			}


			aimer.look_node.transform.position = aimer.transform.position + aimer.transform.forward * 64.0f;
			aimer.target_node.transform.position = targetPos; // Vector3.Lerp(aimer.target_node.transform.position, targetPos, 0.25f);
		}

		void fireBullet()
        {
			GameObject bullet = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyPlayerFX.basicSlingshotProjectile);

			Collider c = bullet.GetComponentInChildren<Collider>();
			Collider mCol = GetComponent<Collider>();

			Physics.IgnoreCollision(c, mCol);

			bullet.transform.position = weapon_node.transform.position + weapon_node.transform.up * 0.1f;

			bullet.transform.LookAt(aimer.target_node.transform,Vector3.up);

			timeOut = maxTimeOut;
			ammoLoaded = false;
			afterFireTimeout = 0.25f;
		}

		public void ReleaseSling()
		{
			aimer.ReleaseSling();
		}

		public void GrabSling()
		{
			aimer.GrabSling();
		}

		public void LoadAmmo()
        {
			ammoLoaded = true;
		}

		void handleInput()
        {
			float camX = 0;
			float camY = 0;

			float sensitivity = 512;

			if (GameManager.Instance.cameraInputDisabled)
				return;

			float shoot = Input.GetAxis("Shoot");

			//You were quick~
			if(currentState == state.FIRE && afterFireTimeout > 0.05f)
            {
				if (shoot > 0.5f)
				{
					quickReloadTimeout = 1.0f;
					mAnimator.CrossFadeInFixedTime("Slingshot_Reload", 0.1f);
					currentState = state.RELOAD;
					return;
				}
			}

			if(afterFireTimeout < 0.0001f && currentState == state.FIRE)
            {
				if (shoot < 0.1f && quickReloadTimeout < 0.001f)
				{
					mAnimator.CrossFadeInFixedTime("Slingshot_Ready", 0.25f);
					currentState = state.READY;
					ReleaseSling();
					return;
				}
				else
                {
					mAnimator.CrossFadeInFixedTime("Slingshot_Reload", 0.1f);
					currentState = state.RELOAD;
					return;
				}
			}


			if(currentState == state.READY && shoot > 0.5f)
            {
				mAnimator.CrossFadeInFixedTime("Slingshot_Reload", 0.1f);
				currentState = state.RELOAD;
				return;
            }

			if(currentState == state.READY && shoot < 0.1f)
            {
				float h = Input.GetAxis("Horizontal");
				float v = Input.GetAxis("Vertical");

				if (GameManager.Instance.usingController)
				{
					h = Input.GetAxis("Left Analog X");
					v = Input.GetAxis("Left Analog Y");
				}

				float deadZone = 0.2f;

				if (Mathf.Abs(h) > deadZone || Mathf.Abs(v) > deadZone)
				{
					mPlayer.changeCurrentMode(PlayerModes.NORMAL);
					mAnimator.CrossFade("Walk", 0.2f);
					return;
				}

				if(Input.GetButtonDown("Jump") && mPlayer.canJump(false))
                {
					mPlayer.changeCurrentMode(PlayerModes.NORMAL);
					mPlayer.Jump(false);
					return;
				}
			}

			if(currentState == state.RELOAD)
            {
				if(ammoLoaded)
                {

					if (shoot > 0.5f)
					{
						mAnimator.CrossFadeInFixedTime("Slingshot_Aim", 0.1f);
						currentState = state.AIM;
						GrabSling();
						return;
					}
					else
                    {
						mAnimator.Play("Slingshot_Fire");
						currentState = state.FIRE;
						fireBullet();
						return;
					}

				}
				else if(shoot < 0.1f && quickReloadTimeout < 0.001f)
                {
					//Cancel reload
					ammoLoaded = false;
					currentState = state.READY;
					mAnimator.CrossFadeInFixedTime("Slingshot_Ready", 0.3f);
					ReleaseSling();
					return;
				}
            }

			if(currentState == state.AIM && ammoLoaded)
            {
				if(shoot < 0.1f)
                {
					mAnimator.Play("Slingshot_Fire");
					currentState = state.FIRE;
					fireBullet();
					return;
                }
            }

			if(Input.GetButtonDown("Zoom"))
            {
				leftSide = !leftSide;
            }

			if (!GameManager.Instance.usingController)
			{
				camX = Input.GetAxisRaw("Mouse X");
				camY = Input.GetAxisRaw("Mouse Y");

			}
			else
			{
				if (!Input.GetButton("Zoom"))
				{
					camX = Input.GetAxisRaw("Right Analog X");
					camY = Input.GetAxisRaw("Right Analog Y");
					sensitivity = 128;
				}
			}


			aim_y += camY * (Time.unscaledDeltaTime * (sensitivity * 0.85f)) * GameManager.Instance.config.lookSensitivity * (GameManager.Instance.config.pitchInvert ? -1.0f : 1.0f);
			aim_x += camX * (Time.unscaledDeltaTime * (sensitivity * 0.85f)) * GameManager.Instance.config.lookSensitivity;



			aim_y = Mathf.Clamp(aim_y, -60.0f, 60.0f);


			if (aim_x > 360)
				aim_x -= 360;

			if (aim_x < -360)
				aim_x += 360;
		}
	}
}
