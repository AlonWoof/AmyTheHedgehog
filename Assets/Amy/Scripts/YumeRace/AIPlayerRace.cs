using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public enum YumeRacePhase
    {
		Init,
		Ready,
		Normal,
		Complete
    }

	public class AIPlayerRace : PlayerMode
	{

		Racetrack mRaceTrack;
		public AIPlayer aiplayer;
		public int currentRaceNode = 0;

		public float nodeDist = 0.0f;
		public float verticalNodeDist = 0.0f;
		public Vector3 nodeDir = Vector3.forward;

		YumeRacePhase currentPhase = YumeRacePhase.Init;

		float phaseTimeout = 5.0f;

		// Start is called before the first frame update
		void Start()
	    {
			getBaseComponents();

			mRaceTrack = FindObjectOfType<Racetrack>();
			aiplayer = GetComponent<AIPlayer>();

			if (!mRaceTrack)
				enabled = false;

			

			
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			switch(currentPhase)
			{
				case YumeRacePhase.Init:
					updateInitPhase();
					break;

				case YumeRacePhase.Ready:
					updateReadyPhase();
					break;

				case YumeRacePhase.Normal:
					updateNormalPhase();
					break;

				case YumeRacePhase.Complete:
					break;
			}
		}

		void changePhase(YumeRacePhase newPhase)
        {
			if (currentPhase == newPhase)
				return;

			currentPhase = newPhase;

			switch (newPhase)
            {

				case YumeRacePhase.Ready:
					mPlayer.changeCurrentMode(PlayerModes.CUTSCENE);
					mAnimator.Play("Race_Ready");
					break;

				case YumeRacePhase.Normal:
					mPlayer.changeCurrentMode(PlayerModes.NORMAL);
					mPlayer.resetGroundFlags();
					mAnimator.CrossFade("Locomotion", 0.2f);
					break;

				case YumeRacePhase.Complete:
					break;
            }
        }

		void updateInitPhase()
        {
			if(mPlayer.framesGrounded > 10)
            {
				mPlayer.clearAccel();
				updateNormalPhase();
				mPlayer.setAngleInstantly(nodeDir);
				phaseTimeout = 5.0f;
				changePhase(YumeRacePhase.Ready);

			}
        }

		void updateReadyPhase()
		{
			phaseTimeout -= Time.deltaTime;

			if (phaseTimeout < 0.0f)
				changePhase(YumeRacePhase.Normal);

			Vector3 start = transform.position + Vector3.up * 0.5f;
			Vector3 end = start - Vector3.up * 10.0f;

			RaycastHit hitInfo = new RaycastHit();

			if(Physics.Linecast(start,end, out hitInfo, mPlayer.mColMask))
            {
				transform.position = hitInfo.point;
            }
		}

		void updateNormalPhase()
        {
			aiplayer.handleVirtualButtons();
			evaluateWaypointDistance();
			moveToDestination();

			checkIfDead();
		}

		Waypoint getCurrentNode()
        {
			if (currentRaceNode > mRaceTrack.racetrackNodes.Count-1)
				currentRaceNode = 0;

			return mRaceTrack.racetrackNodes[currentRaceNode];
		}

		void evaluateWaypointDistance()
        {
			Vector3 mpos = transform.position;
			Vector3 tpos = getCurrentNode().transform.position;

			//tpos = PlayerManager.Instance.mPlayerInstance.transform.position;

			verticalNodeDist = tpos.y - mpos.y;

			mpos.y = 0;
			tpos.y = 0;

			nodeDist = Vector3.Distance(mpos, tpos);
			nodeDir = Helper.getDirectionTo(mpos, tpos).normalized;
        }

		void moveToDestination()
        {
			
			if(nodeDist < 3.0f)
            {
				currentRaceNode++;

				if (currentRaceNode > mRaceTrack.racetrackNodes.Count)
					currentRaceNode = 0;
			}

			float sine = Mathf.Sin(Time.time * 3.0f);

			aiplayer.desiredVirtualAnalogX = nodeDir.x + (sine * 0.2f);
			aiplayer.desiredVirtualAnalogY = nodeDir.z + (sine * 0.2f);


			if (verticalNodeDist > 3.0f && nodeDist < 8.0f)
			{
				aiplayer.pressJump();
			}

			if (verticalNodeDist < 1.0f || mPlayer.framesGrounded > 15)
            {
				aiplayer.releaseJump();
            }
		}

		void checkIfDead()
        {
			mPlayer.getStatus().currentHealth = 9999.0f;


			if (mPlayer.currentMode == PlayerModes.KILLED)
            {
				currentRaceNode--;

				if (currentRaceNode < 0)
					currentRaceNode = 0;

				mPlayer.getStatus().currentHealth = mPlayer.getStatus().maxHealth;
				mPlayer.changeCurrentMode(PlayerModes.NORMAL);

				mPlayer.transform.position = getCurrentNode().transform.position;
            }
        }
	}
}
