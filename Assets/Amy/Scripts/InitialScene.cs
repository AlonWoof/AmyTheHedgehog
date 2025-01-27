using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

    public class InitialScene : MonoBehaviour
    {
        float timeLeft = 0.0f;
        public float timeBeforeContinue = 5.0f;

        bool canContinue = false;

        public GameObject continueText;

        // Start is called before the first frame update
        void Start()
        {
            timeLeft = timeBeforeContinue;
        }

        // Update is called once per frame
        void Update()
        {
            if (timeLeft > 0.0f)
            {
                timeLeft -= Time.unscaledDeltaTime;


            }
            else if (!canContinue)
            {
                canContinue = true;
                continueText.SetActive(true);
            }


            if (canContinue && Input.GetButtonDown("Action") || canContinue && Input.GetButtonDown("Pause"))
            {
                LoadTitle();

                canContinue = false;
                timeLeft = 99.0f;
            }
        }

        void LoadTitle()
        {
            GameManager.Instance.loadTitleScreen();

        }
    }
}
