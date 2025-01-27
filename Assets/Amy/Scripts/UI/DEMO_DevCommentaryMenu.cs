using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class DEMO_DevCommentaryMenu : MonoBehaviour
	{
		public int selection;
		public Text yesText;
		public Text noText;
		public AudioSource sfx;
		public Animator mAnimator;

		float timeout = 1.75f;

		bool appeared = false;
		bool selected = false;

	    // Start is called before the first frame update
	    void Start()
	    {
			updateSelection();

		}
	
	    // Update is called once per frame
	    void Update()
	    {

			if(timeout > 0)
            {
				timeout -= Time.deltaTime;

				if (timeout < 0.25f && !appeared)
				{
					sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_menuAppear);
					mAnimator.Play("Appear");
					appeared = true;
				}

				return;
            }				

			if(!selected)
				handleInput();
	    }

		void clampValues()
		{
			if (selection < 0)
				selection = 1;

			if (selection > 1)
				selection = 0;
		}


		void handleInput()
        {
			if (Input.GetKeyDown(KeyCode.A) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Left))
			{
				selection--;
				//selectSound.Play();
				//sfx.PlayOneShot(moveSound);
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
				clampValues();
				updateSelection();

			}

			if (Input.GetKeyDown(KeyCode.D) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Right))
			{
				selection++;
				//selectSound.Play();
				//sfx.PlayOneShot(moveSound);
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);

				clampValues();
				updateSelection();

			}

			if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Action"))
			{
				confirmSelection();
			}

			void confirmSelection()
			{

				if(selection == 0)
                {
					PlayerManager.Instance.setStoryFlag("DEVCOMMENT", true);
                }

				//sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_confirmSound);
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_menuDisappear);

				mAnimator.Play("Disappear");
				GameManager.Instance.loadScene("Jungle", true);
			}

	
		}

		void updateSelection()
		{
			yesText.transform.localScale = Vector3.one * 1.0f;
			noText.transform.localScale = Vector3.one * 1.0f;

			yesText.color = Color.white;
			noText.color = Color.white;

			switch (selection)
			{
				case 0:
					//yesText.transform.localScale = Vector3.one * 1.2f;
					yesText.color = Color.Lerp(Color.yellow, Color.white, 0.25f);
					break;

				case 1:
					//noText.transform.localScale = Vector3.one * 1.2f;
					noText.color = Color.Lerp(Color.yellow, Color.white, 0.25f);
					break;

			}

			//track.difficulty = (RaceDifficulty)selection;
		}
	}
}
