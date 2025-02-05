using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class SuikaShopEntry : MonoBehaviour
	{

		public int index = 0;
		public bool isSelected = false;
		public bool isAvailable = true;

		public Text itemName;
		public Text itemCost;
		public CanvasGroup alphaGroup;

		bool hasItemData = false;
		SuikaMartItemData data;

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {

			if (isSelected)
			{
				itemName.color = Color.yellow;
				itemCost.color = Color.yellow;
			}
			else if(isAvailable)
            {
				itemName.color = Color.white;
				itemCost.color = Color.white;
			}
			else
            {
				itemName.color = Color.gray;
				itemCost.color = Color.gray;
			}
	    }


		public void setItemData(SuikaMartItemData d)
        {

			if(d == null)
            {
				hasItemData = false;
				alphaGroup.alpha = 0.0f;
				itemName.text = "SORRY NOTHING";
				//itemCost.text = data.itemPrice.ToString();
				return;
			}

			data = d;
			alphaGroup.alpha = 1.0f;
			itemName.text = data.item.displayName;
			itemCost.text = data.itemPrice.ToString();
        }

		public SuikaMartItemData getItemData()
        {
			return data;
        }
	}
}
