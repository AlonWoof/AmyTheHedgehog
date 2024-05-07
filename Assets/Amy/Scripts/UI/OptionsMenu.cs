using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{






	public class OptionsMenu : MonoBehaviour
	{
		public CanvasGroup canvasGroup;
		public Color highlightColor;
		public Color defaultColor;
		public Color unavailableColor;

		public AudioSource sfx;
		public AudioClip moveSound;
		public AudioClip pauseSound;
		public AudioClip confirmSound;

		public List<OptionsMenuEntry> menuEntry;
		int currentChoice = 0;
		public bool selectionDisabled = false;

		float timeout = 0.5f;

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }

        private void OnEnable()
        {
			timeout = 0.15f;

		}

        private void OnDisable()
        {
            
        }

        // Update is called once per frame
        void Update()
	    {
			updateAvailibility();
			highlightChoice();
			handleInput();
			updateCheckedStatus();

			if(!selectionDisabled)
            {
				canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1.0f, 0.25f);
			}
			else
            {
				canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0.0f, 0.25f);
			}
		}

		public void updateAvailibility()
		{
			foreach (OptionsMenuEntry entry in menuEntry)
			{
				entry.isAvailable = true;
			}
		}

		void clampValues()
		{
			if (currentChoice > (menuEntry.Count - 1))
				currentChoice = 0;

			if (currentChoice < 0)
				currentChoice = (menuEntry.Count - 1);
		}

		public void highlightChoice()
		{
			clampValues();

			if (!menuEntry[currentChoice])
				return;

			for (int i = 0; i < menuEntry.Count; i++)
			{
				Text t = menuEntry[i].text;
				t.transform.localScale = Vector3.Lerp(t.transform.localScale, Vector3.one * 0.9f, 0.25f);

				if (!menuEntry[i].isAvailable)
					t.color = unavailableColor;
				else
					t.color = defaultColor;
			}

			menuEntry[currentChoice].text.transform.localScale = Vector3.Lerp(menuEntry[currentChoice].text.transform.localScale, Vector3.one * 1.0f, 0.25f);
			menuEntry[currentChoice].text.color = highlightColor;
		}

		void updateCheckedStatus()
        {
			foreach(OptionsMenuEntry e in menuEntry)
            {
				switch(e.type)
                {
					case OptionEntryType.InvertPitch:
						e.isChecked = GameManager.Instance.config.pitchInvert;
						break;
					case OptionEntryType.InvertYaw:
						e.isChecked = GameManager.Instance.config.yawInvert;
						break;
                }
            }
        }

		void confirmSelection()
		{
			switch (menuEntry[currentChoice].type)
			{
				case OptionEntryType.InvertYaw:
					GameManager.Instance.config.yawInvert = !GameManager.Instance.config.yawInvert;
					break;

				case OptionEntryType.InvertPitch:
					GameManager.Instance.config.pitchInvert = !GameManager.Instance.config.pitchInvert;
					break;
			}

			sfx.PlayOneShot(confirmSound);
		}

		public void handleInput()
		{

			if (selectionDisabled)
				return;

			if(timeout > 0)
            {
				timeout -= Time.unscaledDeltaTime;
				return;

			}

			if (Input.GetKeyDown(KeyCode.W) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Up))
			{
				currentChoice--;
				//selectSound.Play();
				sfx.PlayOneShot(moveSound);
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
				sfx.PlayOneShot(moveSound);
				clampValues();

				//Skiparoo
				if (!menuEntry[currentChoice].isAvailable)
				{
					currentChoice++;
					clampValues();
				}
			}

			if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Action"))
			{
				confirmSelection();

			}

		}
	}
}
