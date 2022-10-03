using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;

/* Copyright 2022 Jason Haden */
namespace Amy
{

	public class Fader : MonoBehaviour
	{

        public Image mImage;

        public void Start()
        {
            fadeScreen(true);
        }

        public void fadeScreen(bool fadeIn, float time = 1.0f, bool toWhite = false)
        {
            if (!mImage)
                return;


            float targetOpacity = fadeIn ? 0.0f : 1.0f;

            if (toWhite && !fadeIn)
                mImage.color = Color.white;
            else if(!fadeIn)
                mImage.color = Color.black;

            mImage.CrossFadeAlpha(targetOpacity, time, true);
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
                fadeScreen(false, 1.0f, false);

            if (Input.GetKeyDown(KeyCode.G))
                fadeScreen(true, 1.0f, false);
        }
	}

}
