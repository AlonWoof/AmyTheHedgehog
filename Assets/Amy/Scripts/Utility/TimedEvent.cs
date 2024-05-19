using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TimedEvent : MonoBehaviour
	{
		public float time = 1.0f;
		public bool waitForSceneFadeIn = false;

		public UnityEvent timedEvent;

	
	    // Update is called once per frame
	    void Update()
	    {
			//TODO: Add an easy hook into screen fade
			if (waitForSceneFadeIn && GameManager.Instance.playerInputDisabled)
				return;

			if (time > 0.0f)
			{
				time -= Time.deltaTime;

				if(time < 0.0f)
                {
					timedEvent.Invoke();
                }
			}
	    }
	}
}
