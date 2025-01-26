using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AIForceJump : MonoBehaviour
	{

		public float timeout = 0.2f;
		float timeLeft = 0.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			if (timeLeft > 0.0f)
				timeLeft -= Time.deltaTime;
		}

        private void OnTriggerEnter(Collider other)
        {
			if (timeLeft > 0.0f)
				return;

			AIPlayer aiplayer = other.GetComponent<AIPlayer>();

			if (aiplayer)
			{
				//aiplayer.releaseJump();
				aiplayer.pressJump();
				timeLeft = timeout;

				if(aiplayer.currentMode == PlayerModes.SWIMMING)
                {
					aiplayer.changeCurrentMode(PlayerModes.NORMAL);
					aiplayer.Jump(true);
                }
			}
		}
    }
}
