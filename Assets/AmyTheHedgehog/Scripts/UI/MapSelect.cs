using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Copyright 2021 Jason Haden */
namespace Amy
{
    [System.Serializable]
    public class MapSelectEntry
    {
        public string label = "SPOT 0";
        public string sceneName = "default";
        public int exitNumber = 0;
        public bool isForbidden = false;
    }

    public enum TimeOfDaySelect
    {
        Noon,
        Sunset,
        Midnight,
        Sunrise,
        MAX
    }


    public class MapSelect : MonoBehaviour
    {

        public List<MapSelectEntry> mapList;

        public List<Text> entryTexts;
        int maxListSize;

        int cursorPosition = 0;
        MapSelectEntry currentItem;

        public Text exitNumberText;
        public Text timeOfDayText;

        PlayableCharacter characterSelect;
        int selectedExit;

        public List<string> loadingMessages;
        public List<string> rareMessages;
        public List<string> creepyMessages;

        public Text loadingText;
        public GameObject loadingScreen;

        public AudioSource selectSound;

        public Color color_amy;
        public Color color_cream;

        // Start is called before the first frame update
        void Start()
        {
            /*
            if(PlayerManager.Instance.saveGame.getCutsceneFlag("REMEMBER 2010"))
            {
                List<MapSelectEntry> toRemove = new List<MapSelectEntry>();

                foreach(MapSelectEntry m in mapList)
                {
                    if(m.label == "")
                    {
                        toRemove.Add(m);
                    }
                }

                foreach(MapSelectEntry m in toRemove)
                {
                    mapList.Remove(m);
                }
            }*/

            maxListSize = entryTexts.Count - 1;
            loadingScreen.SetActive(false);

            int i = 0;

            for (i = 0; i < mapList.Count; i++)
            {
                if (mapList[i].isForbidden)
                {
                    //75% chance of just not being there?

                   // if (Random.Range(0, 100) > 25)
                    //    mapList.RemoveAt(i);
                }
            }

            i = 0;

            foreach (Text t in entryTexts)
            {
                if (i < mapList.Count)
                {
                    if(mapList[i].isForbidden)
                        t.text = (i + 1) + ": " + getCorruptedName();
                    else
                        t.text = (i + 1) + ": " + mapList[i].label;
                }
                else
                {
                    t.text = "";
                }

                i++;
            }

            characterSelect = PlayerManager.Instance.currentCharacter;

            updateSelection();
        }

        // Update is called once per frame
        void Update()
        {
            //updateSelection();
            handleInput();
        }

        string getCorruptedName()
        {
            char[] newSTR = new char[32];

            for (int i = 0; i < 32; i++)
            {
                if (Random.Range(0, 100) > 75)
                    newSTR[i] = (char)Random.Range(0, 255);
                else
                    newSTR[i] = ' ';
            }


            return new string(newSTR);

        }

        void updateSelection()
        {

            cursorPosition = Mathf.Clamp(cursorPosition, 0, maxListSize);

            if(characterSelect >= PlayableCharacter.MAX)
            {
                characterSelect = 0;
            }

            selectedExit = Mathf.Clamp(selectedExit, 0, 16);

            setCharacterText();
            setExitNumberText();

            Color textColor = color_amy;

            if (characterSelect == PlayableCharacter.Cream)
                textColor = color_cream;

            if (entryTexts[cursorPosition] != null && mapList[cursorPosition] != null)
            {
                foreach (Text t in entryTexts)
                {
                    t.color = textColor;
                }

                entryTexts[cursorPosition].color = Color.white;
                currentItem = mapList[cursorPosition];
            }
        }

        void handleInput()
        {
            if(Input.GetKeyDown(KeyCode.W) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Up))
            {
                cursorPosition--;
                selectSound.Play();
                updateSelection();
            }


            if (Input.GetKeyDown(KeyCode.S) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Down))
            {
                cursorPosition++;
                selectSound.Play();
                updateSelection();
            }

            if(Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Jump"))
            {
                characterSelect++;
                selectSound.Play();
                updateSelection();
            }

            if(Input.GetKeyDown(KeyCode.D) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Right))
            {
                selectedExit++;
                selectSound.Play();
                updateSelection();
            }

            if(Input.GetKeyDown(KeyCode.A) || GameManager.Instance.isAnalogDown(AnalogStickDirection.leftStick_Left))
            {
                selectedExit--;
                selectSound.Play();
                updateSelection();
            }

            if(Input.GetButtonDown("Action"))
            {
                showLoadingScreen();
                setPlayerCharacter();
                PlayerManager.Instance.lastExit = selectedExit;
                GameManager.Instance.loadScene(currentItem.sceneName);

                enabled = false;
            }
        }

        void setCharacterText()
        {
            string txt = "Character: ";

            switch (characterSelect)
            {
                case PlayableCharacter.Amy:
                    txt += "<color=#f6a3bb>Amy</color>";
                    break;

                case PlayableCharacter.Cream:
                    txt += "<color=#f8e0b8>Cream</color>";
                    break;
            }

            timeOfDayText.text = txt;
        }

        void setExitNumberText()
        {
            exitNumberText.text = "Exit Number: " + selectedExit;
        }

        void showLoadingScreen()
        {

            if(currentItem.isForbidden)
            {

                int msg_num = Random.Range(0, creepyMessages.Count);

                loadingText.text = creepyMessages[msg_num];
            }
            else
            {
                int rareChance = Random.Range(0, 100);

                if (rareChance < 10)
                {
                    int msg_num = Random.Range(0, rareMessages.Count);
                    loadingText.text = rareMessages[msg_num];
                }
                else
                {
                    int msg_num = Random.Range(0, loadingMessages.Count);
                    loadingText.text = loadingMessages[msg_num];
                }
            }



            loadingScreen.SetActive(true);
        }

        void setPlayerCharacter()
        {
            PlayerManager.Instance.currentCharacter = characterSelect;

        }
	}

}
