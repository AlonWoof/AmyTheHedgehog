using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ContextButton : MonoBehaviour
	{

		public Animator mAnimator;
		public Text actionText;

		public float timeLeft = 0.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
			actionText.text = "";

		}
	
	    // Update is called once per frame
	    void Update()
	    {
			if(timeLeft > 0)
            {
				timeLeft -= Time.deltaTime;
			}

			mAnimator.SetFloat("timeLeft", timeLeft);

			if(GameManager.Instance.gamePaused)
            {
				mAnimator.SetFloat("timeLeft", 0);
			}
	    }

		public void setActionText(string str)
        {
			actionText.text = str;
			timeLeft = 0.1f;
        }

		public void clearActionText()
        {
			timeLeft = 0.0f;
        }
	}
}
