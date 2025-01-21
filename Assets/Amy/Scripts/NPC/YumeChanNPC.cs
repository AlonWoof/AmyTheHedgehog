using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class YumeChanNPC : NPC
	{

		public GameObject clothedModel;
		public GameObject nudeModel;

		public Message AmyIntroMessage;
		public Message CreamIntroMessage;

		public List<Message> AmyMessages;
		public List<Message> CreamMessages;

		int timesTalkedTo = 0;
		int lastMessageID = -1;
		public bool randomOrder = true;


		public bool naked = false;
		Animator mAnimator;

        
        private void Awake()
        {

			CharacterTags tg = transform.parent.gameObject.AddComponent<CharacterTags>();
			tg.character = CharaTag.Yume;

			timesTalkedTo = -1;

			//She has a very slim chance of being naked.
			int rng = Random.Range(0, 64);

			//chance increase if you have my nude mod~
			if(PlayerManager.Instance.getStoryFlag(StoryFlag.SADX_NUDE))
				rng = Random.Range(0, 32);

			if(rng == 7)
            {
				naked = true;
			}

			if(naked)
            {
				nudeModel.SetActive(true);
				clothedModel.SetActive(false);

				mAnimator = nudeModel.GetComponent<Animator>();
				addNakedMessages();
			}
			else
            {
				nudeModel.SetActive(false);
				clothedModel.SetActive(true);

				mAnimator = clothedModel.GetComponent<Animator>();
				
			}

		}

		void addNakedMessages()
        {
			Message mes = new Message();

			mes.messages = new List<string>();

			mes.messages.Add("Yeah, I'm naked this time. Just like you. \nFeels nice, huh?");
			mes.messages.Add("Of course, some people hate that.\n But they should just leave us alone.");

			AmyMessages.Add(mes);

			mes = new Message();
			mes.messages = new List<string>();

			mes.messages.Add("You know, in some cultures nudity\n is a sign of innocence and purity.");
			mes.messages.Add("...Present company definitely proves that point.\n In other words, you're adorable. ");

			CreamMessages.Add(mes);
		}


		public Message getNextMessage()
        {

            if (PlayerManager.Instance.currentCharacter == PlayableCharacter.YoungAmy)
            {
                Message youngAmyMessage = new Message();

                youngAmyMessage.messages = new List<string>();

				youngAmyMessage.messages.Add("Wow, how adorable! So that's how you looked\n when you were Cream's age!");
                youngAmyMessage.messages.Add("Anyways... to the person behind the screen...");
                youngAmyMessage.messages.Add("A certain blue-haired girl told me to tell you\nnot to be surprised if the game breaks.");
                youngAmyMessage.messages.Add("And uh, I must say I agree. It's embarassing, \nbut my dreams aren't fully stable, you know...");
				youngAmyMessage.messages.Add("Anyways, mata ne... until we meet again~");

				return youngAmyMessage;

			}

            if (timesTalkedTo == -1)
			{
				if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Amy)
					return AmyIntroMessage;
				else if(PlayerManager.Instance.currentCharacter == PlayableCharacter.Cream)
					return CreamIntroMessage;

			}

			if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Amy)
			{

				if (timesTalkedTo >= AmyMessages.Count)
				{
					timesTalkedTo = 0;

					if (randomOrder)
						AmyMessages.Shuffle();
				}

				return AmyMessages[timesTalkedTo];
			}
			else
			{
				if (timesTalkedTo >= CreamMessages.Count)
				{
					timesTalkedTo = 0;

					if (randomOrder)
						CreamMessages.Shuffle();
				}

				return CreamMessages[timesTalkedTo];
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


			CoroutineHandle msgProc = UIManager.Instance.messageBox.showMessageBox(msg, SpeakerProfile.Yume);

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

			if (PlayerManager.Instance.currentCharacter == PlayableCharacter.YoungAmy)
			{

				mPlayer.interactTimeout = 1.0f;
				transform.parent.gameObject.SetActive(false);
			}
		}

		// Update is called once per frame
		void Update()
	    {
	        
	    }
	}
}
