using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TitleScreen : MonoBehaviour
	{

		public Animator titleScreenAnimator;
		public bool sequenceDone = false;
		public CanvasGroup pressStartCanvas;

	    // Start is called before the first frame update
	    void Start()
	    {
			Timing.RunCoroutine(doTitleSequence());

	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
			if(sequenceDone)
            {
				float targetAlpha = Mathf.Sin(Time.time * 2).Remap(-1.0f, 1.0f, 0.0f, 1.0f);
				pressStartCanvas.alpha = Mathf.Lerp(pressStartCanvas.alpha, targetAlpha, 0.2f);

			}
	    }

		IEnumerator<float> doTitleSequence()
        {
			yield return 0f;

			while(!sequenceDone)
            {
				yield return 0f;

				if (Input.GetButtonDown("Action") || Input.GetButtonDown("Pause"))
				{
					if(!GameManager.Instance.playerInputDisabled)
						titleScreenAnimator.Play("QuickAppear");
				}

			}

			yield return Timing.WaitForSeconds(0.25f);

			while(!Input.GetButtonDown("Action") && !Input.GetButtonDown("Pause"))
            {
				yield return 0f;
            }

			while (GameManager.Instance.playerInputDisabled)
			{
				yield return 0f;
			}

			UIManager.Instance.fadeScreen(false,2.0f, true);
			GameManager.Instance.loadScene("FileSelect", true);
        }
	}
}
