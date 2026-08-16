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

		bool canSwitchGirls()
        {

			PlayerStatus otherGirl;

			if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Amy)
				otherGirl = PlayerManager.Instance.CreamStatus;
			else
				otherGirl = PlayerManager.Instance.AmyStatus;

			if (otherGirl.sleepTimeLeft > 0.0f || otherGirl.checkStatusEffect(PlayerStatusFX.Sick))
				return false;

			return true;
		}


		IEnumerator<float> doSleepScene()
        {
			PlayerManager.Instance.setStoryFlag("PROLOGUE_DONE", true);
			nextDayText.text = PlayerManager.Instance.days.ToString();
			updateTextColor(0.01f);

			yield return Timing.WaitForSeconds(4.0f);
			processSleeping();

			PlayerManager.Instance.advanceDay();



			if (PlayerManager.Instance.getCurrentPlayerStatus().sleepTimeLeft > 0.0f && canSwitchGirls())
            {
				if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Amy)
					PlayerManager.Instance.currentCharacter = PlayableCharacter.Cream;

				if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Cream)
					PlayerManager.Instance.currentCharacter = PlayableCharacter.Amy;

			}

			nextDayText.text = PlayerManager.Instance.days.ToString();
			updateTextColor(1.0f);

			if(PlayerManager.Instance.isBadDay())
            {
				PlayerManager.Instance.AmyStatus.setStatusEffect(PlayerStatusFX.Horny);
				PlayerManager.Instance.AmyStatus.setStatusEffect(PlayerStatusFX.Sick);
				PlayerManager.Instance.AmyStatus.currentStamina *= 0.5f;
			}

			PlayerManager.Instance.decidePlayerNPCLocation();

			yield return Timing.WaitForSeconds(2.0f);

			PlayerManager.Instance.wakeupScene(false);

		}
		
		void processSleeping()
        {
			//15 minutes
			PlayerManager.Instance.processSleeping(PlayerManager.Instance.AmyStatus, Helper.minutesToSeconds(15.5f));
			PlayerManager.Instance.processSleeping(PlayerManager.Instance.CreamStatus, Helper.minutesToSeconds(15.5f));

		}
	}
}
