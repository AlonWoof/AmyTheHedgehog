using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public enum SuikaMainMenuChoice
    {
		Buy,
		Sell,
		Talk,
		Quit
    }

	[System.Serializable]
	public class SuikaMainMenuEntry
    {
		public SuikaMainMenuChoice choice;

		public bool isAvailable = true;
		public bool isHighligted = false;

		public GameObject gameObject;
		public Text mText;
    }

	public class SuikaMartMainMenu : MonoBehaviour
	{
		public CanvasGroup mCanvasGroup;
		public List<SuikaMainMenuEntry> menuEntry;
		public SuikaMartShopMenu buyMenu;
		public int selection = 0;
		public bool inputEnabled = false;
		public bool menuVisible = false;
		public SuikaNPC suika;

	    // Start is called before the first frame update
	    void Start()
	    {
			updateSelection();

		}
	
	    // Update is called once per frame
	    void Update()
	    {
			if(menuVisible)
            {
				mCanvasGroup.alpha = Mathf.Lerp(mCanvasGroup.alpha, 1.0f, 0.1f);
            }
			else
            {
				mCanvasGroup.alpha = Mathf.Lerp(mCanvasGroup.alpha, 0.0f, 0.1f);
			}

			updateSelection();
		}

		void handleInput()
        {
			if (Input.GetKeyDown(KeyCode.W) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Up))
			{
				selection--;
				//selectSound.Play();
				//sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
				clampValues();

				//Skiparoo
				if (!menuEntry[selection].isAvailable)
				{
					selection--;
					clampValues();
				}

				updateSelection();
			}


			if (Input.GetKeyDown(KeyCode.S) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Down))
			{
				selection++;
				//selectSound.Play();
				//sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
				clampValues();

				//Skiparoo
				if (!menuEntry[selection].isAvailable)
				{
					selection++;
					clampValues();
				}

				updateSelection();
			}


		}

		public CoroutineHandle enableMenu()
        {
			return Timing.RunCoroutine(doMenu(), gameObject);
        }

		IEnumerator<float> doMenu()
        {
			selection = 0;

			menuStart:

			menuVisible = true;
			yield return Timing.WaitForSeconds(0.5f);
			inputEnabled = true;

			bool choiceMade = false;

			while(!choiceMade)
            {
				yield return 0f;

				handleInput();
				clampValues();


				if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Action"))
					choiceMade = true;
			}

			inputEnabled = false;
			menuVisible = false;

			CoroutineHandle menuCoroutine = new CoroutineHandle();

			bool loopBack = true;

			switch(menuEntry[selection].choice)
            {
				case SuikaMainMenuChoice.Buy:
					menuCoroutine = buyMenu.openShopMenu();
					break;

				case SuikaMainMenuChoice.Sell:
					break;

				case SuikaMainMenuChoice.Talk:
					menuCoroutine = suika.dailyMessage();
					break;

				case SuikaMainMenuChoice.Quit:
					loopBack = false;
					break;
            }

			while (menuCoroutine.IsRunning)
			{
				yield return 0f;
			}

			if (loopBack)
				goto menuStart;

		}

		void clampValues()
		{
			if (selection < 0)
				selection = menuEntry.Count - 1;

			if (selection > menuEntry.Count-1)
				selection = 0;
		}

		void updateSelection()
		{

			foreach(SuikaMainMenuEntry e in menuEntry)
            {
				e.mText.color = Color.white;
				e.mText.transform.localScale = Vector3.one * 1.0f;
			}

			menuEntry[selection].mText.color = Color.yellow;
			menuEntry[selection].mText.transform.localScale = Vector3.one * (1.1f + (0.05f * Mathf.Sin(Time.time*3)));

			//track.difficulty = (RaceDifficulty)selection;
		}
	}
}
