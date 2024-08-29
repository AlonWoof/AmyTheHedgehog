using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	public enum PauseMenuChoices
	{
		Continue,
		Return,
		Options,
		Exit
	}

	[System.Serializable]
	public class PauseMenuEntry
    {
		public string label;
		public PauseMenuChoices choice;
		public bool isAvailable = true;
		public bool isVisible = true;
    }

	public class PauseMenu : MonoBehaviour
	{

		public GameObject pauseText;
		public CanvasGroup pauseMenuGroup;
		public OptionsMenu optionsScreen;
		public List<PauseMenuEntry> menuEntry;
		public List<Text> menuText;

		public Color highlightColor;
		public Color defaultColor;
		public Color unavailableColor;

		public AudioSource sfx;

		int currentChoice = 0;

		bool selectionDisabled = false;
		bool inOptionsMenu = false;

		const int defaultSize = 8;

	    // Start is called before the first frame update
	    void Start()
	    {
	        for(int i = 0; i < menuEntry.Count; i++)
            {
				menuText[i].text = menuEntry[i].label;
            }

			optionsScreen.selectionDisabled = true;
		}
	
	    // Update is called once per frame
	    void Update()
	    {

			if (GameManager.Instance.gamePaused )
			{

				if (Input.GetButton("Select") || !UIManager.Instance.hudEnabled || inOptionsMenu)
				{
					pauseMenuGroup.alpha = 0.0f;
				}
				else
				{

					pauseMenuGroup.alpha = Mathf.Lerp(pauseMenuGroup.alpha, 1.0f, 0.25f);

					if (!pauseText.activeInHierarchy)
					{
						pauseText.SetActive(true);
						optionsScreen.selectionDisabled = true;
						sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_pauseSound);
						updateAvailibility();
						currentChoice = 0;
					}
				}
			}

			if (!GameManager.Instance.gamePaused && pauseText.activeInHierarchy)
			{
				selectionDisabled = false;
				inOptionsMenu = false;
				pauseMenuGroup.alpha = Mathf.Lerp(pauseMenuGroup.alpha, 0.0f, 0.25f);

				if(pauseMenuGroup.alpha < 0.001f)
					pauseText.SetActive(false);
			}


			if(inOptionsMenu)
            {
				if(optionsScreen.selectionDisabled)
					optionsScreen.selectionDisabled = false;

				if (!optionsScreen.gameObject.activeInHierarchy)
					optionsScreen.gameObject.SetActive(true);
			}
			else if (!inOptionsMenu)
            {
				if(!optionsScreen.selectionDisabled)
					optionsScreen.selectionDisabled = true;
			}

			if(optionsScreen.canvasGroup.alpha < 0.01f && !inOptionsMenu)
            {
				optionsScreen.gameObject.SetActive(false);
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

					if (PlayerManager.Instance.isPrologue)
						entry.isVisible = false;
					else
						entry.isVisible = true;
                }

				if (!entry.isVisible)
					entry.isAvailable = false;
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

			if (inOptionsMenu)
			{
				if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Pause"))
					inOptionsMenu = false;

				return;
			}

			if(Input.GetButtonDown("RightBumper") || Input.GetKeyDown(KeyCode.R))
            {
				Timing.RunCoroutine(doScreenshot(), Segment.RealtimeUpdate);
            }

			if (Input.GetKeyDown(KeyCode.W) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Up))
			{
				currentChoice--;
				//selectSound.Play();
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
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
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
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

				if (menuEntry[i].isVisible && !t.gameObject.activeInHierarchy)
				{
					t.gameObject.SetActive(true);
					t.rectTransform.sizeDelta = new Vector2(t.rectTransform.sizeDelta.x, defaultSize);
				}
				else if (!menuEntry[i].isVisible && t.gameObject.activeInHierarchy)
				{
					t.gameObject.SetActive(false);
					t.rectTransform.sizeDelta = new Vector2(t.rectTransform.sizeDelta.x, 0);
				}
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
				case PauseMenuChoices.Options:
					inOptionsMenu = true;
					break;
				case PauseMenuChoices.Exit:
					SaveGame.writeSaveGame(PlayerManager.Instance.saveFileSlot);
					GameManager.Instance.loadTitleScreen();
					break;
			}
			sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_confirmSound);
		}

		public IEnumerator<float> doScreenshot()
        {
			UIManager.Instance.hudEnabled = false;

			yield return Timing.WaitForSeconds(0.01f);

			string screenLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			screenLocation += "/Amy the Hedgehog/";

			if (!System.IO.Directory.Exists(screenLocation))
			{
				System.IO.Directory.CreateDirectory(screenLocation);
            }

			string fileName = "AmyScreenshot_" + System.DateTime.Now.ToFileTime() + ".png";

			ScreenCapture.CaptureScreenshot(screenLocation + fileName, 2);
			yield return Timing.WaitForSeconds(0.01f);

			UIManager.Instance.fadeScreen(false, 0.01f, true);
			yield return Timing.WaitForSeconds(0.02f);
			UIManager.Instance.fadeScreen(true, 0.68f, true);

			yield return Timing.WaitForSeconds(0.75f);
			UIManager.Instance.hudEnabled = true;
		}
	}
}
