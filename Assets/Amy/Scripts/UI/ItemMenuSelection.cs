using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ItemMenuSelection : MonoBehaviour
	{
		// Start is called before the first frame update

		public ItemData mData;
		public Text mText;
		public Image mIcon;
		public bool isAvailable = false;

	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		public void refreshLabel()
        {
			if(mData == null)
            {
				mText.text = "";
				mIcon.enabled = false;
				isAvailable = false;
			}
			else
            {
				mText.text = mData.displayName;

				if (mData.icon)
					mIcon.sprite = mData.icon;

				isAvailable = true;
			}
        }
    }
}
