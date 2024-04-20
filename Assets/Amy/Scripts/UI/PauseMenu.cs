using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	public enum PauseMenuChoices
	{
		Continue,
		Return,
		Exit
	}

	[System.Serializable]
	public class PauseMenuEntry
    {
		public string label;
		public PauseMenuChoices choice;
		public bool isAvailable = true;
    }

	public class PauseMenu : MonoBehaviour
	{



		public GameObject pauseText;
		public CanvasGroup pauseMenuGroup;
		public List<PauseMenuEntry> menuEntry;
		public List<Text> menuText;

		public Color highlightColor;
		public Color defaultColor;
		public Color unavailableColor;

		public AudioSource selectSound;

		int currentChoice = 0;

		bool selectionDisabled = false;

	    // Start is called before the first frame update
	    void Start()
	    {
	        for(int i = 0; i < menuEntry.Count; i++)
            {
				menuText[i].text = menuEntry[i].label;
            }
	    }
	
	    // Update is called once per frame
	    void Update()
	    {

			if (GameManager.Instance.gamePaused)
			{
				pauseMenuGroup.alpha = Mathf.Lerp(pauseMenuGroup.alpha, 1.0f, 0.25f);

				if (!pauseText.activeInHierarchy)
				{
					pauseText.SetActive(true);
					updateAvailibility();
					currentChoice = 0;
				}
			}

			if (!GameManager.Instance.gamePaused && pauseText.activeInHierarchy)
			{
				selectionDisabled = false;
				pauseMenuGroup.alpha = Mathf.Lerp(pauseMenuGroup.alpha, 0.0f, 0.25f);

				if(pauseMenuGroup.alpha < 0.001f)
					pauseText.SetActive(false);
			}


			if (!GameManager.Instance.gamePaused)
				return;

			updateAvailibility();
			highlightChoice();
			handleInput();

		}

		public void updateAvailibility()
        {
			foreach(PauseMenuEntry entry in menuEntry)
            {
				entry.isAvailable = true;

				if (entry.choice == PauseMenuChoices.Return)
                {
					if(!PlayerManager.Instance.getPlayer().canWarp())
						entry.isAvailable = false;
                }
            }
        }


		void clampValues()
        {
			if (currentChoice > (menuEntry.Count - 1))
				currentChoice = 0;

			if (currentChoice < 0)
				currentChoice = (menuEntry.Count - 1);
		}

		public void handleInput()
        {
			if (selectionDisabled)
				return;


			if (Input.GetKeyDown(KeyCode.W) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Up))
			{
				currentChoice--;
				//selectSound.Play();
				clampValues();

				//Skiparoo
				if (!menuEntry[currentChoice].isAvailable)
				{
					currentChoice--;
					clampValues();
				}
			}


			if (Input.GetKeyDown(KeyCode.S) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Down))
			{
				currentChoice++;
				//selectSound.Play();
				clampValues();

				//Skiparoo
				if (!menuEntry[currentChoice].isAvailable)
				{
					currentChoice++;
					clampValues();
				}
			}


			if(Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Action"))
            {
				confirmSelection();

			}
		}

		public void highlightChoice()
        {
			clampValues();

			if (!menuText[currentChoice])
				return;


			for(int i = 0; i < menuEntry.Count; i++)
            {
				Text t = menuText[i];
				t.transform.localScale = Vector3.Lerp(t.transform.localScale, Vector3.one * 0.9f, 0.25f);

				if(!menuEntry[i].isAvailable)
					t.color = unavailableColor;
				else
					t.color = defaultColor;
			}

			menuText[currentChoice].transform.localScale = Vector3.Lerp(menuText[currentChoice].transform.localScale, Vector3.one * 1.2f, 0.25f);
			menuText[currentChoice].color = highlightColor;
		}

		void confirmSelection()
        {
			switch(menuEntry[currentChoice].choice)
            {
				case PauseMenuChoices.Continue:
					GameManager.Instance.unPauseGame();
					selectionDisabled = true;
					break;
				case PauseMenuChoices.Return:
					GameManager.Instance.unPauseGame();
					PlayerManager.Instance.getPlayer().startWarp("WarpCenter", 7);
					break;
				case PauseMenuChoices.Exit:
					SaveGame.writeSaveGame(0);
					Application.Quit();
					break;
			}
        }
	}
}
