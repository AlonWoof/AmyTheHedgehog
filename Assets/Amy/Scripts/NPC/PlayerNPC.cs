using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerNPC : NPC
	{
		public MessageBank[] messageBanks;

        List<Message> messageList;
        int currentMessage = 0;

        private void Awake()
        {
            loadMessages();
        }

        public void loadMessages()
        {

            List<Message> tempList = new List<Message>();
            messageList = new List<Message>();


            foreach (MessageBank bank in messageBanks)
            {

                tempList.Clear();

                foreach (Message m in bank.messages)
                {
                    tempList.Add(m);
                }

                tempList.Shuffle();

                int msgCount = Mathf.RoundToInt((float)tempList.Count * 0.25f);
                msgCount = Mathf.Clamp(msgCount, 1, tempList.Count);

                for(int i = 0; i < msgCount; i++)
                {
                    messageList.Add(tempList[i]);
                }
            }


            
            /*
            foreach(MessageBank bank in messageBanks)
            {
                foreach(Message m in bank.messages)
                {
                    messageList.Add(m);
                }
            }
            */

            messageList.Shuffle();
        }

        public void talk()
        {
            Timing.RunCoroutine(doTalk().CancelWith(gameObject));
        }

        IEnumerator<float> doTalk()
        {
            Player mPlayer = PlayerManager.Instance.getPlayer();

            CoroutineHandle msgProc = UIManager.Instance.messageBox.showMessageBox(messageList[currentMessage], speakerProfile);

            if (path)
                path.disableMovement();

            bool wasNormal = false;

            if (mPlayer.currentMode == PlayerModes.NORMAL)
            {
                mPlayer.changeCurrentMode(PlayerModes.LISTENING);
                wasNormal = true;
            }

            onStartTalk.Invoke();

            while (msgProc.IsRunning)
            {
                yield return 0f;
            }

            onEndTalk.Invoke();


            if (path)
                path.enableMovement();

            if (wasNormal)
                mPlayer.changeCurrentMode(PlayerModes.NORMAL);

            currentMessage++;

            if (currentMessage > messageList.Count-1)
                currentMessage = 0;
        }

        void Update()
        {
            updatelookAt();
        }

    }
}
