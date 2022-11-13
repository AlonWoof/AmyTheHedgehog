using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;
using UnityEngine.Events;

/* Copyright 2022 Jason Haden */
namespace Amy
{

    [System.Serializable]
    public enum MessageBoxLocation
    {
        Bottom,
        Middle,
        Top
    }

    public enum MessageBoxStyle
    {
        Default,
        Invisible,
        Sign
    }

    [System.Serializable]
    public class Choice
    {
        public string choiceLabel;
        public Message msgPath;

        public UnityEvent onChoiceMade;
    }

    [System.Serializable]
    public class Message
    {
        //TODO: Make a Talker class
        public Talker mTalker;

        public Color nameColor = Color.blue;
        public string nameTag = "";

        public float delay = 0.0f;


        public bool hasChoice = false;

        public bool isSilent = false;
        public bool invisibleBox = false;

        MessageBoxStyle style = MessageBoxStyle.Default;
        MessageBoxLocation location = MessageBoxLocation.Bottom;

        [TextArea(5, 5)]
        public List<string> messages;
        public List<Choice> choices;

        public AudioClip voice;

        [Range(0, 3)]
        public float voice_pitch = 1.0f;

        public UnityEvent onStartMessage;
        public UnityEvent onEndMessage;

        public UnityEvent onStartPrint;
        public UnityEvent onEndPrint;
    }


    [System.Serializable]
    public class MessageBoxSounds
    {
        public AudioClip AUDIO_TextBox_Open;
        public AudioClip AUDIO_TextBox_Next;
        public AudioClip AUDIO_TextBox_Stop;
        public AudioClip AUDIO_TextBox_Choice;
        public AudioClip AUDIO_TextBox_Close;
    }

    public class MessageBox : MonoBehaviour
	{

        public Text myText;
        public Text nameTag;
        public Image boxBackground;

        public GameObject doneIndicator;
        public GameObject nextArrow;

        public GameObject root;

        public Animator animator;

        public GameObject callbackTarget;
        public Talker talker;


        public Message testData;
        public bool isPrinting = false;

        [TextArea]
        public string fullMessage;
        public string nextMessagePart;

        bool unclosedColorTag = false;

        float charsPerSecond = 25.0f;
        public float speedMult = 1.0f;

        public int textProgress = 0;

        public float currentTime = 0.0f;

        public float waitTime = 0.0f;

        public Vector3 desiredScale = new Vector3(0.5f, 0.0f, 0.0f);

        public GameObject ChoiceA;
        public GameObject ChoiceB;
        public GameObject ChoiceC;
        public GameObject ChoiceCursor;

        public AudioSource voiceSource;
        public AudioSource sfxSource;

        public bool messageInProgress = false;

        public Vector3 worldPosition;

        // Use this for initialization
        void Start()
        {
            // transform.localScale = desiredScale;

            ChoiceA.SetActive(false);
            ChoiceB.SetActive(false);
            ChoiceC.SetActive(false);
            ChoiceCursor.SetActive(false);

            animator = GetComponent<Animator>();

            nameTag.text = "";

            if (!voiceSource)
                voiceSource = gameObject.AddComponent<AudioSource>();

            voiceSource.spatialBlend = 0.0f;
            voiceSource.volume = 0.45f;
            voiceSource.pitch = 0.9f;
            // soundSource.clip = Database.getDatabase().UI_MessageSound;
            voiceSource.playOnAwake = false;

            myText.text = "";
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 npos = GameManager.Instance.mainCamera.WorldToScreenPoint(worldPosition);
            npos.x = Mathf.Clamp(npos.x, 378.0f, 1550.0f);
            npos.y = Mathf.Clamp(npos.y, 175.0f, 915.0f);

           // Debug.Log("X: " + npos.x + " Y: " + npos.y);

            transform.position = Vector3.Lerp(transform.position, npos, 0.5f);

            if (Input.GetKeyDown("t"))
                Timing.RunCoroutine(doShowMessage(testData));

            if (Input.GetKeyDown("u"))
                Timing.RunCoroutine(handleChoice(testData));


        }

        //c0 = white
        //c1 = red
        //c2 = blue
        //c3 = green
        //c4 = yellow
        //s0 = default speed
        //s1 = slowest
        //s2 = slow
        //s3 = fast
        //s4 = veryfast

