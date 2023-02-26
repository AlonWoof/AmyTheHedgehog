using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerBasicMove : PlayerMode
	{
	    // Start is called before the first frame update
	    void Start()
	    {
			getBaseComponents();
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			handleInput();
			mPlayer.checkStickPower();
			mPlayer.CalcSlope();
			mPlayer.checkForJump();

			if(mPlayer.mChara == PlayableCharacter.Amy)
				mPlayer.checkForHammerJump();
		}

        private void FixedUpdate()
        {

			mPlayer.CalcVerticalVelocity();
			mPlayer.applyFriction();
			mPlayer.updatePosition();
			mPlayer.updateRotation();
		}

        private void LateUpdate()
        {
			mPlayer.checkIfUnderwater();

		}

        void handleInput()
        {


		}


	}
}
