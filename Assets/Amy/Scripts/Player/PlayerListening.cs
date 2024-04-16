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

			startListeningAnimation();
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
			if(mPlayer.lastMode == PlayerModes.NORMAL)
            {
				mAnimator.CrossFade("Listening", 0.2f);
            }
        }

		// Update is called once per frame
		void Update()
	    {
	        
	    }

		private void FixedUpdate()
		{
			if (GameManager.Instance.gamePaused)
				return;

			mPlayer.CalcVerticalVelocity();
			mPlayer.applyFriction();
			mPlayer.updatePosition();
			mPlayer.updateRotation();
		}
	}
}
