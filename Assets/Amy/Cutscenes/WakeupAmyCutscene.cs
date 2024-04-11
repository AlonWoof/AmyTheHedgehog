using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class WakeupAmyCutscene : BaseCutscene
	{

		public Animator sleepyAmyAnimator;
		public GameObject wakeup_ZoomCam;
		public Animator wakeupCam_anim;
		public GameObject checkpoint;

		// Start is called before the first frame update
		void Start()
	    {
			playCutscene();

			mPlayer = PlayerManager.Instance.getPlayer();
		}
	

		protected override IEnumerator<float> doCutscene()
		{
			GameManager.Instance.cutsceneMode = true;
			GameManager.Instance.cameraInputDisabled = true;
			GameManager.Instance.playerInputDisabled = true;

			if (mPlayer)
			{
				mPlayer.transform.position = Vector3.up * 1000000.0f;
			}

			sleepyAmyAnimator.Play("Sleep");
			wakeup_ZoomCam.SetActive(true);

			yield return Timing.WaitForSeconds(5);

			wakeupCam_anim.Play("Start");

			yield return Timing.WaitForSeconds(10.5f);

			sleepyAmyAnimator.Play("WakeUp");
			yield return Timing.WaitForSeconds(5.0f);

			UIManager.Instance.fadeScreen(false, 1.0f);
			yield return Timing.WaitForSeconds(1.0f);



			PlayerManager.Instance.playerCheckpoint.transform.position = checkpoint.transform.position;
			PlayerManager.Instance.playerCheckpoint.transform.rotation = checkpoint.transform.rotation;
			mPlayer = PlayerManager.Instance.spawnPlayerAtCheckpoint();

			sleepyAmyAnimator.gameObject.SetActive(false);
			wakeup_ZoomCam.SetActive(false);

			yield return Timing.WaitForSeconds(0.5f);

			mPlayer.tpc.centerBehindPlayer();

			UIManager.Instance.fadeScreen(true, 1.0f);
			yield return Timing.WaitForSeconds(1.5f);

			GameManager.Instance.cutsceneMode = false;
			GameManager.Instance.cameraInputDisabled = false;
			GameManager.Instance.playerInputDisabled = false;

			endCutscene();
		}
	}
}
