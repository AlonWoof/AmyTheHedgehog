using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class SleepScreen : MonoBehaviour
	{

		public CoroutineHandle routineHandle;
		public Text nextDayText;

		// Start is called before the first frame update
		void Start()
	    {
			routineHandle = Timing.RunCoroutine(doSleepScene());
	    }
	
	    // Update is called once per frame
	    void Update()
	    {

	    }

		void updateTextColor(float colorChangeTime)
        {
			if (PlayerManager.Instance.isBadDay())
			{
				nextDayText.CrossFadeColor(new Color(1.0f, 0.3f, 0.4f), colorChangeTime, true, false);
			}
			else
			{
				nextDayText.CrossFadeColor(new Color(1.0f, 1.0f, 1.0f), colorChangeTime, true, false);
			}
		}

		IEnumerator<float> doSleepScene()
        {
			nextDayText.text = "Day " + PlayerManager.Instance.days;
			updateTextColor(0.01f);



			yield return Timing.WaitForSeconds(4.0f);
			processSleeping();

			PlayerManager.Instance.advanceDay();

			if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Amy)
				PlayerManager.Instance.currentCharacter = PlayableCharacter.Cream;
			else
				PlayerManager.Instance.currentCharacter = PlayableCharacter.Amy;

			nextDayText.text = "Day " + PlayerManager.Instance.days;
			updateTextColor(1.0f);

			if(PlayerManager.Instance.isBadDay())
            {
				int rng = Random.Range(0, 100);

				if(rng > 30)
					PlayerManager.Instance.AmyStatus.setStatusEffect(PlayerStatusFX.Horny);

				rng = Random.Range(0, 100);

				if (rng > 30)
					PlayerManager.Instance.AmyStatus.setStatusEffect(PlayerStatusFX.Sick);

				rng = Random.Range(0, 100);

				if (rng > 30)
					PlayerManager.Instance.AmyStatus.currentStamina *= 0.5f;
			}

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
