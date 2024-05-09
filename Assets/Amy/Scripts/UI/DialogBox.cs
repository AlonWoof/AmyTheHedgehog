using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class DialogBox : MonoBehaviour
	{
		public Animator mAnimator;
		public Text myText;
		public int textProgress = 0;
		public string fullText;
		public float delay = 0.03f;

		const float speed_fast = 0.01f;
		const float speed_mid = 0.02f;
		const float speed_slow = 0.04f;

		// Start is called before the first frame update
		void Start()
	    {
			myText.text = "";
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			//testInput();

		}

		void testInput()
        {
			if(Input.GetKeyDown(KeyCode.F7))
            {
				Message test = new Message();
				test.messages = new List<string>();

				test.messages.Add("This Message is a Test");
				test.messages.Add("This Sausage is the Last");
				test.messages.Add("This Massage is the Best");
				test.messages.Add("The bondage was a Success");

				showMessageBox(test);
			}
        }

		public CoroutineHandle showMessageBox(Message msg)
		{
			return Timing.RunCoroutine(doMessage(msg));
        }

		IEnumerator<float> doMessage(Message msg)
        {
			bool inputState = GameManager.Instance.playerInputDisabled;

			GameManager.Instance.disablePlayerInput();

			myText.text = "";
			mAnimator.Play("Appear");
			yield return Timing.WaitForSeconds(0.5f);

			msg.onStartMessage.Invoke();

			foreach (string str in msg.messages)
            {

				CoroutineHandle print = Timing.RunCoroutine(doPrint(str));

				while (print.IsRunning)
					yield return 0f;


				while(!Input.GetButtonDown("Action"))
					yield return 0f;

				yield return Timing.WaitForSeconds(0.05f);
			}

			yield return Timing.WaitForSeconds(0.1f);

			mAnimator.Play("Disappear");
			msg.onEndMessage.Invoke();

			yield return Timing.WaitForSeconds(0.2f);

			if (!inputState)
				GameManager.Instance.enablePlayerInput();
		}

		IEnumerator<float> doPrint(string str)
        {
			fullText = str;
			textProgress = 0;
			myText.text = "";

			int skipFrames = 0;

			while (textProgress < fullText.Length)
			{

				textProgress = Mathf.Clamp(textProgress, 0, str.Length);

				myText.text = getCurrentText();

				textProgress++;


				if (Input.GetButton("Action"))
					skipFrames++;

				if (skipFrames > 8)
					textProgress = fullText.Length;

				yield return Timing.WaitForSeconds(delay);
			}

			myText.text = fullText;
		}

		string getCurrentText()
        {
			return fullText.Insert(textProgress, "<color=#00000000>") + "</color>";
		}
	}
}
