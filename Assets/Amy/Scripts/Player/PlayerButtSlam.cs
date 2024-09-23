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

			mPlayer.tpc.changeCameraMode(TPCMode.Normal);
			Timing.RunCoroutine(startButtSlam());
		}



		IEnumerator<float> startButtSlam()
        {

			//mPlayer.tpc.changeCameraMode(TPCMode.Normal);
			mAnimator.Play("ButtSlam_Start");

			yield return Timing.WaitForSeconds(0.125f);

			Timing.RunCoroutine(buttSlamLoop());
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

				if(mPlayer.currentMode != PlayerModes.BUTTSLAM)
                {
					buttSlamAura.SetActive(false);
					yield break;
				}

				if (mPlayer.framesGrounded > 3)
					done = true;

				yield return 0f;
            }
			buttSlamAura.SetActive(false);


			mPlayer.acceleration.y = 0.0f;
			mAnimator.Play("ButtSlam_Impact");
			SpawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_creamButtSlamImpact);

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
