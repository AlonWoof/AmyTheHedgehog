using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;


//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class InsideVanillaScreen : MonoBehaviour
	{
		public CoroutineHandle routineHandle;

		// Start is called before the first frame update
		void Start()
	    {
			Timing.RunCoroutine(insideVanillaScene());
	    }

		IEnumerator<float> insideVanillaScene()
		{

			yield return Timing.WaitForSeconds(4.0f);
			processSleeping();

			PlayerManager.Instance.advanceDay();
			PlayerManager.Instance.decidePlayerNPCLocation();

			yield return Timing.WaitForSeconds(2.0f);

			PlayerManager.Instance.wakeupScene(false);
		}

		void processSleeping()
		{
			//60 second nap
			PlayerManager.Instance.processSleeping(PlayerManager.Instance.AmyStatus, 60.0f);
			PlayerManager.Instance.processSleeping(PlayerManager.Instance.CreamStatus, 60.0f);

		}
	}
}
