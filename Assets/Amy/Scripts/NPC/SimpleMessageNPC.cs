using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class SimpleMessageNPC : NPC
	{

		public Message SimpleMessage;


        public void simpleTalk()
        {
            Timing.RunCoroutine(doSimpleTalk().CancelWith(gameObject));
        }

        private void Update()
        {
            updatelookAt();
        }

        IEnumerator<float> doSimpleTalk()
        {
            Player mPlayer = PlayerManager.Instance.getPlayer();
            Message msg = SimpleMessage;


            CoroutineHandle msgProc = UIManager.Instance.messageBox.showMessageBox(msg, speakerProfile);

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

            if (path)
                path.enableMovement();

            if (wasNormal)
                mPlayer.changeCurrentMode(PlayerModes.NORMAL);

            yield return Timing.WaitForOneFrame;

            onEndTalk.Invoke();
        }
    }
}
