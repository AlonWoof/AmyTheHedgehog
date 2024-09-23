using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CameraCenterTrigger : MonoBehaviour
	{
	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnTriggerEnter(Collider other)
        {
			Player pl = other.GetComponent<Player>();

			if(pl)
            {
				pl.tpc.centerBehindPlayer();
            }
        }
    }
}
