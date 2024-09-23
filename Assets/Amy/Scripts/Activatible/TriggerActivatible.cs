using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TriggerActivatible : MonoBehaviour
	{
		public string interactionLabel = "Sit";
		public UnityEvent onActivate;

		public bool directionDependent = false;
		public bool turnAroundPlayer = true;

		public Player mPlayer;

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		public bool canActivate(Player pl)
		{
			if (directionDependent)
			{
				Vector3 dir = Helper.getDirectionTo(transform.position, pl.transform.position + Vector3.up * 0.5f);

				//Debug.Log(Vector3.Dot(-pl.transform.forward, transform.forward));

				if (Vector3.Dot(-pl.transform.forward, transform.forward) < 0.5f)
					return false;
			}

			return true;
		}


		public void Activate(Player pl)
		{
			if (!canActivate(pl))
				return;

			onActivate.Invoke();
		}

        private void OnTriggerEnter(Collider other)
        {
			if (other.GetComponentInChildren<Player>())
			{
				mPlayer = other.GetComponentInChildren<Player>();
			}
		}

        private void OnTriggerStay(Collider other)
        {
			if (other.GetComponentInChildren<Player>())
			{
				mPlayer = other.GetComponentInChildren<Player>();
			}


			if (mPlayer)
			{

				if (mPlayer.interactTimeout <= 0.0f)
					UIManager.Instance.contextButton.setActionText(interactionLabel);

				if (Input.GetButtonDown("Action"))
				{
					if (!mPlayer.areaDetector.closestActivatible)
					{
						Activate(mPlayer);
						mPlayer.interactTimeout = 1.0f;
						UIManager.Instance.contextButton.clearActionText();
					}
				}
			}
		}

        private void OnTriggerExit(Collider other)
        {
			mPlayer = null;
        }

    }
}
