using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class SleepScreen : MonoBehaviour
	{

		public CoroutineHandle routineHandle;

		// Start is called before the first frame update
		void Start()
	    {
			routineHandle = Timing.RunCoroutine(doSleepScene());   
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		IEnumerator<float> doSleepScene()
        {
			yield return Timing.WaitForSeconds(1.0f);
			processSleeping();
			yield return Timing.WaitForSeconds(3.0f);
			PlayerManager.Instance.wakeupScene(false);

		}
		
		void processSleeping()
        {
			//60 second nap
			PlayerManager.Instance.processSleeping(PlayerManager.Instance.AmyStatus, 60.0f);
			PlayerManager.Instance.processSleeping(PlayerManager.Instance.CreamStatus, 60.0f);


			if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Amy)
				PlayerManager.Instance.currentCharacter = PlayableCharacter.Cream;
			else
				PlayerManager.Instance.currentCharacter = PlayableCharacter.Amy;

			PlayerManager.Instance.decidePlayerNPCLocation();

		}
	}
}
