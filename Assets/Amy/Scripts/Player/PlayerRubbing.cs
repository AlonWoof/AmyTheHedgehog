 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.InputSystem;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerRubbing : PlayerMode
	{

		float phaseTimeLeft = 10.0f;

		public GameObject cunnyDripFX;
		public SkinnedMeshRenderer karadaMesh = null;

		enum MasturbationPhase
        {
			Main,
			Close,
			Cum,
			Caught
        }

		enum cantMasturbateReason
        {
			Generic,
			Public,
			Scared,
			DirtyPlace,
			DirtyAmy,
			Tired,
			Cream
        }

		MasturbationPhase phase = MasturbationPhase.Main;


		private void OnEnable()
		{
			getBaseComponents();

			if (!cunnyDripFX)
			{
				cunnyDripFX = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_cunnyDrip);
				cunnyDripFX.transform.SetParent(mPlayer.getBoneByName("hips"));
				cunnyDripFX.transform.localPosition = Vector3.zero;
				cunnyDripFX.transform.localRotation = Quaternion.identity;
				cunnyDripFX.SetActive(false);
			}

			foreach (SkinnedMeshRenderer smr in GetComponentsInChildren<SkinnedMeshRenderer>())
			{
				if(smr.gameObject.name.ToLower().Contains("karada"))
                {
					karadaMesh = smr;
                }
			}

			if (mPlayer.currentMode != PlayerModes.RUBBING)
			{
				enabled = false;
				return;
			}

			//Sometimes a girl needs a little break~
			Timing.RunCoroutine(doStartRubbing().CancelWith(gameObject));
			


		}

        private void OnDisable()
        {
			if(mPlayer.tpc)
				mPlayer.tpc.changeCameraMode(TPCMode.Normal);
		}

        public IEnumerator<float> doStartRubbing()
		{
			mPlayer.clearAccel();
			mPlayer.clearSpeed();

			mPlayer.tpc.changeCameraMode(TPCMode.AmyMasturbation);
			changePhase(MasturbationPhase.Main);

			bool cancel = false;

			

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

			if (!mPlayer.isAiControlled)
				GameManager.Instance.controllerRumble(1.0f, 1.0f, 1.0f);

			float effectTime = Helper.minutesToSeconds(5);

			//Really hit the spot <3
			if (mPlayer.getStatus().checkStatusEffect(PlayerStatusFX.Horny))
			{
				mPlayer.getStatus().unSetStatusEffect(PlayerStatusFX.Horny);
				mPlayer.getStatus().setStatusEffect(PlayerStatusFX.RecentOrgasm);
				mPlayer.getStatus().recentOrgasmTimeLeft = Helper.minutesToSeconds(10);
			}


			cunnyDripFX.SetActive(false);
			mAnimator.CrossFade("Idle", 0.2f);
			yield return Timing.WaitForSeconds(0.1f);

			mPlayer.tpc.changeCameraMode(TPCMode.Normal);
			mPlayer.changeCurrentMode(PlayerModes.NORMAL);

			yield return Timing.WaitForSeconds(Random.Range(3.0f, 6.0f));

			int aftershock = Random.Range(2, 8);

			while (aftershock > 0)
			{
				cunnyDripFX.SetActive(true);

				if(!mPlayer.isAiControlled)
					GameManager.Instance.controllerRumble(0.5f, 0.05f, 0.06f);

				aftershock--;
				yield return Timing.WaitForSeconds(Random.Range(3.0f, 6.0f));
				cunnyDripFX.SetActive(false);
			}

			cunnyDripFX.SetActive(false);
		}


		public bool canMasturbate()
		{
			PlayerStatus pstats = mPlayer.getStatus();

			//Creamy isn't interested
			if (mPlayer.mChara != PlayableCharacter.Amy)
				return false;

			//What was I thinking? Girls have no refractory period lol...
			//if (pstats.checkStatusEffect(PlayerStatusFX.RecentOrgasm))
			//	return false;

			if (pstats.checkStatusEffect(PlayerStatusFX.Horny))
				return true;

			if (pstats.currentHealth < pstats.maxHealth * 0.75f)
				return true;

			if (PlayerManager.Instance.getStoryFlag(StoryFlag.SADX_NUDE))
				return true;

			return false;
		}

		public bool shouldMasturbate()
        {
			PlayerStatus pstats = mPlayer.getStatus();

			if (mPlayer.mChara != PlayableCharacter.Amy)
				return false;

			if (mPlayer.areaDetector.getNearbyActorCount() > 0)
			{
				showCantMasturbateMessage(cantMasturbateReason.Public);
				return false;
			}

			if (mPlayer.areaDetector.isVisibleToNPC())
			{
				showCantMasturbateMessage(cantMasturbateReason.Public);
				return false;
			}

			if (pstats.checkVibe(VibeType.Dirty))
			{
				showCantMasturbateMessage(cantMasturbateReason.DirtyPlace);
				return false;
			}


			if (pstats.checkVibe(VibeType.Scary) || pstats.checkStatusEffect(PlayerStatusFX.Scared))
			{
				showCantMasturbateMessage(cantMasturbateReason.Scared);
				return false;
			}

			if (pstats.checkStatusEffect(PlayerStatusFX.Dirty))
            {
				showCantMasturbateMessage(cantMasturbateReason.DirtyAmy);
				return false;
			}

			return true;

        }

		void changePhase(MasturbationPhase np)
        {
			switch(np)
            {
				case MasturbationPhase.Main:
					mAnimator.Play("Rubbing_Start");
					mAnimator.SetFloat("animSpeed", 0.8f);
					phaseTimeLeft = Random.Range(10, 15);
					break;

				case MasturbationPhase.Close:
					mAnimator.CrossFade("Rubbing_Close", 0.7f);
					mAnimator.SetFloat("animSpeed", 1.0f);
					phaseTimeLeft = Random.Range(10, 15);
					break;

				case MasturbationPhase.Cum:
					phaseTimeLeft = 100;
					Timing.RunCoroutine(doOrgasm());
					break;
			}

			phase = np;
        }

		// Update is called once per frame
		void Update()
	    {

			float healRate = 5.0f;
			float stamDrainRate = 5.0f;
			float animProgress = mAnimator.GetFloat("animProgress");
			float rumbleStrength = 0.5f;

			switch (phase)
            {
				case MasturbationPhase.Main:
					healRate = 2.0f;
					stamDrainRate = 0.25f;
					rumbleStrength = 0.25f;

					if (phaseTimeLeft < 5.0f)
                    {
						mAnimator.SetFloat("animSpeed", 1.0f);
					}
					else
                    {
						mAnimator.SetFloat("animSpeed", 0.75f);
					}

					break;

				case MasturbationPhase.Close:
					healRate = 5.0f;
					stamDrainRate = 0.5f;
					rumbleStrength = 0.5f;

					if (phaseTimeLeft < 5.0f)
					{
						mAnimator.SetFloat("animSpeed", 1.5f);
					}
					else
					{
						mAnimator.SetFloat("animSpeed", 1.0f);
					}

					break;

				case MasturbationPhase.Cum:
					healRate = 8.0f;
					stamDrainRate = 1.25f;
					rumbleStrength = 1.0f;
					break;
            }


			mPlayer.getStatus().currentHealth += healRate * (Time.deltaTime * animProgress);

			if(mPlayer.getStatus().currentHealth < (mPlayer.getStatus().maxHealth * 0.95f))
				mPlayer.getStatus().currentStamina -= stamDrainRate * (Time.deltaTime * animProgress);

			

			if (Gamepad.current != null)
				Gamepad.current.SetMotorSpeeds(animProgress * rumbleStrength, animProgress * rumbleStrength);

			phaseTimeLeft -= Time.deltaTime;


			if(phaseTimeLeft < 0.0f)
            {
				switch(phase)
                {
					case MasturbationPhase.Main:
						changePhase(MasturbationPhase.Close);
						break;

					case MasturbationPhase.Close:
						changePhase(MasturbationPhase.Cum);
						break;
				}
            }

	    }

		void showCantMasturbateMessage(cantMasturbateReason reason)
        {
			string reasonMessage = "";
			
			switch(reason)
            {
				case cantMasturbateReason.Generic:
					reasonMessage = "I can't do that now!";
					break;

				case cantMasturbateReason.Public:
					reasonMessage = "There's people around! \n What if someone sees me?";
					break;

				case cantMasturbateReason.DirtyPlace:
					reasonMessage = "This place is disgusting! \n Why would I do that here?";
					break;

				case cantMasturbateReason.DirtyAmy:
					reasonMessage = "I need a shower first.";
					break;

				case cantMasturbateReason.Scared:
					reasonMessage = "I'm too scared!";
					break;

				case cantMasturbateReason.Tired:
					reasonMessage = "I'm too tired, even for that...";
					break;

				case cantMasturbateReason.Cream:
					reasonMessage = "I can't do that in front of Cream!";
					break;

			}

			Timing.RunCoroutine(doCantMasturbateMessage(reasonMessage), gameObject);
        }

		IEnumerator<float> doCantMasturbateMessage(string m)
        {
			CoroutineHandle cr = UIManager.Instance.messageBox.showMessageBox(m, SpeakerProfile.Amy);

			while(cr.IsRunning)
            {
				yield return 0f;
            }


        }

	}
}
