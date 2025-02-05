using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ShopRingCounter : MonoBehaviour
	{

		public Text ringAmountText;
		public int desiredRingCount = 0;
		public int ringCount = 0;

	    // Start is called before the first frame update
	    void Start()
	    {
			ringCount = PlayerManager.Instance.getTotalRings();

		}
	
	    // Update is called once per frame
	    void Update()
	    {
			desiredRingCount = PlayerManager.Instance.getTotalRings();

			if (ringCount < desiredRingCount)
				ringCount++;
			else if (ringCount > desiredRingCount)
				ringCount--;

			ringAmountText.text = ringCount.ToString();
	    }
	}
}
