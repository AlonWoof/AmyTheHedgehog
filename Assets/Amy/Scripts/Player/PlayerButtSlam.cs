using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.InputSystem;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerButtSlam : PlayerMode
	{

		public GameObject buttSlamAura;
		public CoroutineHandle actionHandle;

		void SpawnFX(GameObject fx)
        {
			GameObject inst = GameObject.Instantiate(fx);
			inst.transform.position = transform.position;
			inst.transform.rotation = transform.rotation;
			inst.transform.SetParent(transform);
        }


		private void OnEnable()
		{
			getBaseComponents();


			if (mPlayer.currentMode != PlayerModes.BUTTSLAM)
			{
				enabled = false;
				return;
			}

			mAnimator.Rebind();
			mAnimator.Update(0f);

			if (!buttSlamAura)
			{
				buttSlamAura = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_creamButtSlamAura);
				buttSlamAura.transform.SetParent(transform);
				buttSlamAura.transform.position = transform.position;
				buttSlamAura.transform.rotation = transform.rotation;
			}

			buttSlamAura.SetActive(false);

			mPlayer.clearAccel();
			mPlayer.clearSpeed();
			mPlayer.canAirAttack = false;
			mPlayer.framesGrounded = 0;

			if(mPlayer.tpc)
				mPlayer.tpc.changeCameraMode(TPCMode.Normal);

			actionHandle = Timing.RunCoroutine(startButtSlam(), gameObject);
		}

        private void OnDisable()
        {
            if(actionHandle.IsRunning)
            {
				Timing.KillCoroutines(actionHandle);
				buttSlamAura.SetActive(false);
            }
        }

        IEnumerator<float> startButtSlam()
        {

			//mPlayer.tpc.changeCameraMode(TPCMode.Normal);
			mAnimator.Rebind();
			mAnimator.Update(0f);
			mAnimator.Play("ButtSlam_Start");

			yield return Timing.WaitForSeconds(0.125f);

			actionHandle = Timing.RunCoroutine(buttSlamLoop(), gameObject);
        }

		IEnumerator<float> buttSlamLoop()
        {
			bool done = false;

			mPlayer.mVoice.playVoice(mPlayer.mVoice.powerAttack, true);
			buttSlamAura.SetActive(true);
			while (!done)
            {
				mAnimator.Play("ButtSlam_Loop");


				mPlayer.acceleration.y = -30.0f;



				if (mPlayer.getWaterDepth() > 0.5f)
				{
					mPlayer.acceleration.y = 0.0f;
					mPlayer.changeCurrentMode(PlayerModes.SWIMMING);
					mPlayer.setVelocityDirectly(mPlayer.mRigidBody.velocity * 0.5f);
					mPlayer.resetGroundFlags();
				}

				if(mPlayer.currentMode != PlayerModes.BUTTSLAM)
                {
					buttSlamAura.SetActive(false);
					yield break;
				}

				if (mPlayer.framesGrounded > 3  && mPlayer.slopeAmount < 0.2f)
					done = true;

				yield return 0f;
            }
			buttSlamAura.SetActive(false);


			mPlayer.acceleration.y = 0.0f;
			mAnimator.Play("ButtSlam_Impact");
			SpawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_creamButtSlamImpact);

			yield return Timing.WaitForSeconds(0.05f);

			if (!mPlayer.isAiControlled)
			{
				GameManager.Instance.controllerRumble(0.25f, 0.75f, 0.75f);
				mPlayer.tpc.shakeCamera(0.2f, 0.15f);
				GameManager.Instance.hitStun(0.03f, 0.0f);
			}

			yield return Timing.WaitForSeconds(0.25f);
			mAnimator.Play("ButtSlam_End");

			yield return Timing.WaitForSeconds(0.5f);

			mPlayer.resetGroundFlags();
			mPlayer.changeCurrentMode(PlayerModes.NORMAL);
        }

        private void FixedUpdate()
        {
			//mPlayer.CalcVerticalVelocity();
			mPlayer.applyFriction();
			mPlayer.updatePosition();
			mPlayer.updateRotation();
		}

        private void Update()
        {
			if(!checkGrounded())
				mPlayer.setVelocityDirectly(Vector3.down * 8.0f);

			mPlayer.checkIfUnderwater();
		}



        bool checkGrounded()
        {
			Vector3 start = transform.position + Vector3.up;
			Vector3 end = transform.position;

			RaycastHit hitInfo = new RaycastHit();

			if(Physics.Linecast(start,end, out hitInfo, mPlayer.mColMask))
            {
				float ang = Vector3.Dot(Vector3.up, hitInfo.normal);
				Debug.Log("BUTT ANG: " + ang);

				transform.position = hitInfo.point;

				

				if (Vector3.Dot(Vector3.up, hitInfo.normal) > 0.6f)
				{
					mPlayer.groundNormal = hitInfo.normal;
					mPlayer.framesGrounded++;
					return true;
				}
            }

			mPlayer.framesGrounded = 0;
			return false;
        }
	}
}
