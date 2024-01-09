using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerRubbing : PlayerMode
	{

		const float time_High = 30.0f;
		const float time_Mid = 15.0f;
		const float time_Low = 7.5f;

		public float timeTilOrgasm = 15.0f;
		public float healthRecoveryRate = 0.8f;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		private void OnEnable()
		{
			getBaseComponents();

			if (mPlayer.currentMode != PlayerModes.RUBBING)
			{
				enabled = false;
				return;
			}

			//Sometimes a girl needs a little break~

			Timing.RunCoroutine(doStartRubbing());

		}

		public IEnumerator<float> doStartRubbing()
		{
			timeTilOrgasm = Random.Range(10.0f, 20.0f);

			bool cancel = false;


			mAnimator.CrossFade("Rubbing",0.2f);

			yield return Timing.WaitForSeconds(0.1f);

			mPlayer.mAnimator.Play("Face_Ecchi");

		}

		public IEnumerator<float> doOrgasm()
        {
			bool cancel = false;

			PlayerManager.Instance.lastOrgasmCooldown = 60.0f * Random.Range(10, 15);

			mAnimator.CrossFade("Idle", 0.2f);
			yield return Timing.WaitForSeconds(0.1f);
			mPlayer.mAnimator.Play("Face_Neutral");
			mPlayer.changeCurrentMode(PlayerModes.NORMAL);
		}

		public IEnumerator<float> cancelCaught()
        {
			mPlayer.getStatus().currentMood = 0.0f;
			mAnimator.CrossFade("Idle", 0.2f);
			yield return Timing.WaitForSeconds(0.1f);
			mPlayer.mAnimator.Play("Face_Neutral");
			mPlayer.changeCurrentMode(PlayerModes.NORMAL);
		}

		// Update is called once per frame
		void Update()
	    {
			PlayerStatus pstats = mPlayer.getStatus();

			float healMult = mAnimator.GetFloat("animSpeed");

			if (pstats.currentMood < pstats.maxMood)
            {
				pstats.currentMood += Time.deltaTime * healthRecoveryRate * healMult;
				mPlayer.updateHealth();
			}

			if (timeTilOrgasm > 0.0f)
				timeTilOrgasm -= Time.deltaTime;

			if (mPlayer.areaDetector.getNearbyActorCount() > 0)
			{
				timeTilOrgasm = 10.0f;
				Timing.RunCoroutine(cancelCaught());
				
			}

			if (timeTilOrgasm <= 0.0f)
            {
				pstats.currentHealth += pstats.maxHealth * 0.25f;
				mPlayer.updateHealth();

				Timing.RunCoroutine(doOrgasm());
            }


	    }

	}
}