        //w0 - wait 0.25 seconds
        //w1 - wait 0.5 seconds
        //w2 - wait 1.0 seconds


        string replaceTagsWithChars(string input)
        {
            input = input.Replace("/c0", "░");
            input = input.Replace("/c1", "▒");
            input = input.Replace("/c2", "▓");
            input = input.Replace("/c3", "▐");
            input = input.Replace("/c4", "▀");

            input = input.Replace("/s0", "╖");
            input = input.Replace("/s1", "╘");
            input = input.Replace("/s2", "╛");
            input = input.Replace("/s3", "◘");
            input = input.Replace("/s4", "◙");

            input = input.Replace("/w0", "▬");
            input = input.Replace("/w1", "↕");
            input = input.Replace("/w2", "↔");

            input = input.Replace("/n", "♪");

            input = input.Replace("<3","♥");

            Debug.Log(input);

            return input;
        }

        string preCleanString(string input)
        {

            if (input.Contains("♪"))
            {
                input = input.Replace("♪", "\n");
            }

            if (input.Contains("▒"))
            {
                input = input.Replace("▒", "<color=red>");
                unclosedColorTag = true;
            }

            if (input.Contains("▓"))
            {
                input = input.Replace("▓", "<color=#4784ff>");
                unclosedColorTag = true;
            }

            if (input.Contains("▐"))
            {
                input = input.Replace("▐", "<color=#29c007>");
                unclosedColorTag = true;
            }

            if (input.Contains("▀"))
            {
                input = input.Replace("▀", "<color=yellow>");
                unclosedColorTag = true;
            }

            if (input.Contains("░"))
            {
                input = input.Replace("░", "</color>");
                unclosedColorTag = false;
            }

            if (input.Contains("╖"))
            {
                speedMult = 1.0f;
                input = input.Replace("╖", "");
            }

            if (input.Contains("╘"))
            {
                speedMult = 0.25f;
                input = input.Replace("╘", "");
            }

            if (input.Contains("╛"))
            {
                speedMult = 0.5f;
                input = input.Replace("╛", "");
            }

            if (input.Contains("◘"))
            {
                speedMult = 1.25f;
                input = input.Replace("◘", "");
            }

            if (input.Contains("◙"))
            {
                speedMult = 1.5f;
                input = input.Replace("◙", "");
            }

            if (input.Contains("▬"))
            {
                waitTime = 0.25f;
                input = input.Replace("▬", "");
            }

            if (input.Contains("↕"))
            {
                waitTime = 0.5f;
                input = input.Replace("↕", "");
            }

            if (input.Contains("↔"))
            {
                waitTime = 1.0f;
                input = input.Replace("↔", "");
            }



            return input;
        }


        public void TestMessage()
        {
            Timing.RunCoroutine(doPrintText());
        }

