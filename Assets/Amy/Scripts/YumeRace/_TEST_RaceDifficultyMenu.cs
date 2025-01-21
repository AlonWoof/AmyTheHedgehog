using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class _TEST_RaceDifficultyMenu : MonoBehaviour
	{
		public AudioSource sfx;
		public Racetrack track;
		public GameObject yume_easy;
		public GameObject yume_medium;
		public GameObject yume_hard;

		public int selection;

		public GameObject sel_easy;
		public GameObject sel_medium;
		public GameObject sel_hard;

	    // Start is called before the first frame update
	    void Start()
	    {
			updateSelection();

		}
	
	    // Update is called once per frame
	    void Update()
	    {
			handleInput();

		}

		void clampValues()
        {
			if (selection < 0)
				selection = 2;

			if (selection > 2)
				selection = 0;
        }

		void handleInput()
        {

			if (Input.GetKeyDown(KeyCode.W) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Up))
			{
				selection--;
				//selectSound.Play();
				//sfx.PlayOneShot(moveSound);
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
				clampValues();
				updateSelection();

			}

			if (Input.GetKeyDown(KeyCode.S) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Down))
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
		}

		void confirmSelection()
        {
			gameObject.SetActive(false);
			track.startRace();
        }

		void updateSelection()
        {
			sel_easy.transform.localScale = Vector3.one * 0.8f;
			sel_medium.transform.localScale = Vector3.one * 0.8f;
			sel_hard.transform.localScale = Vector3.one * 0.8f;

			yume_easy.SetActive(false);
			yume_medium.SetActive(false);
			yume_hard.SetActive(false);

			switch ((RaceDifficulty)selection)
            {
				case RaceDifficulty.Easy:
					sel_easy.transform.localScale = Vector3.one * 1.2f;
					yume_easy.SetActive(true);
					break;

				case RaceDifficulty.Medium:
					sel_medium.transform.localScale = Vector3.one * 1.2f;
					yume_medium.SetActive(true);
					break;

				case RaceDifficulty.Hard:
					sel_hard.transform.localScale = Vector3.one * 1.2f;
					yume_hard.SetActive(true);
					break;
			}

			track.difficulty = (RaceDifficulty)selection;
        }
	}
}
