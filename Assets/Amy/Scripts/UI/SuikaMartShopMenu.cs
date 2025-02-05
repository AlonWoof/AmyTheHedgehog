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
	[System.Serializable]
	public class SuikaMartItemData
    {
		public string label;
		public ItemData item;
		public int itemPrice = 10;
		public int affinityRequirement = 0;
		public int emblemRequirement = 0;

    }

	public class SuikaMartShopMenu : MonoBehaviour
	{
		public CanvasGroup mCanvas;
		bool menuActive = false;

		float timeout = 0.2f;

		public List<SuikaMartItemData> itemData;
		public List<SuikaShopEntry> shopEntries;
		public int choiceOffset = 0;
		public int currentChoice = 0;

		public Color highlightColor;
		public Color defaultColor;
		public Color unavailableColor;

		public Text descriptionBox;

		public AudioSource sfx;

		// Start is called before the first frame update
		void Start()
		{
			currentChoice = 0;
			choiceOffset = 0;

			updateHighlighted();
		}

		// Update is called once per frame
		void Update()
		{
			if (menuActive)
			{
				mCanvas.alpha = Mathf.Lerp(mCanvas.alpha, 1.0f, 0.2f);

			}
			else
			{
				mCanvas.alpha = Mathf.Lerp(mCanvas.alpha, 0.0f, 0.2f);
				descriptionBox.text = "";
			}

		}

		public CoroutineHandle openShopMenu()
		{
			return Timing.RunCoroutine(doShopMenu(), gameObject);
		}

		IEnumerator<float> doShopMenu()
		{
			UIManager.Instance.fadeScreen(false, 0.25f, true);

			yield return Timing.WaitForSeconds(0.26f);
			currentChoice = 0;
			choiceOffset = 0;

			updateHighlighted();
			clampValues();

			menuActive = true;
			mCanvas.alpha = 1;
			timeout = 0.5f;
			UIManager.Instance.fadeScreen(true, 0.25f, true);

			bool doneShopping = false;

			while (!doneShopping)
			{
				handleInput();
				clampValues();
				

				if (Input.GetKeyDown(KeyCode.Delete) || Input.GetButtonDown("Cancel"))
				{
					//deleteFile();
					doneShopping = true;
				}

				yield return 0f;
			}

			UIManager.Instance.fadeScreen(false, 0.25f, true);

			yield return Timing.WaitForSeconds(0.26f);
			currentChoice = 0;
			choiceOffset = 0;

			updateHighlighted();
			clampValues();

			menuActive = false;
			mCanvas.alpha = 0;
			timeout = 1.0f;
			UIManager.Instance.fadeScreen(true, 0.25f, true);
		}

		void updateHighlighted()
		{
			clampValues();
			

			for (int i = 0; i < shopEntries.Count; i++)
            {
				if(i == currentChoice)
                {
					shopEntries[i].isSelected = true;
                }
				else
                {
					shopEntries[i].isSelected = false;
				}

				shopEntries[i].isAvailable = canPurchaseItem(shopEntries[i].getItemData());
            }

			
		}

		void clampValues()
		{

			int maxItemDat = (itemData.Count - 1);
			int maxEntry = (shopEntries.Count - 1);
			int realChoice = choiceOffset + currentChoice;

			if (currentChoice > maxEntry)
			{
				choiceOffset++;
				currentChoice = maxEntry;
			}

			if (currentChoice < 0)
			{
				choiceOffset--;
				currentChoice = 0;
			}

			if (currentChoice > maxItemDat)
				currentChoice = maxItemDat;

			if ((choiceOffset + maxEntry) > maxItemDat)
				choiceOffset = (maxItemDat - maxEntry);

			if (choiceOffset < 0)
				choiceOffset = 0;

			for (int i = 0; i < shopEntries.Count; i++)
			{
				if(choiceOffset + i < itemData.Count)
					shopEntries[i].setItemData(itemData[choiceOffset + i]);
				else
					shopEntries[i].setItemData(null);
			}
		}

		void handleInput()
		{
			if (!menuActive)
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
				updateItemDescription();
			}

			if (Input.GetKeyDown(KeyCode.S) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Down))
			{
				currentChoice++;
				//selectSound.Play();
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_selectSound);
				updateHighlighted();
				updateItemDescription();
			}

			if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Action"))
			{
				//confirmSelection();
				purchaseItem(shopEntries[currentChoice].getItemData());
			}

			if (Input.GetKeyDown(KeyCode.Delete) || Input.GetButtonDown("Jump"))
			{
				//deleteFile();

			}
		}

		void updateItemDescription()
        {
			clampValues();

			SuikaMartItemData item = shopEntries[currentChoice].getItemData();

			if (item == null)
			{
				descriptionBox.text = "";
				return;
			}

			descriptionBox.text = item.item.description;
		}

		bool canPurchaseItem(SuikaMartItemData item)
        {

			if (item == null)
				return false;

			int rings = PlayerManager.Instance.getTotalRings();

			if(rings >= item.itemPrice)
            {
				return true;
            }

			return false;
		}



		void purchaseItem(SuikaMartItemData item)
        {
			if(!canPurchaseItem(item))
            {
				sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_cancelSound);
				return;
            }

			sfx.PlayOneShot(GameManager.Instance.systemData.AUDIO_confirmSound);
			PlayerManager.Instance.subtractTotalRings(item.itemPrice);
			PlayerManager.Instance.getCurrentPlayerStatus().addItem(item.item.getHash());
			updateHighlighted();
        }

        private void OnValidate()
        {
            foreach(SuikaMartItemData i in itemData)
            {
				i.label = i.item.name;
            }
        }
    }
}
