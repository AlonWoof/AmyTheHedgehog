using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class NonSwimmableWater : MonoBehaviour
	{

		Player mPlayer = null;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
			if(mPlayer)
            {
				Vector3 spd = mPlayer.speed;
				Vector3 accel = mPlayer.acceleration;

				accel.y = Mathf.Clamp(accel.y, -3.0f, 0.0f);
				mPlayer.acceleration = accel;

            }

	    }

        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<Player>())
            {
				mPlayer = other.GetComponent<Player>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
			if (other.GetComponent<Player>())
			{
				mPlayer = null;
			}
		}
    }
}
