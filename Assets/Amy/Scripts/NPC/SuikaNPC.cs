using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MEC;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{



	[System.Serializable]
	public class SuikaMartData
    {
		public MessageBank AmyGreetingsBasic;
		public MessageBank CreamGreetingsBasic;

		public MessageBank AmyBuyItems;
		public MessageBank CreamBuyItems;

		public MessageBank AmyDailyTips;
		public MessageBank CreamDailyTips;
    }

	public class SuikaMartItem
    {
		public string itemName;
		public int itemPrice;

		public GameObject itemModel;

    }

	public class SuikaNPC : NPC
	{
		MessageBank SuikaGreetings;
		MessageBank SuikaGoodbyes;
		MessageBank SuikaBuyItems;
		MessageBank SuikaDailyTip;

		CoroutineHandle currentAction;

		public CinemachineVirtualCamera vCam;
		public SuikaMartMainMenu mainMenu;

		int lastMessage = 0;

        public void Start()
        {
			loadMessageBanks();
        }

        public void loadMessageBanks()
        {
			switch(PlayerManager.Instance.currentCharacter)
            {
				case PlayableCharacter.Amy:
					SuikaGreetings = GameManager.Instance.systemData.suikaMartData.AmyGreetingsBasic;
					SuikaBuyItems = GameManager.Instance.systemData.suikaMartData.AmyBuyItems;
					SuikaDailyTip = GameManager.Instance.systemData.suikaMartData.AmyDailyTips;
					break;
				case PlayableCharacter.Cream:
					SuikaGreetings = GameManager.Instance.systemData.suikaMartData.CreamGreetingsBasic;
					SuikaBuyItems = GameManager.Instance.systemData.suikaMartData.CreamBuyItems;
					SuikaDailyTip = GameManager.Instance.systemData.suikaMartData.CreamDailyTips;
					break;
			}

			SuikaDailyTip.messages.Shuffle();
        }

		public void talkToSuika()
        {
			Timing.RunCoroutine(doTalkToSuika(), gameObject);
        }

		public CoroutineHandle dailyMessage()
        {
			return Timing.RunCoroutine(doDailyMessage(), gameObject);
        }

		public IEnumerator<float> doDailyMessage()
        {

			CoroutineHandle msgCoroutine = new CoroutineHandle();

			if (lastMessage == 2)
            {
				msgCoroutine = UIManager.Instance.messageBox.showMessageBox("I... don't got anything else to say today." +
					"\nSorry, Charlie.", SpeakerProfile.Suika);

			}
			else
            {
				msgCoroutine = UIManager.Instance.messageBox.showMessageBox(SuikaDailyTip.messages[lastMessage++], SpeakerProfile.Suika);

			}


			Player pl = PlayerManager.Instance.mPlayerInstance;

			pl.changeCurrentMode(PlayerModes.LISTENING);

			yield return 0f;

			GameManager.Instance.disablePlayerInput();

			while (msgCoroutine.IsRunning)
			{
				yield return 0f;
				pl.interactTimeout = 3.0f;
			}

			GameManager.Instance.enablePlayerInput();
		}

		public IEnumerator<float> doTalkToSuika()
        {
			//Just a test for now.
			int affectionLevel = 0;

			affectionLevel = Mathf.Clamp(affectionLevel, 0, SuikaGreetings.messages.Length);

			Player pl = PlayerManager.Instance.mPlayerInstance;

			pl.changeCurrentMode(PlayerModes.LISTENING);


			CoroutineHandle msgCoroutine = UIManager.Instance.messageBox.showMessageBox(SuikaGreetings.messages[affectionLevel],SpeakerProfile.Suika);

			while (msgCoroutine.IsRunning)
			{
				yield return 0f;
				pl.interactTimeout = 3.0f;
				GameManager.Instance.playerInputDisabled = true;
			}

			yield return 0f;

			GameManager.Instance.changeCameraBlendMode(GameManager.blend_mode.slow);
			vCam.Priority = 999;

			CoroutineHandle menuCoroutine = mainMenu.enableMenu();

			GameManager.Instance.disablePlayerInput();

			while (menuCoroutine.IsRunning)
			{
				yield return 0f;
				GameManager.Instance.playerInputDisabled = true;
			}

			vCam.Priority = -100;
			GameManager.Instance.enablePlayerInput();
			pl.changeCurrentMode(PlayerModes.NORMAL);
			

			//Code for store interface here

		}
	}
}
