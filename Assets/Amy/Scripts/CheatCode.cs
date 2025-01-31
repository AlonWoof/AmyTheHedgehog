using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;
using UnityEngine.SceneManagement;

namespace Amy
{


    [System.Serializable]
    public class CheatCodeInfo
    {
        public char[] code;
        public int progress = 0;

        public float timeLeft = 0.0f;
        public bool isComplete = false;

        public CheatCodeInfo(string ncode)
        {
            code = ncode.ToUpper().ToCharArray();
        }
    }

    public class CheatCode : MonoBehaviour
    {

        const float maxTimeout = 1.0f;

        public CheatCodeInfo debugModeCheat = new CheatCodeInfo("MURAKAMI");
        public CheatCodeInfo waremeCheat = new CheatCodeInfo("CUNNY");


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            checkInput();
        }

        void procCheat(CheatCodeInfo cheat)
        {
            cheat.isComplete = false;

            if (isNextButtonDown(cheat.code[cheat.progress]))
            {
                cheat.progress++;
                cheat.timeLeft = maxTimeout;

                cheat.progress = Mathf.Clamp(cheat.progress, 0, cheat.code.Length);

                if (cheat.progress == cheat.code.Length)
                {
                    cheat.isComplete = true;
                    cheat.progress = 0;
                }
            }

            if (cheat.timeLeft > 0.0f)
            {
                cheat.timeLeft -= Time.deltaTime;

                if (cheat.timeLeft < 0.00001f)
                {
                    cheat.progress = 0;
                }
            }
        }

        void activateDebugCheat()
        {
            //Timing.KillCoroutines();
            GameManager.Instance.debugMode = true;
            SceneManager.LoadScene("MapSelect");
        }

        void activateWaremeCheat()
        {
            //われめは ∞ 
            GameManager.Instance.loadScene("wareme");
        }

        void checkInput()
        {
            procCheat(debugModeCheat);
            procCheat(waremeCheat);

            if (debugModeCheat.isComplete)
                activateDebugCheat();

            if (waremeCheat.isComplete)
                activateWaremeCheat();
        }


        bool checkKeyButtonPair(KeyCode key, string button)
        {
            if (Input.GetKey(key) || Input.GetButton(button))
                return true;

            return false;
        }

        bool isNextButtonDown(char letter)
        {
            KeyCode key = KeyCode.None;
            string button = "";

            switch (letter)
            {
                case 'A':
                    key = KeyCode.A;
                    button = "Action";
                    break;

                case 'B':
                    key = KeyCode.B;
                    button = "Cancel";
                    break;

                case 'C':
                    key = KeyCode.C;
                    button = "";
                    break;

                case 'D':
                    key = KeyCode.D;
                    button = "";
                    break;

                case 'E':
                    key = KeyCode.E;
                    button = "";
                    break;

                case 'I':
                    key = KeyCode.I;
                    button = "";
                    break;

                case 'K':
                    key = KeyCode.K;
                    button = "";
                    break;

                case 'L':
                    key = KeyCode.L;
                    button = "";
                    break;

                case 'M':
                    key = KeyCode.M;
                    button = "";
                    break;

                case 'N':
                    key = KeyCode.N;
                    button = "";
                    break;

                case 'R':
                    key = KeyCode.R;
                    button = "";
                    break;

                case 'T':
                    key = KeyCode.T;
                    button = "";
                    break;

                case 'U':
                    key = KeyCode.U;
                    button = "";
                    break;

                case 'X':
                    key = KeyCode.X;
                    button = "Attack";
                    break;

                case 'Y':
                    key = KeyCode.Y;
                    button = "";
                    break;

                case 'Z':
                    key = KeyCode.Z;
                    button = "ResetView";
                    break;

                default:
                    return false;
            }

            if (button == "")
            {

                if (Input.GetKeyDown(key))
                    return true;
            }
            else
            {

                if (Input.GetKeyDown(key) || Input.GetButtonDown(button))
                    return true;
            }


            return false;
        }


    }
}
