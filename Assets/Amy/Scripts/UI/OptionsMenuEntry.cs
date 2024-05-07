using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[System.Serializable]
	public enum OptionEntryType
	{
		InvertPitch,
		InvertYaw
	}

	public class OptionsMenuEntry : MonoBehaviour
	{

		public OptionEntryType type;
		public Text text;
		public Image checkboxImage;

		public Sprite checkedSprite;
		public Sprite uncheckedSprite;

		public bool isAvailable = true;
		public bool isChecked = false;

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        if(isChecked)
            {
				checkboxImage.sprite = checkedSprite;
            }
			else
            {
				checkboxImage.sprite = uncheckedSprite;
            }

			checkboxImage.color = text.color;
	    }
	}
}
