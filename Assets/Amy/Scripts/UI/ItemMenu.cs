using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ItemMenu : MonoBehaviour
	{

		public List<ItemMenuSelection> menuEntry;
		public Text descriptionText;
		public float selectTimeout = 0.5f;
		int currentChoice = 0;
		public bool selectionDisabled = false;

		int itemCount = 0;
		public CanvasGroup canvasGroup;
		public CanvasGroup descriptBoxGroup;
		public Animator mAnimator;

		public Image menuBackground;
		public Image descriptionBackground;
		public Image statusMenuBackground;

		public Text healthText;
		public Text staminaText;
		public Text moodText;


		PlayerStatus pstats;

		// Start is called before the first frame update
		void Start()
	    {
			populateItemList();

		}

        private void OnEnable()
        {
			populateItemList();

		}

        // Update is called once per frame
        void Update()
	    {
			if (!PlayerManager.Instance.mPlayerInstance)
				return;

			pstats = PlayerManager.Instance.mPlayerInstance.getStatus();
			//updateAvailibility();
			highlightChoice();
			handleInput();

			//Less frequently because strings.
			if (Time.frameCount % 2 == 0)
				updateStatusScreen();
		}

		public void updateStatusScreen()
        {
			healthText.text = Mathf.RoundToInt(pstats.currentHealth) + " / " + Mathf.RoundToInt(pstats.maxHealth);
			staminaText.text = Mathf.RoundToInt(pstats.currentMagic) + " / " + Mathf.RoundToInt(pstats.maxMagic);
			moodText.text = PlayerManager.Instance.getMoodLabel();

        }

		public void highlightChoice()
		{
			clampValues();

			if (!menuEntry[currentChoice])
				return;

			if (pstats.items.Count == 0)
				return;

			for (int i = 0; i < menuEntry.Count; i++)
			{
				Text t = menuEntry[i].mText;
				t.transform.localScale = Vector3.Lerp(t.transform.localScale, Vector3.one * 0.9f, 0.25f);

				if (!menuEntry[i].isAvailable)
					t.color = SystemColors.choiceInactiveColor;
				else
					t.color = SystemColors.choiceDefaultColor;
			}

			menuEntry[currentChoice].mText.transform.localScale = Vector3.Lerp(menuEntry[currentChoice].mText.transform.localScale, Vector3.one * 1.0f, 0.25f);
			menuEntry[currentChoice].mText.color = SystemColors.choiceHighlightColor;
		}

		void clampValues()
		{
			if (currentChoice > itemCount)
				currentChoice = 0;

			if (currentChoice < 0)
				currentChoice = itemCount;

			updateDescription();
		}

		

		public void handleInput()
		{



			if (selectTimeout > 0)
			{
				selectTimeout -= Time.unscaledDeltaTime;
				return;

			}

			if (selectionDisabled)
				return;

			if (pstats.items.Count == 0)
			{
				currentChoice = 0;
				return;
			}

			if (Input.GetKeyDown(KeyCode.W) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Up))
			{
				currentChoice--;
				//selectSound.Play();
				//sfx.PlayOneShot(moveSound);
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
				//sfx.PlayOneShot(moveSound);
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

		void confirmSelection()
        {
			if (menuEntry[currentChoice].mData == null)
				return;

			PlayerManager.Instance.useItem(menuEntry[currentChoice].mData);
			PlayerManager.Instance.closeItemMenu();
			selectTimeout = 1.0f;
		}

		void updateDescription()
        {
			if (menuEntry[currentChoice].mData == null)
			{
				descriptionText.text = "";
				descriptBoxGroup.alpha = 0.0f;
				return;
			}

			descriptBoxGroup.alpha = 1.0f;
			descriptionText.text = menuEntry[currentChoice].mData.description;

		}

		void populateItemList()
        {
			if (!PlayerManager.Instance.mPlayerInstance)
				return;


			Color menuColor = Color.Lerp(SystemColors.AmyColor, Color.gray, 0.25f);

			if(PlayerManager.Instance.currentCharacter == PlayableCharacter.Cream)
				menuColor = Color.Lerp(SystemColors.CreamColor, Color.gray, 0.25f);

			menuColor.a = 0.75f;

			menuBackground.color = menuColor;
			descriptionBackground.color = menuColor;
			statusMenuBackground.color = menuColor;

			itemCount = pstats.items.Count - 1;

			foreach (ItemMenuSelection ims in menuEntry)
			{
				ims.mData = null;
			}


			for (int i = 0; i < pstats.items.Count; i++)
            {
				if (i > 9)
					continue;

				menuEntry[i].mData = pstats.items[i];
				
			}

			foreach(ItemMenuSelection ims in menuEntry)
            {
				ims.refreshLabel();

			}
        }

		public void openMenu()
        {
			if (selectTimeout > 0.0f)
				return;

			if (PlayerManager.Instance.itemMenuOpen)
				return;

			Time.timeScale = 0.0f;
			GameManager.Instance.mainCamera.GetComponent<Cinemachine.CinemachineBrain>().m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.LateUpdate;

			populateItemList();
			mAnimator.Play("Appear");
			selectTimeout = 0.6f;

			PlayerManager.Instance.itemMenuOpen = true;
			selectionDisabled = false;
		}

		public void closeMenu()
        {
			if (selectTimeout > 0.0f)
				return;

			if (!PlayerManager.Instance.itemMenuOpen)
				return;

			Time.timeScale = 1.0f;
			GameManager.Instance.mainCamera.GetComponent<Cinemachine.CinemachineBrain>().m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate;

			populateItemList();
			mAnimator.Play("Disappear");
			selectTimeout = 0.6f;

			PlayerManager.Instance.itemMenuOpen = false;
			selectionDisabled = true;
		}
	}
}
