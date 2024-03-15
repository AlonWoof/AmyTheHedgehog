using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	public enum DoorColor
    {
		Pink,
		Blue,
		Green,
		Yellow
    }

	public class WorldDoor : MonoBehaviour
	{

		public float range = 8.0f;

		bool isOpen = false;
		bool isWarping = false;

		public DoorColor color;
		public Animator mAnimator;

		public string destinationScene = "default";
		public int destinationExit = 0;

		// Start is called before the first frame update
		void Start()
	    {
			mAnimator = GetComponentInChildren<Animator>();
			isWarping = false;
		}
	
	    // Update is called once per frame
	    void Update()
	    {

			if (Time.frameCount % 15 == 0)
				checkPlayerInRange();

			if (isWarping)
				Time.timeScale = 0.01f;
	    }


		void checkPlayerInRange()
        {
			if (!PlayerManager.Instance.mPlayerInstance)
				return;

			if (!canOpenDoor())
				return;

			if (PlayerManager.Instance.mPlayerInstance.currentMode != PlayerModes.NORMAL)
			{

				if (isOpen)
				{
					mAnimator.Play("Close");
					isOpen = false;
				}

				return;

			}

			float dist = Vector3.Distance(transform.position + Vector3.up, PlayerManager.Instance.mPlayerInstance.transform.position);

			if(dist < range)
            {
				if(!isOpen)
                {
					mAnimator.Play("Open");
					isOpen = true;
                }
            }
			else
            {
				if(isOpen)
                {
					mAnimator.Play("Close");
					isOpen = false;
				}
            }
        }

		bool canOpenDoor()
        {
			switch(color)
            {
				case DoorColor.Pink:
					return true;

				default:
					return false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
			if (!isOpen)
				return;

			//No double-warpsies. 
			if (isWarping)
				return;

			Player nPlayer = other.GetComponentInChildren<Player>();

			if (!nPlayer)
				return;

			//Only if we in normal mode.
			if (nPlayer.currentMode != PlayerModes.NORMAL)
				return;

			PlayerManager.Instance.lastExit = destinationExit;
			GameManager.Instance.loadScene(destinationScene, true);

			isWarping = true;
		}
    }
}
