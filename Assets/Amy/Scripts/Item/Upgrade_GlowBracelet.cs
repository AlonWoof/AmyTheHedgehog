using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Upgrade_GlowBracelet : Upgrade
	{

		public Message itemGetMessage;
		public GameObject braceletGetScene;

		private void Awake()
        {
			character = PlayableCharacter.Cream;
		}

        // Update is called once per frame
        void Update()
	    {
	        
	    }

		protected override bool playerHasItem()
		{
			return PlayerManager.Instance.hasGlowBracelet;
		}


		protected override void doItemGetScene()
		{
			//Insert some fancy cutscene or something.
			mAnimator.Play("Disappear");

			Timing.RunCoroutine(itemGetSceneRoutine(), gameObject);
		}

		IEnumerator<float> itemGetSceneRoutine()
		{

			GameManager.Instance.disableInput();
			GameManager.Instance.cutsceneMode = true;

			UIManager.Instance.fadeScreen(false, 0.25f, true);

			Player pl = PlayerManager.Instance.getPlayer();

			pl.transform.position = transform.position;

			pl.clearAccel();
			pl.clearSpeed();

			yield return Timing.WaitForSeconds(0.75f);

			pl.changeCurrentMode(PlayerModes.CUTSCENE);
			pl.transform.position = transform.position + Vector3.up * 10000.0f;

			GameObject cutsceneObject = GameObject.Instantiate(braceletGetScene);
			PlayerManager.Instance.hasGlowBracelet = true;

			cutsceneObject.transform.position = transform.position;
			cutsceneObject.transform.rotation = transform.rotation;

			UIManager.Instance.fadeScreen(true, 1.0f, true);

			yield return Timing.WaitForSeconds(1.0f);
			GameManager.Instance.playSystemSound(GameManager.Instance.systemData.AUDIO_itemGetJingle, 0.5f);

			CoroutineHandle messageTask = UIManager.Instance.messageBox.showMessageBox(itemGetMessage);

			while (messageTask.IsRunning)
				yield return 0f;

			UIManager.Instance.fadeScreen(false, 0.25f, true);

			yield return Timing.WaitForSeconds(0.75f);

			pl.framesAirborne = -5;
			pl.changeCurrentMode(PlayerModes.NORMAL);
			pl.transform.position = transform.position;
			pl.setAngleInstantly(transform.forward);
			pl.tpc.centerBehindPlayer();

			yield return Timing.WaitForSeconds(0.75f);

			Destroy(cutsceneObject);

			yield return Timing.WaitForSeconds(0.75f);

			UIManager.Instance.fadeScreen(true, 1.0f, true);

			yield return Timing.WaitForSeconds(0.75f);

			GameManager.Instance.enableInput();
			GameManager.Instance.cutsceneMode = false;

			Invoke("Die", 5.0f);
		}
	}
}
