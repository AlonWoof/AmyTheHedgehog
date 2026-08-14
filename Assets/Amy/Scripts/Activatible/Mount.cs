using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Mount : MonoBehaviour
	{
		public Transform mountPoint;
		public string mountAnimation = "Sit";
		public string quitText = "Get Up";

		public float timeout = 0.0f;

		public bool isMounted = false;
		public bool canUseFirstPerson = true;

		Player mPlayer;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        if(isMounted && mPlayer && timeout < 0.01f)
            {
				mPlayer.transform.position = mountPoint.transform.position;
				mPlayer.transform.rotation = mountPoint.transform.rotation;

				if (mPlayer.currentMode == PlayerModes.FIRSTPERSON)
					return;

				UIManager.Instance.contextButton.setActionText(quitText);

				if (Input.GetButtonDown("Action"))
				{
					unmountPlayer();
				}

				if (Input.GetButtonDown("View"))
				{
					mPlayer.changeCurrentMode(PlayerModes.FIRSTPERSON);
				}
			}

			if (timeout > 0.0f)
				timeout -= Time.deltaTime;

		}

		//This function name sounds kinda lewd....
		public void mountPlayer()
        {

			if (isMounted)
				return;

			mPlayer = PlayerManager.Instance.mPlayerInstance;

			if (!mPlayer)
				return;

			mPlayer.transform.position = mountPoint.transform.position;
			mPlayer.transform.rotation = mountPoint.transform.rotation;

			mPlayer.changeCurrentMode(PlayerModes.CUTSCENE);
			mPlayer.mAnimator.Play(mountAnimation);
			mPlayer.disableCollision();

			timeout = 1.0f;
			isMounted = true;
        }

		public void unmountPlayer()
        {
			if (!isMounted)
				return;

			if (!mPlayer)
				return;


			mPlayer.transform.position = mountPoint.transform.position;
			mPlayer.transform.rotation = mountPoint.transform.rotation;

			mPlayer.resetGroundFlags();
			mPlayer.enableCollision();
			mPlayer.changeCurrentMode(PlayerModes.NORMAL);
			mPlayer.mAnimator.Play("Idle");
			mPlayer.interactTimeout = 1.0f;

			isMounted = false;
        }
	}
}
