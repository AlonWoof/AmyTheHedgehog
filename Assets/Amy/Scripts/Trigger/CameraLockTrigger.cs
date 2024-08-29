using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CameraLockTrigger : MonoBehaviour
	{
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
        private void OnTriggerEnter(Collider other)
        {
            Player pl = other.GetComponent<Player>();

            if (!pl)
                return;

            pl.tpc.lockPosition = true;
        }

        private void OnTriggerExit(Collider other)
        {
            Player pl = other.GetComponent<Player>();

            if (!pl)
                return;

            pl.tpc.lockPosition = false;
        }
    }
}
