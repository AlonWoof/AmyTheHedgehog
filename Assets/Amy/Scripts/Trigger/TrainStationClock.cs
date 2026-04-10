using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TrainStationClock : MonoBehaviour
	{
		int hours = 0;
		int minutes = 0;

		public Transform minuteHand;
		public Transform hourHand;

	    // Start is called before the first frame update
	    void Start()
	    {
			

		}
	
	    // Update is called once per frame
	    void Update()
	    {
			updateHoursMinutes();

			//hourHand.transform.rotation.

			float baseRot = 180.0f;
			float inc = 360.0f / 12.0f;

			Vector3 euler = hourHand.transform.rotation.eulerAngles;
			euler.z = baseRot + (inc * hours);

			hourHand.transform.rotation = Quaternion.Euler(euler);
			hourHand.transform.rotation = Quaternion.Lerp(hourHand.transform.rotation, Quaternion.Euler(euler), 0.25f);



			inc = 360.0f / 60.0f;

			euler = minuteHand.transform.rotation.eulerAngles;
			euler.z = baseRot + (inc * minutes);

			minuteHand.transform.rotation = Quaternion.Lerp(minuteHand.transform.rotation, Quaternion.Euler(euler), 0.25f);
		}

		void updateHoursMinutes()
        {
			hours = System.DateTime.Now.Hour;
			minutes = System.DateTime.Now.Minute;

			if (hours > 12)
				hours = hours - 12;

			if (hours == 0)
				hours = 12;
		}
	}
}
