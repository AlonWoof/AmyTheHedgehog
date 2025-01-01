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
			GameManager.Instance.disableInput();


			if (mPlayer)
			{
				mPlayer.transform.position = Vector3.up * 1000000.0f;
			}

			sleepyAmyAnimator.Play("Sleep");
			wakeup_ZoomCam.SetActive(true);

			UIManager.Instance.fadeScreen(false, 0.01f, false);

			wakeupCam_anim.Play("Start");
			UIManager.Instance.fadeScreen(true, 3.0f, false);
			
			enableSkip();

			yield return Timing.WaitForSeconds(5.0f);

			

			sleepyAmyAnimator.Play("WakeUp");
			yield return Timing.WaitForSeconds(6.0f);

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
			GameManager.Instance.enableInput();

			endCutscene();
		}

		public override void endCutscene()
        {
			base.endCutscene();

			TimedDestroy dest = gameObject.AddComponent<TimedDestroy>();
			dest.lifetime = 1.0f;
        }

        protected override void skipCutscene()
        {
            base.skipCutscene();
			UIManager.Instance.fadeScreen(false, 0.01f, false);

			GameManager.Instance.enableInput();

			PlayerManager.Instance.playerCheckpoint.transform.position = checkpoint.transform.position;
			PlayerManager.Instance.playerCheckpoint.transform.rotation = checkpoint.transform.rotation;


			mPlayer = PlayerManager.Instance.spawnPlayerAtCheckpoint();

			sleepyAmyAnimator.gameObject.SetActive(false);
			wakeup_ZoomCam.SetActive(false);
			mPlayer.tpc.centerBehindPlayer();

			UIManager.Instance.fadeScreen(true, 3.0f);

			TimedDestroy dest = gameObject.AddComponent<TimedDestroy>();
			dest.lifetime = 1.0f;
		}

    }
}
