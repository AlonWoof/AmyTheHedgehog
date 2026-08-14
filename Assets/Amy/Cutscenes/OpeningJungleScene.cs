using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class OpeningJungleScene : BaseCutscene
	{
		public Animator amyAnimator;
		public GameObject cam0;
		public GameObject cam1;
		public GameObject cam2;
		public GameObject cam3;
		public GameObject cam3_alt;
		public GameObject cam4;

		public AudioSource amyVoice;
		public AudioClip amy_wakingUp;
		public AudioClip amy_are;
		public AudioClip amy_thinking;
		public AudioClip amy_nakedKya;
		public AudioClip amy_thinking2;
		public AudioClip amy_kokodoko;

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

			canSkipScene = true;

			cam0.SetActive(true);
			cam1.SetActive(false);
			cam2.SetActive(false);
			cam3.SetActive(false);
			cam3_alt.SetActive(false);
			cam4.SetActive(false);

			if (mPlayer)
			{
				mPlayer.transform.position = Vector3.up * 1000000.0f;
			}

			yield return Timing.WaitForSeconds(10.5f);
			amyAnimator.Play("Face_WakeUp");
			yield return Timing.WaitForSeconds(0.5f);
			amyVoice.volume = 0.15f;
			amyVoice.PlayOneShot(amy_wakingUp);
			yield return Timing.WaitForSeconds(0.1f);

			amyAnimator.Play("GetUp");

			yield return Timing.WaitForSeconds(3.0f);

			cam0.SetActive(false);
			cam1.SetActive(true);

			yield return Timing.WaitForSeconds(4.0f);

			amyAnimator.Play("word_are");
			amyVoice.volume = 0.5f;
			amyVoice.PlayOneShot(amy_are);
			CoroutineHandle msg = UIManager.Instance.messageBox.showMessageBox("Huh...? Where am I...?", SpeakerProfile.Amy);
			
			while (msg.IsRunning)
				yield return 0f;

			cam2.SetActive(true);
			cam1.SetActive(false);

			amyAnimator.CrossFade("LookUp", 0.12f);
			yield return Timing.WaitForSeconds(0.5f);


			yield return Timing.WaitForSeconds(0.5f);
			amyAnimator.Play("word_jungle");
			amyVoice.PlayOneShot(amy_thinking);
			msg = UIManager.Instance.messageBox.showMessageBox("Some kind of jungle?", SpeakerProfile.Amy);

			while (msg.IsRunning)
			{
				yield return 0f;
			}

			if (!SaveGame.readSADXNudeModData())
			{
				amyAnimator.Play("word_are");
				msg = UIManager.Instance.messageBox.showMessageBox("...Also...", SpeakerProfile.Amy);

				while (msg.IsRunning)
				{
					yield return 0f;
				}

				yield return Timing.WaitForSeconds(0.5f);

				cam2.SetActive(false);
				cam3.SetActive(true);
				amyAnimator.Play("Hazukashii");
				amyVoice.PlayOneShot(amy_nakedKya);
				UIManager.Instance.messageBox.setTextColorTint(SystemColors.AmyColor);
				UIManager.Instance.messageBox.setMessageBoxSpeed(DialogBox.speed_fast * 0.5f);
				msg = UIManager.Instance.messageBox.showMessageBox("WHY AM I NAKED?!?!", SpeakerProfile.Amy);

				while (msg.IsRunning)
				{
					yield return 0f;
				}

				yield return Timing.WaitForSeconds(0.5f);

				UIManager.Instance.messageBox.setTextColorTint(SystemColors.AmyColor);
				UIManager.Instance.messageBox.setMessageBoxSpeed(DialogBox.speed_mid);
				amyAnimator.CrossFade("Idle", 0.125f);
				msg = UIManager.Instance.messageBox.showMessageBox("...I guess that's not important right now.", SpeakerProfile.Amy);

				while (msg.IsRunning)
				{
					yield return 0f;
				}

				yield return Timing.WaitForSeconds(0.5f);
			}
			else
			{
				cam2.SetActive(false);
				cam3_alt.SetActive(true);
				amyAnimator.CrossFade("NoGloves", 0.25f);
				amyVoice.PlayOneShot(amy_thinking2);
				amyAnimator.Play("word_are");

				msg = UIManager.Instance.messageBox.showMessageBox("Where did my boots and gloves go?\n I'm even more naked than usual...", SpeakerProfile.Amy);

				while (msg.IsRunning)
				{
					yield return 0f;
				}

				yield return Timing.WaitForSeconds(0.5f);
			}

			amyAnimator.CrossFade("Idle", 0.125f);

			cam2.SetActive(false);
			cam3.SetActive(false);
			cam4.SetActive(true);

			yield return Timing.WaitForSeconds(0.5f);

			UIManager.Instance.messageBox.setMessageBoxSpeed(DialogBox.speed_mid);
			msg = UIManager.Instance.messageBox.showMessageBox("...I need to figure out just\nwhere the heck I am.", SpeakerProfile.Amy);

			yield return Timing.WaitForSeconds(0.5f);
			amyAnimator.Play("word_jungle");
			amyVoice.PlayOneShot(amy_kokodoko);

			while (msg.IsRunning)
			{
				yield return 0f;
			}

			yield return Timing.WaitForSeconds(0.5f);

			UIManager.Instance.fadeScreen(false, 0.5f, false);

			yield return Timing.WaitForSeconds(0.5f);

			endCutscene();

		}


		public override void endCutscene()
		{
			base.endCutscene();
			GameManager.Instance.loadScene("Jungle");
		}

		private void LateUpdate()
		{
			if (mPlayer)
				mPlayer.transform.position = Vector3.up * 10000.0f;

			GameManager.Instance.cutsceneMode = true;
			GameManager.Instance.disableInput();
		}


	}
}
