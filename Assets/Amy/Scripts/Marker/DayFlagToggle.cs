using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class DayFlagToggle : MonoBehaviour
	{
		public GameObject stationCircleNight;

	    // Start is called before the first frame update
	    void Awake()
	    {
			stationCircleNight.SetActive(PlayerManager.Instance.todayEvents.stationCircleNight);

		}
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
