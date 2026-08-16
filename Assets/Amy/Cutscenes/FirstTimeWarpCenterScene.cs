using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2026 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class FirstTimeWarpCenterScene : BaseCutscene
	{
		public GameObject amyRoot;
		public Animator amyAnimator;
		public GameObject cam0;
		public GameObject cam1;
		public GameObject cam2;
		public GameObject cam3;


		// Start is called before the first frame update
		void Start()
	    {
			amyRoot.SetActive(false);
			playCutscene();
			mPlayer = PlayerManager.Instance.getPlayer();

			if (!PlayerManager.Instance.getStoryFlag(getStoryHash()))
				PlayerManager.Instance.lastExit = 999;
		}

		protected override IEnumerator<float> doCutscene()
		{
			//if (mPlayer)
			//	mPlayer.gameObject.SetActive(false);

			cam0.SetActive(true);
			cam1.SetActive(false);
			cam2.SetActive(false);
			cam3.SetActive(false);

			while (GameManager.Instance.isLoading)
				yield return 0f;

			if (mPlayer)
			{
				mPlayer.changeCurrentMode(PlayerModes.CUTSCENE);
				mPlayer.transform.position = Vector3.up * 1000000.0f;
			}

			GameManager.Instance.cutsceneMode = true;
			GameManager.Instance.disableInput();

			canSkipScene = true;



			amyRoot.SetActive(true);

			yield return Timing.WaitForSeconds(5.2f);

			cam0.SetActive(false);
			cam1.SetActive(true);

			
			amyAnimator.CrossFade("LookAround", 0.2f);

			yield return Timing.WaitForSeconds(5.5f);

			cam1.SetActive(false);
			cam2.SetActive(true);

			amyAnimator.CrossFade("LookUp", 0.2f);
			yield return Timing.WaitForSeconds(0.21f);
			amyAnimator.Play("word_are");
			CoroutineHandle msg = UIManager.Instance.messageBox.showMessageBox("This... isn't my home.", SpeakerProfile.Amy);

			
			amyAnimator.Play("Face_Talk");
			yield return Timing.WaitForSeconds(1.0f);
			amyAnimator.CrossFade("Face_Neutral",0.1f);

			while (msg.IsRunning)
				yield return 0f;

			yield return Timing.WaitForSeconds(1.0f);

			cam2.SetActive(false);
			cam3.SetActive(true);

			yield return Timing.WaitForSeconds(3.0f);

			msg = UIManager.Instance.messageBox.showMessageBox("That door... \nWhy does it have my name on it?", SpeakerProfile.Amy);

			while (msg.IsRunning)
				yield return 0f;

			yield return Timing.WaitForSeconds(3.0f);

			UIManager.Instance.fadeScreen(false, 0.5f, false);

			yield return Timing.WaitForSeconds(0.5f);

			endCutscene();

		}

		public override void endCutscene()
		{
			base.endCutscene();
			PlayerManager.Instance.lastExit = 7;
			GameManager.Instance.loadScene("WarpCenter");
			//PlayerManager.Instance.setStoryFlag("PROLOGUE_DONE", true);
		}
	}
}
