using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class YumeChanNPC : NPC
	{

		public GameObject clothedModel;
		public GameObject nudeModel;

		public Message AmyIntroMessage;
		public Message CreamIntroMessage;

		public Message[] AmyMessages;
		public Message[] CreamMessages;

		int timesTalkedTo = 0;
		int lastMessageID = -1;
		public bool randomOrder = true;


		public bool naked = false;
		Animator mAnimator;

        
        private void Awake()
        {

			//She has a very slim chance of being naked.
			int rng = Random.Range(0, 64);

			if(rng == 7)
            {
				naked = true;
			}

			if(naked)
            {
				nudeModel.SetActive(true);
				clothedModel.SetActive(false);

				mAnimator = nudeModel.GetComponent<Animator>();

			}
			else
            {
				nudeModel.SetActive(false);
				clothedModel.SetActive(true);

				mAnimator = clothedModel.GetComponent<Animator>();
			}

		}

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }


		public Message getNextMessage()
		{
			if (timesTalkedTo == 0)
			{
				if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Amy)
					return AmyIntroMessage;
				else
					return CreamIntroMessage;
			}

			if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Amy)
			{
				int rng = Helper.nonRepeatingRandom(0, AmyMessages.Length, lastMessageID);
				lastMessageID = rng;


				if (!randomOrder)
                {
					rng = timesTalkedTo + 1;

					rng = Mathf.Clamp(rng, 1, AmyMessages.Length - 1);
                }

				return AmyMessages[rng];
			}
			else
			{
				int rng = Helper.nonRepeatingRandom(0, CreamMessages.Length, lastMessageID);
				lastMessageID = rng;

				if (!randomOrder)
				{
					rng = timesTalkedTo + 1;

					rng = Mathf.Clamp(rng, 1, CreamMessages.Length - 1);
				}

				return CreamMessages[rng];
			}
		}

		public void yumeTalk()
        {
			Timing.RunCoroutine(doYumeTalk());
        }

		IEnumerator<float> doYumeTalk()
		{
			Player mPlayer = PlayerManager.Instance.getPlayer();
			Message msg = getNextMessage();


			CoroutineHandle msgProc = UIManager.Instance.messageBox.showMessageBox(msg);

			if (path)
				path.disableMovement();

			bool wasNormal = false;

			if (mPlayer.currentMode == PlayerModes.NORMAL)
			{
				mPlayer.changeCurrentMode(PlayerModes.LISTENING);
				wasNormal = true;
			}

			while (msgProc.IsRunning)
			{
				yield return 0f;
			}

			timesTalkedTo++;

			if (path)
				path.enableMovement();

			if (wasNormal)
				mPlayer.changeCurrentMode(PlayerModes.NORMAL);
		}

		// Update is called once per frame
		void Update()
	    {
	        
	    }
	}
}
