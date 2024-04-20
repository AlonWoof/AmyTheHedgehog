using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CityElevator : MonoBehaviour
	{
		public Animator mAnimator;
		public float activateDistance = 3.0f;

		public bool isOpen = false;
		bool hasPlayer = false;
		Player mPlayer;

		public string sceneToLoad;
		public int sceneExit = 0;

	    // Start is called before the first frame update
	    void Start()
	    {
			mAnimator = GetComponent<Animator>();
		}
	
	    // Update is called once per frame
	    void Update()
	    {
	        if(!mPlayer)
            {
				mPlayer = PlayerManager.Instance.getPlayer();
				return;
            }

			if (hasPlayer)
				return;

			float dst = Vector3.Distance(mPlayer.transform.position, transform.position);

			if(dst < activateDistance)
            {
				if(!isOpen)
                {
					mAnimator.Play("Open");
					isOpen = true;
                }
            }
			else
            {
				if (isOpen)
				{
					mAnimator.Play("Close");
					isOpen = false;
				}
			}


	    }

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<Player>())
            {
				hasPlayer = true;
				Timing.RunCoroutine(doElevetorSequence(mPlayer));
			}
        }

		public IEnumerator<float> doElevetorSequence(Player pl)
        {
			pl.changeCurrentMode(PlayerModes.LISTENING);
			pl.transform.position = transform.position - transform.forward;
			pl.direction = transform.forward;
			pl.clearAccel();

			pl.tpc.lockPosition = true;

			mAnimator.Play("Close");

			yield return Timing.WaitForSeconds(1.0f);

			PlayerManager.Instance.lastExit = sceneExit;
			GameManager.Instance.loadScene(sceneToLoad);
		}
    }
}
