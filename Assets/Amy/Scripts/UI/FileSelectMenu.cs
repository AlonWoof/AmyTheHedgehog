using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{



	public class FileSelectMenu : MonoBehaviour
	{

		bool selectionDisabled;
		float timeout = 0.2f;
		public List<FileSelectMenuEntry> menuEntries;
		public int choiceOffset = 1;
		public int currentChoice = 1;

		public Color highlightColor;
		public Color defaultColor;
		public Color unavailableColor;

		public WarningDialog warningDialog;

		public AudioSource sfx;

		// Start is called before the first frame update
		void Start()
	    {
			updateHighlighted();
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			handleInput();
			clampValues();

		}

		void handleInput()
		{
			if (selectionDisabled)
				return;

			if (timeout > 0)
			{
				timeout -= Time.unscaledDeltaTime;
				return;

			}

			if (Input.GetKeyDown(KeyCode.W) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Up))
			{
				currentChoice--;
				//selectSound.Play();
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
				updateHighlighted();

			}

			if (Input.GetKeyDown(KeyCode.S) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Down))
			{
				currentChoice++;
				//selectSound.Play();
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
				updateHighlighted();

			}

			if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Action"))
			{
				confirmSelection();
			}

			if(Input.GetKeyDown(KeyCode.Delete) || Input.GetButtonDown("Jump"))
            {
				deleteFile();

			}
		}

		void confirmSelection()
        {
			PlayerManager.Instance.saveFileSlot = currentChoice;

			MusicManager.Instance.fadeBGM(0, 1.0f);

			if (!SaveGame.isSaveValid(currentChoice))
			{
				PlayerManager.Instance.startNewGame();
			}
			else
			{
				
				SaveGame.loadGame(PlayerManager.Instance.saveFileSlot);

				if(PlayerManager.Instance.isPrologue)
                {
					GameManager.Instance.loadScene("Jungle", true);
                }
				else
                {
					PlayerManager.Instance.wakeupScene(true);
				}

				
			}

			selectionDisabled = true;
        }

		void deleteFile()
        {

			if (!SaveGame.isSaveValid(currentChoice))
				return;

			Timing.RunCoroutine(deleteDialog());
        }

		IEnumerator<float> deleteDialog()
        {
			selectionDisabled = true;

			warningDialog.gameObject.SetActive(true);

			while(!warningDialog.choiceConfirmed)
            {
				yield return 0f;
			}

			if(warningDialog.currentChoice == 1)
            {
				SaveGame.deleteSaveFile(currentChoice);
            }

			warningDialog.mAnimator.Play("Disappear");
			yield return Timing.WaitForSeconds(0.25f);

			warningDialog.gameObject.SetActive(false);

			selectionDisabled = false;

			yield return 0f;
        }

		void clampValues()
        {
			while(currentChoice > choiceOffset + 3)
				choiceOffset++;

			if (currentChoice < choiceOffset)
				choiceOffset--;

			if (choiceOffset < 1)
				choiceOffset = 1;

			if (choiceOffset > 96)
				choiceOffset = 1;

			if (currentChoice < 1)
				currentChoice = 1;

			if (currentChoice > 99)
				currentChoice = 99;

			for (int i = 0; i < menuEntries.Count; i++)
            {
				menuEntries[i].setFileIndex(choiceOffset + i);
            }
		}

		void updateHighlighted()
        {
			clampValues();

			foreach (FileSelectMenuEntry e in menuEntries)
            {
				e.isSelected = false;
				e.transform.localScale = Vector3.one * 0.98f;
				e.mColor = defaultColor;

				if (e.fileIndex == currentChoice)
				{
					e.isSelected = true;
					e.transform.localScale = Vector3.one;
					e.mColor = highlightColor;
				}
            }
        }

	}
}
