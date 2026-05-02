using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2026 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class StationCircleNPC : NPC
	{

		public NPCPath mPath;
        public Animator mAnimator;

        public List<string> idleAnimations;
        public List<string> talkAnimations;

        private void Awake()
        {

        }


        public void LateUpdate()
        {
            
        }

        public void SC_Talk()
        {
            Timing.RunCoroutine(doSCTalk().CancelWith(gameObject));
        }


        public IEnumerator<float> doSCTalk()
        {
            Player mPlayer = PlayerManager.Instance.getPlayer();
            Message msg = AmyMessage;

            if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Yume)
            {
                msg = new Message();
                msg.messages.Clear();
                msg.messages.Add("NPC NAME: " + transform.parent.name +
                    " \n" + "NUMBER OF MESSAGES: " +
                    (AmyMessage.messages.Count + CreamMessage.messages.Count)
                    + "\n POS: " + transform.position);
            }

            if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Cream)
                msg = CreamMessage;

            CoroutineHandle msgProc = UIManager.Instance.messageBox.showMessageBox(msg, speakerProfile);

            if (mPath)
                mPath.disableMovement();

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

            if (path)
                path.enableMovement();


            if (wasNormal)
                mPlayer.changeCurrentMode(PlayerModes.NORMAL);

            yield return Timing.WaitForOneFrame;

            onEndTalk.Invoke();
        }

        public void selectRandomIdle()
        {
            int rand = Random.Range(0, idleAnimations.Count);

            
        }
	}
}
