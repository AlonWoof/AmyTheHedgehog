using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PressureSwitch : MonoBehaviour
	{

		public Animator mAnimator;

		public bool isOn = false;
		public bool stayOn = true;



		public float timer = 0.5f;

		public UnityEvent onActivate;
		public UnityEvent onDeactivate;


		public string storyFlag;
		public bool useStoryFlag = true;

		Player mPlayer;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
			if(useStoryFlag)
            {
				isOn = PlayerManager.Instance.getStoryFlag(storyFlag);
            }
	    }

		void Update()
		{
			mAnimator.SetBool("isOn", isOn);
		}

        private void OnTriggerEnter(Collider other)
        {
			
        }

        private void OnTriggerStay(Collider other)
        {
            if(other.GetComponent<Player>())
            {
				mPlayer = other.GetComponent<Player>();

				if(mPlayer.isOnGround && mPlayer.framesGrounded > 5)
                {
					if (!isOn)
					{
						isOn = true;
						onActivate.Invoke();
						GameManager.Instance.controllerRumble(0.2f, 0.1f, 0.1f);

						mPlayer.acceleration *= 0.5f;

						if (useStoryFlag)
							PlayerManager.Instance.setStoryFlag(storyFlag, true);
					}

				}

			}

        }
    }
}
