using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class SleepAction : MonoBehaviour
	{
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }


		public void goToSleep()
        {
			GameManager.Instance.cutsceneMode = true;
			GameManager.Instance.disableInput();

			GameManager.Instance.loadScene("SleepScreen");
			
		}
	}
}
