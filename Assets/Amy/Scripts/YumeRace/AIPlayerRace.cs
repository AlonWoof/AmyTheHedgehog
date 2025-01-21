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
		public RaceDifficulty difficulty;
		public int currentRaceNode = 0;

		public float nodeDist = 0.0f;
		public float verticalNodeDist = 0.0f;
		public Vector3 nodeDir = Vector3.forward;

		YumeRacePhase currentPhase = YumeRacePhase.Init;

		float phaseTimeout = 5.0f;

		List<Renderer> renderers;

		// Start is called before the first frame update
		void Start()
	    {
			getBaseComponents();



			mRaceTrack = FindObjectOfType<Racetrack>();
			aiplayer = GetComponent<AIPlayer>();


			renderers = new List<Renderer>();

			foreach(Renderer r in GetComponentsInChildren<Renderer>())
            {
				renderers.Add(r);
            }

			if (!mRaceTrack)
				enabled = false;

		}

		bool isVisibleToPlayer()
        {

			if (Vector3.Distance(transform.position, GameManager.Instance.mainCamera.transform.position) < 8.0f)
				return true;

			foreach (Renderer r in renderers)
            {
				if (r.isVisible)
					return true;
            }



			return false;
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
					updateCompletePhase();
					break;
			}
		}

		public void changePhase(YumeRacePhase newPhase)
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
					mAnimator.Play("Locomotion");

					if (difficulty == RaceDifficulty.Medium)
						mPlayer.acceleration.z = 2.0f;

					if (difficulty == RaceDifficulty.Hard)
						mPlayer.acceleration.z = 6.0f;

					break;

				case YumeRacePhase.Complete:
					mPlayer.acceleration.z *= 0.5f;
					break;
            }
        }

		void updateInitPhase()
        {
			if(mPlayer.framesGrounded > 10)
            {
				mPlayer.clearAccel();
				evaluateWaypointDistance();
				mPlayer.setAngleInstantly(nodeDir);
				changePhase(YumeRacePhase.Ready);

			}
        }

		void updateCompletePhase()
        {
			aiplayer.desiredVirtualAnalogX = 0.0f;
			aiplayer.desiredVirtualAnalogY = 0.0f;
		}

		void updateReadyPhase()
		{
			phaseTimeout -= Time.deltaTime;


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
			evaluateWaypointDistance();
			moveToDestination();

			checkIfDead();

			if (difficulty == RaceDifficulty.Hard)
			{
				aiplayer.acceleration.z = Mathf.Clamp(aiplayer.acceleration.z, 3.0f, 100.0f);
			}

			updateRubberbanding();
		}

		void updateRubberbanding()
        {

			if (isVisibleToPlayer())
				return;


			int bandThreshold = 20;


			if (difficulty == RaceDifficulty.Medium)
				bandThreshold = 10;

			if (difficulty == RaceDifficulty.Hard)
				bandThreshold = 1;

			if(mRaceTrack.getRacerScore(Racer.Amy) - mRaceTrack.getRacerScore(Racer.Yume) > bandThreshold)
            {
				transform.position = Vector3.Lerp(transform.position, getCurrentNode().transform.position + Vector3.up * 0.5f, 0.1f);
				mAnimator.Play("Locomotion");
            }
        }

		RaceNode getCurrentNode()
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
			
			if(nodeDist < getCurrentNode().aiRange)
            {
				currentRaceNode++;

				if (currentRaceNode > mRaceTrack.racetrackNodes.Count)
					currentRaceNode = 0;
			}

			//float sineMult = Mathf.Sin(Time.time);
			float sine = Mathf.Sin(Time.time * 3.0f);
			float sineInfluence = 0.2f;


			if (difficulty == RaceDifficulty.Easy)
				sineInfluence = 0.4f;

			if (difficulty == RaceDifficulty.Medium)
				sineInfluence = 0.2f;

			if (difficulty == RaceDifficulty.Hard)
				sineInfluence = 0.2f;

			float pityMultiplier = 1.0f;

			if (mRaceTrack.getRacerScore(Racer.Yume) - mRaceTrack.getRacerScore(Racer.Amy) > 10)
				pityMultiplier = 0.5f;

			if (difficulty == RaceDifficulty.Hard)
				pityMultiplier = 1.0f;

			aiplayer.desiredVirtualAnalogX = (nodeDir.x + (sine * sineInfluence)) * pityMultiplier;
			aiplayer.desiredVirtualAnalogY = (nodeDir.z + (sine * sineInfluence)) * pityMultiplier;


		

			if (aiplayer.isOnGround)
			{
				if (aiplayer.canJumpLedge(1.0f, 2.0f) || 
					aiplayer.canJumpLedge(aiplayer.acceleration.z * 0.5f, 2.0f) || 
					aiplayer.canJumpGap(aiplayer.acceleration.z) ||
					aiplayer.isSmallObstacleInFront(1.0f + aiplayer.acceleration.z * 0.25f))
				{
					aiplayer.pressJump();
				}
			}

			if (aiplayer.shouldReleaseJump())
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
