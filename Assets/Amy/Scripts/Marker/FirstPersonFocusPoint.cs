using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class FirstPersonFocusPoint : MonoBehaviour
	{
		Player mPlayer;

		public float distance = 3.0f;
		public float fovMult = 0.75f;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        if(!mPlayer)
            {
				mPlayer = PlayerManager.Instance.getPlayer();
				return;
			}

			if (mPlayer.currentMode != PlayerModes.FIRSTPERSON)
				return;

			if (Vector3.Distance(GameManager.Instance.mainCamera.transform.position, transform.position) > distance)
				return;

			Vector3 camPos = GameManager.Instance.mainCamera.transform.position;
			Vector3 camDir = GameManager.Instance.mainCamera.transform.forward;
			Vector3 pointDir = Helper.getDirectionTo(camPos, transform.position);

			float dotProduct = Vector3.Dot(camDir, pointDir);

			if (dotProduct > 0.9f)
				mPlayer.modeFirstPerson.desiredFOVMult = fovMult;
			else
				mPlayer.modeFirstPerson.desiredFOVMult = 1.0f;

		}
	}
}
