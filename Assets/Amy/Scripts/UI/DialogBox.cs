using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[System.Serializable]
	public enum SpeakerProfile
    {
		Default,
		Amy,
		Cream,
		Yume,
		Suika,
		Jenny
    }

	[System.Serializable]
	public class Message
	{

		[TextArea(1, 2)]
		public List<string> messages = new List<string> { "NULL" };

		public AudioClip voice;

		[Range(0, 3)]
		public float voice_pitch = 1.0f;

		public UnityEvent onStartMessage = new UnityEvent();
		public UnityEvent onEndMessage = new UnityEvent();

		public UnityEvent onStartPrint = new UnityEvent();
		public UnityEvent onEndPrint = new UnityEvent();
	}


	public class DialogBox : MonoBehaviour
	{
		public Animator mAnimator;
		public Text myText;
		public int textProgress = 0;
		public string fullText;
		public float delay = 0.02f;

		public static float speed_fast = 0.01f;
		public static float speed_mid = 0.02f;
		public static float speed_slow = 0.04f;

		public AudioSource mVoice;

		// Start is called before the first frame update
		void Start()
	    {
			myText.text = "";

			if(!mVoice)
            {
				mVoice = gameObject.AddComponent<AudioSource>();
				mVoice.outputAudioMixerGroup = GameManager.Instance.systemData.AUDIO_Group_Voice;
				mVoice.spatialBlend = 0.0f;
				mVoice.volume = 1.0f;

			}
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			//testInput();

		}

		public void setMessageBoxSpeed(float spd)
        {
			delay = spd;
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

		public void setTextColorTint(Color color)
        {
			myText.color = color;
        }

		public CoroutineHandle showMessageBox(string[] str, SpeakerProfile profile = SpeakerProfile.Default)
		{
			Message m = new Message();
			m.messages.Clear();

			foreach (string s in str)
			{
				m.messages.Add(s);
			}

			return showMessageBox(m, profile);
		}

		public CoroutineHandle showMessageBox(string str, SpeakerProfile profile = SpeakerProfile.Default)
		{
			Message m = new Message();
			m.messages.Clear();
			m.messages.Add(str);

			return showMessageBox(m, profile);
        }
		public CoroutineHandle showMessageBox(Message msg, SpeakerProfile profile = SpeakerProfile.Default)
		{
			setTextColorTint(getProfileColor(profile));

			return Timing.RunCoroutine(doMessage(msg), gameObject);
        }

		Color getProfileColor(SpeakerProfile profile)
        {
			switch(profile)
            {
				case SpeakerProfile.Amy:
					return Color.Lerp(SystemColors.AmyColor, Color.white, 0.25f);
				case SpeakerProfile.Cream:
					return Color.Lerp(SystemColors.CreamColor, Color.white, 0.0f);
				case SpeakerProfile.Yume:
					return Color.Lerp(SystemColors.YumeColor, Color.white, 0.25f);

				case SpeakerProfile.Suika:
					return Color.Lerp(SystemColors.YumeColor, Color.white, 0.45f);

				case SpeakerProfile.Jenny:
					return Color.Lerp(SystemColors.JennyColor, Color.white, 0.25f);
			}

			return Color.white;
        }

		public void cancelMessage()
        {
			Timing.KillCoroutines(gameObject);
			fullText = "";
			mAnimator.Play("Disappear");
		}

		IEnumerator<float> doMessage(Message msg)
        {
			bool inputState = GameManager.Instance.playerInputDisabled;

			GameManager.Instance.disablePlayerInput();

			myText.text = "";
			mAnimator.Play("Appear");
			yield return Timing.WaitForSeconds(0.5f);



			msg.onStartMessage.Invoke();

			if(msg.voice)
            {
				mVoice.PlayOneShot(msg.voice);
            }

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

			if (msg.onEndMessage != null)
				msg.onEndMessage.Invoke();

			yield return Timing.WaitForSeconds(0.2f);

			if (!inputState)
				GameManager.Instance.enablePlayerInput();

			setTextColorTint(Color.white);
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
