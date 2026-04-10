using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class DayEventTrigger : MonoBehaviour
	{

		public int luckyNumberMod = -1;
		public int luckyNumberOffset = 0;

		public int universeNumberMod = -1;
		public int universeNumberOfset = 0;

		public UnityEvent onLuckyDay;
		public UnityEvent onNotLuckyDay;

		public UnityEvent onUniverseNumber;
		public UnityEvent onNotUniverseNumber;

        public void Start()
        {
			executeEvents();
        }

        private void Update()
        {
			if (Input.GetKeyDown(KeyCode.F6))
				executeEvents();
		}

        public void executeEvents()
        {

			DayEvents today = PlayerManager.Instance.todayEvents;

			if (luckyNumberMod > 0)
			{
				if (today.luckyNumber % (luckyNumberMod + luckyNumberOffset) == 0)
					onLuckyDay.Invoke();
				else
					onNotLuckyDay.Invoke();
			}

			if (universeNumberMod > 0)
			{
				if (PlayerManager.Instance.universeNumber % (universeNumberMod + universeNumberOfset) == 0)
					onUniverseNumber.Invoke();
				else
					onNotUniverseNumber.Invoke();
			}

		}

	}
}
