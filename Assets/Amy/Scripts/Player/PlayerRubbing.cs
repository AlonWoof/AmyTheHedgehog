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
		public float staminaDrainRate = 0.1f;

		public GameObject cunnyDripFX;

		enum MasturbationPhase
        {
			Main,
			Close,
			Cum
        }

		MasturbationPhase phase = MasturbationPhase.Main;

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

			if(!cunnyDripFX)
            {
				cunnyDripFX = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_cunnyDrip);
				cunnyDripFX.transform.SetParent(mPlayer.getBoneByName("hips"));
				cunnyDripFX.transform.localPosition = Vector3.zero;
				cunnyDripFX.transform.localRotation = Quaternion.identity;

			}

			//Sometimes a girl needs a little break~
			Timing.RunCoroutine(doStartRubbing());
			
		}

		public IEnumerator<float> doStartRubbing()
		{
			mPlayer.clearAccel();
			mPlayer.clearSpeed();

			timeTilOrgasm = Random.Range(10.0f, 20.0f);
			phase = MasturbationPhase.Main;

			bool cancel = false;


			mAnimator.Play("Rubbing_Start");

			while (mAnimator.IsInTransition(0))
			{
				yield return Timing.WaitForSeconds(0.1f);
			}
			cunnyDripFX.SetActive(true);


			mAnimator.CrossFade("Face_Ecchi", 0.25f);

			//mPlayer.updateExpression();

		}

		public IEnumerator<float> doOrgasm()
        {
			bool cancel = false;

			PlayerManager.Instance.lastOrgasmCooldown = 60.0f * Random.Range(10, 15);

			mAnimator.Play("Rubbing_Cum");
			yield return Timing.WaitForSeconds(2.5f);

			cunnyDripFX.SetActive(false);
			mAnimator.CrossFade("Idle", 0.2f);
			yield return Timing.WaitForSeconds(0.1f);
			mPlayer.changeCurrentMode(PlayerModes.NORMAL);

		}

		public IEnumerator<float> cancelCaught()
        {
			mAnimator.CrossFade("Idle", 0.2f);
			yield return Timing.WaitForSeconds(0.1f);
			mPlayer.changeCurrentMode(PlayerModes.NORMAL);
		}

		// Update is called once per frame
		void Update()
	    {
			PlayerStatus pstats = mPlayer.getStatus();

			float healMult = mAnimator.GetFloat("animSpeed");

			if(phase == MasturbationPhase.Main && timeTilOrgasm < 7.5f)
            {
				mAnimator.CrossFade("Rubbing_Close", 1.0f);
				phase = MasturbationPhase.Close;
            }

			if (pstats.currentHealth < pstats.maxHealth)
            {
				pstats.currentHealth += Time.deltaTime * healthRecoveryRate * healMult;
				pstats.currentStamina -= Time.deltaTime * staminaDrainRate * healMult;
				mPlayer.updateHealth();
			}

			if (timeTilOrgasm > 0.0f)
				timeTilOrgasm -= Time.deltaTime;

			if (timeTilOrgasm < 2.0f)
				mAnimator.SetFloat("animSpeed", 1.5f);
			else
				mAnimator.SetFloat("animSpeed", 1.0f);

			/*if (mPlayer.areaDetector.getNearbyActorCount() > 0)
			{
				timeTilOrgasm = 10.0f;
				Timing.RunCoroutine(cancelCaught());
				
			}*/

			if (timeTilOrgasm <= 0.0f)
            {
				pstats.currentHealth += pstats.maxHealth * 0.25f;
				mPlayer.updateHealth();

				Timing.RunCoroutine(doOrgasm());
            }


	    }

	}
}
