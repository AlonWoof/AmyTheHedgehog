using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class DayFlagToggle : MonoBehaviour
	{
		public UnityEvent yumeShowerEvent;
		public UnityEvent stationCircleNightEvent;

		// Start is called before the first frame update
		void Awake()
	    {
			DayEvents today = PlayerManager.Instance.todayEvents;

			if (today.stationCircleNight)
				stationCircleNightEvent.Invoke();

			if (today.yumeShower)
				yumeShowerEvent.Invoke();
		}
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
