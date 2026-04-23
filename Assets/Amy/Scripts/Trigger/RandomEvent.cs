using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2026 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class RandomEvent : MonoBehaviour
	{

		public int dayNumberMod = -1;
		public int dayNumberIndex = 0;

		public int universeNumberMod = -1;
		public int universeNumberOfset = 0;

		public UnityEvent onDayNumber;
		public UnityEvent onNotDayNumber;

		public UnityEvent onUniverseNumber;
		public UnityEvent onNotUniverseNumber;

		// Start is called before the first frame update
		public void Start()
		{
			executeEvents();
		}

		// Update is called once per frame
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F6))
				executeEvents();
		}

		public void executeEvents()
		{
			DayEvents today = PlayerManager.Instance.todayEvents;

			if(dayNumberMod > 0)
            {
				if(today.dayNumbers[dayNumberIndex] % dayNumberMod == 0)
					onDayNumber.Invoke();
				else
					onNotDayNumber.Invoke();
			}

			if(universeNumberMod > 0)
            {
				if (PlayerManager.Instance.universeNumber % universeNumberMod == 0)
					onUniverseNumber.Invoke();
				else
					onNotUniverseNumber.Invoke();
            }
		}
	}
}
