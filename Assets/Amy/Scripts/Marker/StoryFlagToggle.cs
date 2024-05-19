using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class StoryFlagToggle : MonoBehaviour
	{

		public UnityEvent onFlagSet;
		public UnityEvent onFlagUnset;

		public string storyFlag;

	    // Start is called before the first frame update
	    void Start()
	    {
	        if(PlayerManager.Instance.getStoryFlag(storyFlag))
            {
				onFlagSet.Invoke();
            }
			else
            {
				onFlagUnset.Invoke();
            }
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
