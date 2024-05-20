using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2023 AlonWoof            //
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
<<<<<<< Updated upstream
=======
		public GameObject cam3_alt;
>>>>>>> Stashed changes
		public GameObject cam4;

		public AudioSource amyVoice;
		public AudioClip amy_wakingUp;
		public AudioClip amy_are;
		public AudioClip amy_thinking;
		public AudioClip amy_nakedKya;
<<<<<<< Updated upstream
=======
		public AudioClip amy_thinking2;
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======
			cam3_alt.SetActive(false);
>>>>>>> Stashed changes
			cam4.SetActive(false);

			if (mPlayer)
			{
				mPlayer.transform.position = Vector3.up * 1000000.0f;
			}

			yield return Timing.WaitForSeconds(11.0f);
			amyVoice.volume = 0.15f;
			amyVoice.PlayOneShot(amy_wakingUp);

			amyAnimator.Play("GetUp");

			yield return Timing.WaitForSeconds(3.0f);

			cam0.SetActive(false);
			cam1.SetActive(true);

			yield return Timing.WaitForSeconds(4.0f);

			amyAnimator.Play("word_are");
			amyVoice.volume = 0.5f;
			amyVoice.PlayOneShot(amy_are);
			CoroutineHandle msg = UIManager.Instance.messageBox.showMessageBox("Huh...? Where am I...?");

			while(msg.IsRunning)
            {
				yield return 0f;
            }

			amyAnimator.CrossFade("LookUp", 0.25f);
			yield return Timing.WaitForSeconds(0.5f);

			cam2.SetActive(true);
			cam1.SetActive(false);


			yield return Timing.WaitForSeconds(1.0f);
			amyAnimator.Play("word_jungle");
			amyVoice.PlayOneShot(amy_thinking);
			msg = UIManager.Instance.messageBox.showMessageBox("Some kind of jungle?");

			while (msg.IsRunning)
			{
				yield return 0f;
			}

			if (!SaveGame.readSADXNudeModData())
			{
				amyAnimator.Play("word_are");
				msg = UIManager.Instance.messageBox.showMessageBox("...Also...");

				while (msg.IsRunning)
				{
					yield return 0f;
				}

				yield return Timing.WaitForSeconds(0.5f);

				cam2.SetActive(false);
				cam3.SetActive(true);
				amyAnimator.Play("Hazukashii");
				amyVoice.PlayOneShot(amy_nakedKya);
<<<<<<< Updated upstream
=======
				UIManager.Instance.messageBox.setMessageBoxSpeed(DialogBox.speed_fast * 0.5f);
>>>>>>> Stashed changes
				msg = UIManager.Instance.messageBox.showMessageBox("WHY AM I NAKED?!?!");

				while (msg.IsRunning)
				{
					yield return 0f;
				}

				yield return Timing.WaitForSeconds(0.5f);

<<<<<<< Updated upstream
=======
				UIManager.Instance.messageBox.setMessageBoxSpeed(DialogBox.speed_mid);
>>>>>>> Stashed changes
				amyAnimator.CrossFade("Idle", 0.5f);
				msg = UIManager.Instance.messageBox.showMessageBox("...I guess that's not important right now.");

				while (msg.IsRunning)
				{
					yield return 0f;
				}

				yield return Timing.WaitForSeconds(0.5f);
			}
<<<<<<< Updated upstream
=======
			else
            {
				cam2.SetActive(false);
				cam3_alt.SetActive(true);
				amyAnimator.Play("NoGloves");
				amyVoice.PlayOneShot(amy_thinking2);
				amyAnimator.Play("word_are");
				
				msg = UIManager.Instance.messageBox.showMessageBox("Where did my boots and gloves go?\n I'm even more naked than usual...");

				while (msg.IsRunning)
				{
					yield return 0f;
				}

				yield return Timing.WaitForSeconds(0.5f);
			}
>>>>>>> Stashed changes

			amyAnimator.CrossFade("Idle", 0.5f);

			cam2.SetActive(false);
			cam3.SetActive(false);
			cam4.SetActive(true);

			yield return Timing.WaitForSeconds(0.5f);

<<<<<<< Updated upstream

=======
			UIManager.Instance.messageBox.setMessageBoxSpeed(DialogBox.speed_mid);
>>>>>>> Stashed changes
			msg = UIManager.Instance.messageBox.showMessageBox("...I need to figure out just\nwhere the heck I am.");

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
