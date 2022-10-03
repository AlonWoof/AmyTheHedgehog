using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;


/* Copyright 2022 Jason Haden */
namespace Amy
{

	public class UIManager : Singleton<UIManager>
	{

        public Canvas mainCanvas;
        public GameObject ingameHUD;

        Fader mFader;

        CanvasGroup mHudGroup;

        public void Init()
        {
            Debug.Log("UI Manager Initialized...");
        }

        // Start is called before the first frame update
        void Start()
    	{
            GameObject inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_userInterface);


            mFader = inst.GetComponentInChildren<Fader>();
            //mIngameHUD = inst.GetComponentInChildren<IngameHUD>();
            mainCanvas = inst.GetComponent<Canvas>();
            mHudGroup = inst.GetComponentInChildren<CanvasGroup>();
        }

    	// Update is called once per frame
    	void Update()
    	{
            if (PlayerManager.Instance.getPlayer(false) == null || GameManager.Instance.cutsceneMode || GameManager.Instance.playerInputDisabled)
            {
                mHudGroup.alpha = 0;
            }
            else
            {
                mHudGroup.alpha = Mathf.Lerp(mHudGroup.alpha, 1.0f, 0.25f);
            }


        }

        public void fadeScreen(bool fadeIn, float time = 1.0f, bool toWhite = false)
        {
            mFader.fadeScreen(fadeIn, time, toWhite);
        }
    }

}
