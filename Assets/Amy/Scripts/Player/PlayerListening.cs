using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerListening : PlayerMode
	{



		// Start is called before the first frame update

		bool hasFinishedAnim = false;

	    void Start()
	    {
			getBaseComponents();
		}

		private void OnEnable()
		{
			getBaseComponents();

			if (mPlayer.currentMode != PlayerModes.LISTENING)
			{
				enabled = false;
				return;
			}

			mPlayer.clearAccel();
			mPlayer.clearSpeed();
			hasFinishedAnim = false;

			//startListeningAnimation();
			mAnimator.Play("Idle");
			Invoke("startListeningAnimation", 0.2f);
		}

        private void OnDisable()
        {
			if (mPlayer.currentMode == PlayerModes.NORMAL)
			{
				mAnimator.CrossFade("Idle", 0.1f);
			}
		}

        void startListeningAnimation()
        {
			//mAnimator.Play("Listening");
			//mAnimator.SetFloat("run_anim_speed", 0.5f);
			mAnimator.CrossFade("Listening", 0.2f);
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
	}
}
