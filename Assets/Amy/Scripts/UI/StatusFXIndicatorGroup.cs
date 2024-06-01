using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class StatusFXIndicatorGroup : MonoBehaviour
	{
		// Start is called before the first frame update

		public List<StatusFXIndicator> statusFXIcon;
		PlayerStatus pStats;

	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			pStats = PlayerManager.Instance.getCurrentPlayerStatus();


			foreach (StatusFXIndicator sef in statusFXIcon)
            {
				if (pStats.checkStatusEffect(sef.mEffect))
					sef.gameObject.SetActive(true);
            }
	    }
	}
}
