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
		Red,
		Yellow
    }

	public class WorldDoor : MonoBehaviour
	{

		public float range = 8.0f;

		bool isOpen = false;

		public DoorColor color;
		public Animator mAnimator;

		public string destinationScene = "default";
		public int destinationExit = 0;

		// Start is called before the first frame update
		void Start()
	    {
			mAnimator = GetComponentInChildren<Animator>();

		}
	
	    // Update is called once per frame
	    void Update()
	    {

			if (Time.frameCount % 15 == 0)
				checkPlayerInRange();
	    }


		void checkPlayerInRange()
        {
			if (!PlayerManager.Instance.mPlayerInstance)
				return;

			if (!canOpenDoor())
				return;

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

			Player nPlayer = other.GetComponentInChildren<Player>();

			if (!nPlayer)
				return;

			PlayerManager.Instance.lastExit = destinationExit;
			GameManager.Instance.loadScene(destinationScene, true);

		}
    }
}
