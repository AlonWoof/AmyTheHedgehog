using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class SaveFileMetadata
	{
		public int fileIndex;
		public int ringBank;
		public string fileName;
		public System.DateTime lastSaveTime;
		public PlayableCharacter currentCharacter;
		public int totalHours;
		public int totalMinutes;
		public int totalSeconds;
	};

	public class FileSelectMenuEntry : MonoBehaviour
	{

		public int fileIndex = 0;
		public bool isSelected = false;
		SaveFileMetadata mData;

		public Text label;
		public Text ringBank;
		public Text saveTime;
		public Text characterName;
		public CanvasGroup infoCanvas;

		public Color mColor = Color.white;

		// Start is called before the first frame update
		void Start()
	    {

	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }



		public void setFileIndex(int slot)
        {
			fileIndex = slot;
			mData = SaveGame.getFileMetaData(slot);

			label.color = mColor;
			ringBank.color = mColor;
			saveTime.color = mColor;

			if (mData == null)
            {
				label.text = "File " + slot.ToString("00") + "         New Game";
				infoCanvas.alpha = 0.0f;
				return;
            }

			infoCanvas.alpha = 1.0f;
			label.text = "File " + mData.fileIndex.ToString("00");
			ringBank.text = mData.ringBank.ToString("00000");
			saveTime.text = mData.lastSaveTime.ToShortDateString();

			if(mData.currentCharacter == PlayableCharacter.Amy)
            {
				characterName.text = "Amy";
				characterName.color = SystemColors.AmyColor;
            }
			else if(mData.currentCharacter == PlayableCharacter.Cream)
            {
				characterName.text = "Cream";
				characterName.color = SystemColors.CreamColor;
			}
		}

	}
}
