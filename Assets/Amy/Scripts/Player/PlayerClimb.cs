using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerClimb : PlayerMode
	{

		Ladder currentLadder;
		Vector3 mDesiredMovement;

		//よいしょ... よいしょ... よいしょ...
		float climbSpeed = 1.5f;

	    // Start is called before the first frame update
	    void Start()
	    {
			getBaseComponents();

			if (mPlayer.currentMode != PlayerModes.LADDER)
			{
				enabled = false;
				return;
			}
		}
	
	    // Update is called once per frame
	    void LateUpdate()
	    {
			if (!currentLadder)
				return;

			handleInput();
			handleLadderMovement();

			mPlayer.direction = currentLadder.transform.forward;
			Vector3 mVec = currentLadder.transform.position;
			mVec.y = mPlayer.transform.position.y;

			mPlayer.transform.position = Vector3.Lerp(mPlayer.transform.position, mVec, 0.5f);

			transform.rotation = Quaternion.LookRotation(mPlayer.direction.normalized);

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


			Vector3 targetDirection = Vector3.ClampMagnitude(new Vector3(0, v, 0), 1.0f);
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

			Vector3 camAngle = GameManager.Instance.mainCamera.transform.forward;
			camAngle.y = 0;
			camAngle.Normalize();

			mDesiredMovement = targetDirection * (targetDirection.magnitude);

		}

		void handleLadderMovement()
        {

			mRigidBody.velocity = mDesiredMovement * climbSpeed;

		}

		public void startClimbing(Ladder mLadder)
        {
			currentLadder = mLadder;
			mPlayer.direction = mLadder.transform.forward;

			Vector3 mVec = mLadder.transform.position;

			mVec.y = mPlayer.transform.position.y;

			mPlayer.transform.position = mVec;
		}
	}
}
