using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class SaveFileMetadata
	{
		public int fileIndex;
		public int ringBank;
		public string fileName;


	};

	public class FileSelectMenuEntry : MonoBehaviour
	{

		public int fileIndex = 0;
		SaveFileMetadata mData;

		public Text label;
		public Text ringBank;
		public CanvasGroup infoCanvas;

		// Start is called before the first frame update
		void Start()
	    {
			setFileIndex(fileIndex);
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		public void setFileIndex(int slot)
        {

			mData = SaveGame.getFileMetaData(slot);

			if(mData == null)
            {
				label.text = "New Game";
				infoCanvas.alpha = 0.0f;
				return;
            }

			infoCanvas.alpha = 1.0f;
			label.text = "File " + mData.fileIndex.ToString("00");
			PlayerManager.Instance.getTotalRings().ToString("00000");
		}

	}
}
