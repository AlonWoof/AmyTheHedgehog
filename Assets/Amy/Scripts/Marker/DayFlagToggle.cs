using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class DayFlagToggle : MonoBehaviour
	{
		public UnityEvent yumeShowerEvent;
		public UnityEvent stationCircleNightEvent;
		public UnityEvent luckyNumberEvent;
		public UnityEvent universeNumberEvent;

		public int luckyNumberChance = -1;
		public int universeNumberChance = -1;

		// Start is called before the first frame update
		void Awake()
	    {
			DayEvents today = PlayerManager.Instance.todayEvents;

			if (today.stationCircleNight)
				stationCircleNightEvent.Invoke();

			if (today.yumeShower)
				yumeShowerEvent.Invoke();

			if(luckyNumberChance > 0)
            {
				if (today.luckyNumber % luckyNumberChance == 0)
					luckyNumberEvent.Invoke();
            }

			if(universeNumberChance > 0)
            {
				if (PlayerManager.Instance.universeNumber % universeNumberChance == 0)
					universeNumberEvent.Invoke();
			}
		}
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
