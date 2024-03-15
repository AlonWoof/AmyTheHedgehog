using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class DayNightToggle : MonoBehaviour
	{

		public GameObject dayObject;
		public GameObject nightObject;

		bool isNight = false;

	    // Start is called before the first frame update
	    void Start()
	    {
			
			isNight = !PlayerManager.Instance.isNightTime;

		}
	
	    // Update is called once per frame
	    void Update()
	    {
	        if(!isNight && PlayerManager.Instance.isNightTime)
            {
				if (dayObject)
					dayObject.SetActive(false);

				if(nightObject)
					nightObject.SetActive(true);

				isNight = true;
            }
			else if(isNight && !PlayerManager.Instance.isNightTime)
            {
				if (dayObject)
					dayObject.SetActive(true);

				if (nightObject)
					nightObject.SetActive(false);

				isNight = false;
			}
	    }

	}
}
