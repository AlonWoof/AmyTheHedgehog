using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class WarningDialog : MonoBehaviour
	{

		
		public List<Text> choiceText;
		public AudioSource sfx;

		public int currentChoice = 0;
		public bool choiceConfirmed = false;
		public Animator mAnimator;

	    // Start is called before the first frame update
	    void Start()
	    {
			currentChoice = 0;
			choiceConfirmed = false;
			clampValues();
		}

        private void OnEnable()
        {
			currentChoice = 0;
			choiceConfirmed = false;
			clampValues();
		}

        // Update is called once per frame
        void Update()
	    {
			if (!choiceConfirmed)
				handleInput();

			
		}

		void clampValues()
        {
			if (currentChoice < 0)
				currentChoice = 1;

			if (currentChoice > 1)
				currentChoice = 0;

			updateHighlighted();

		}

		void handleInput()
        {
			if (Input.GetKeyDown(KeyCode.A) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Left))
			{
				currentChoice--;
				//selectSound.Play();
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
				clampValues();
			}


			if (Input.GetKeyDown(KeyCode.D) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Right))
			{
				currentChoice++;
				//selectSound.Play();
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
				clampValues();
			}

			if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Action"))
			{
				confirmSelection();
			}

		}

		void confirmSelection()
        {
			choiceConfirmed = true;
        }

		void updateHighlighted()
        {
			for(int i = 0; i < 2; i++)
            {
				choiceText[i].color = SystemColors.choiceDefaultColor;
				choiceText[i].transform.localScale = Vector3.one * 0.98f;

				if (i == currentChoice)
                {
					choiceText[i].color = SystemColors.choiceHighlightColor;
					choiceText[i].transform.localScale = Vector3.one * 1.0f;
				}
            }
        }
	}
}
