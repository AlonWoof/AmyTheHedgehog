using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CityLightController : MonoBehaviour
	{
	    // Start is called before the first frame update
	    void Awake()
	    {
			if (PlayerManager.Instance.todayEvents.stationCircleNight)
				turnOnAllLights();
			else
				turnOffAllLights();

		}


		public void turnOnAllLights()
        {
			foreach (StreetLight l in FindObjectsOfType<StreetLight>())
			{
				l.turnOn();
			}
		}

		public void turnOffAllLights()
        {
			foreach (StreetLight l in FindObjectsOfType<StreetLight>())
			{
				l.turnOff();
			}
		}

    }
}
