using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class EventTrigger : MonoBehaviour
	{

		public bool triggerOnce = false;
		bool triggered = false;

		public UnityEvent onActivate;
		public UnityEvent onDeactivate;

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		private void OnTriggerEnter(Collider other)
		{
			if (other.GetComponent<Player>())
			{
				if (triggerOnce && triggered)
					return;

				triggered = true;
				onActivate.Invoke();
			}
		}

        private void OnTriggerExit(Collider other)
        {
			if (other.GetComponent<Player>())
			{
				onDeactivate.Invoke();
			}
		}
    }
}