        IEnumerator<float> handleChoice(Message msg)
        {

            yield return 0f;

            int numChoices = msg.choices.Count;

            Debug.Log("NumChoices: " + numChoices);
            int currentChoice = 0;
            bool choiceMade = false;

            Vector3 currentPositon = ChoiceA.transform.position;

            bool m_isAxisInUse = false;

            ChoiceCursor.SetActive(true);
            ChoiceA.SetActive(true);
            ChoiceB.SetActive(true);
            ChoiceC.SetActive(true);

            if (numChoices == 2)
            {
                Vector3 pos = ChoiceA.transform.localPosition;
                pos.x = -200;
                ChoiceA.transform.localPosition = pos;

                pos = ChoiceB.transform.localPosition;
                pos.x = 200;
                ChoiceB.transform.localPosition = pos;

                ChoiceA.GetComponentInChildren<Text>().text = msg.choices[0].choiceLabel;
                ChoiceB.GetComponentInChildren<Text>().text = msg.choices[1].choiceLabel;

                ChoiceC.SetActive(false);
            }

            if (numChoices == 3)
            {
                Vector3 pos = ChoiceA.transform.localPosition;
                pos.x = -400;
                ChoiceA.transform.localPosition = pos;

                pos = ChoiceB.transform.localPosition;
                pos.x = 0;
                ChoiceB.transform.localPosition = pos;

                pos = ChoiceC.transform.localPosition;
                pos.x = 400;
                ChoiceC.transform.localPosition = pos;


                ChoiceA.GetComponentInChildren<Text>().text = msg.choices[0].choiceLabel;
                ChoiceB.GetComponentInChildren<Text>().text = msg.choices[1].choiceLabel;
                ChoiceC.GetComponentInChildren<Text>().text = msg.choices[2].choiceLabel;

            }


            while (!choiceMade)
            {

                if (currentChoice == 0)
                {
                    currentPositon = ChoiceA.transform.position;
                }

                if (currentChoice == 1)
                {
                    currentPositon = ChoiceB.transform.position;
                }

                if (currentChoice == 2)
                {
                    currentPositon = ChoiceC.transform.position;
                }

                if (currentChoice > numChoices - 1)
                    currentChoice = 0;

                if (currentChoice < 0)
                    currentChoice = numChoices - 1;


                if (Input.GetAxisRaw("Horizontal") != 0)
                {
                    if (m_isAxisInUse == false)
                    {
                        m_isAxisInUse = true;
                        currentChoice += Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
                        //   soundSource.PlayOneShot(GameManager.getDatabase().AudioRes.menu_Click);
                    }
                }

                if (Input.GetAxisRaw("Horizontal") == 0)
                {
                    m_isAxisInUse = false;
                }

                ChoiceCursor.transform.position = Vector3.Lerp(ChoiceCursor.transform.position, currentPositon, Time.deltaTime * 32.0f);

                yield return 0f;

                if (Input.GetButtonDown("Fire1"))
                    choiceMade = true;
            }

            Timing.WaitForSeconds(0.125f);

            if (msg.choices[currentChoice].msgPath != null)
            {
                Message d = msg.choices[currentChoice].msgPath;

                Timing.RunCoroutine(doShowMessage(d));

                if (msg.choices[currentChoice].onChoiceMade != null)
                {
                    msg.choices[currentChoice].onChoiceMade.Invoke();
                }
            }

            ChoiceA.SetActive(false);
            ChoiceB.SetActive(false);
            ChoiceC.SetActive(false);
            ChoiceCursor.SetActive(false);

        }

        public IEnumerator<float> doPrintText(Message data = null, bool lastOne = false)
        {
            speedMult = 1.0f;
            myText.text = "";


            fullMessage = replaceTagsWithChars(fullMessage);

            int total = fullMessage.Length;
            Debug.Log("STARTING MESSAGE - Total : " + total);

            string toPrint = "";
            textProgress = 0;
            currentTime = 0.0f;

            bool freeToSkip = false;

            if (talker != null)
                talker.startTalking();

            if(data != null)
                data.onStartPrint.Invoke();

            isPrinting = true;
            while (textProgress < total)
            {

                //Let go of the dang button
                if (!Input.GetButton("Action"))
                    freeToSkip = true;

                string tmp = getNextPart();

                toPrint += preCleanString(tmp);

                if (!unclosedColorTag)
                    myText.text = toPrint;
                else
                    myText.text = toPrint + "</color>";

                float skipSpeed = 1.0f;

                if (Input.GetButton("Action") && freeToSkip)
                    skipSpeed = 6.0f;

                currentTime += (Time.deltaTime * speedMult) * skipSpeed;

                if (waitTime > 0)
                {
                    yield return Timing.WaitForSeconds(waitTime);
                    waitTime = 0.0f;
                }

                yield return 0f;

            }




            if (talker != null)
                talker.stopTalking();

            if (data != null)
                data.onEndPrint.Invoke();

            myText.text = preCleanString(fullMessage);
            isPrinting = false;

            if (lastOne)
            {
                doneIndicator.SetActive(true);
                //sfxSource.PlayOneShot(GameManager.Instance.systemData.messageBoxSounds.AUDIO_TextBox_Stop);
            }
            else
            {
                nextArrow.SetActive(true);
            }

            if (unclosedColorTag)
                myText.text += "</color>";

            while (Input.GetButton("Action"))
            {
                //Again, let go of the FUCKING button
                yield return 0f;
            }

            

            while (!Input.GetButton("Action"))
            {
               // UIManager.Instance.setContextActionText("Next");
                yield return 0f;
            }

            doneIndicator.SetActive(false);
            nextArrow.SetActive(false);

            //if (!lastOne)
                //sfxSource.PlayOneShot(GameManager.Instance.systemData.messageBoxSounds.AUDIO_TextBox_Next);

            fullMessage = "";
        }

        public string getNextPart()
        {
            //textProgress = 

            int newProg = Mathf.RoundToInt(currentTime * charsPerSecond);
            //s newProg = Mathf.Clamp(newProg, 0, fullMessage.Length - textProgress);

            string ret = "";

            if (newProg > textProgress)
            {
                if (!voiceSource.isPlaying)
                    voiceSource.Play();

                ret = fullMessage.Substring(textProgress, newProg - textProgress);
                textProgress = newProg;
                //Debug.Log("Prog: " + textProgress + " New: " + newProg + " Ret: " + ret);
            }

            return ret;
        }


        IEnumerator<float> doShowMessage(Message data)
        {
            bool inputState = GameManager.Instance.playerInputDisabled;

            GameManager.Instance.playerInputDisabled = true;

            messageInProgress = true;

            doneIndicator.SetActive(false);
            nextArrow.SetActive(false);

            sfxSource.volume = 0.25f;
            voiceSource.volume = 0.45f;

            if (data.isSilent)
                sfxSource.volume = 0.0f;

            //sfxSource.PlayOneShot(GameManager.Instance.systemData.messageBoxSounds.AUDIO_TextBox_Open);
            //desiredScale = Vector3.one;
            animator.Play("Appear");
            yield return Timing.WaitForSeconds(0.5f);
            yield return 0f;


            nameTag.text = "";

            if (data.voice != null)
                voiceSource.clip = data.voice;

            nameTag.text = data.nameTag;
            nameTag.color = data.nameColor;

            voiceSource.pitch = data.voice_pitch;

            if (data.onStartMessage != null)
                data.onStartMessage.Invoke();

            if (data != null)
                yield return Timing.WaitForSeconds(data.delay);

            int i = 0;

            foreach (string m in data.messages)
            {
                bool last = false;
                

                if (i == (data.messages.Count - 1))
                {
                    last = true;
                }

                fullMessage = m;
                CoroutineHandle action = Timing.RunCoroutine(doPrintText(data,last));

                while (action.IsRunning)
                {

                    yield return 0f;
                }

                i++;
            }


            if (data.hasChoice)
            {

                CoroutineHandle choice = Timing.RunCoroutine(handleChoice(data));


                while (choice.IsRunning)
                {
                    yield return 0f;
                }

            }
            else
            {

                if (talker != null)
                {
                    if (talker.useVirtualCamera)
                    {
                        GameManager.Instance.mainCamera.GetComponent<Cinemachine.CinemachineBrain>().m_DefaultBlend.m_Style = Cinemachine.CinemachineBlendDefinition.Style.Cut;
                        talker.vCam.m_Priority = -1;
                    }
                }

                Player player = FindObjectOfType<Player>();

                if (player != null && talker != null)
                {
                    player.changeCurrentMode(player.lastMode);
                }

                nameTag.text = "";
                myText.text = "";
                //desiredScale = new Vector3(0.5f, 0.0f, 0.0f);
                //sfxSource.PlayOneShot(GameManager.Instance.systemData.messageBoxSounds.AUDIO_TextBox_Close);
                animator.Play("Disappear");
                Timing.WaitForSeconds(0.25f);



                GameManager.Instance.playerInputDisabled = inputState;

                if (data.onEndMessage != null)
                    data.onEndMessage.Invoke();

                messageInProgress = false;

                // GameManager.Instance.inMessageBox = false;

            }
        }


        public void LateUpdate()
        {
            //  transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 16.0f);
        }

        public CoroutineHandle ShowMessageBox(Message mg, Vector3 pos, Talker tk = null)
        {
            talker = tk;
            callbackTarget = tk.gameObject;

            worldPosition = pos;

            if (talker != null)
            {
                if (talker.useVirtualCamera)
                    talker.vCam.m_Priority = 30;


                Player player = FindObjectOfType<Player>();

                if (player != null)
                {
                    //player.lookAt(talker.transform.position + Vector3.up);


                    if (player.currentMode != PlayerModes.CUTSCENE && !GameManager.Instance.playerInputDisabled && player.currentMode != PlayerModes.LISTENING)
                    {
                        player.changeCurrentMode(PlayerModes.LISTENING);
                    }
                    
                    
                }
            }

            return Timing.RunCoroutine(doShowMessage(mg));
        }



    }

}
