using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class DashPanel : MonoBehaviour
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


			if (pl)
            {
				pl.direction = transform.forward;
				pl.setAngleInstantly(transform.forward);
				pl.acceleration.z = 30.0f;
				pl.stickTimeout = 1.0f;
				pl.tpc.centerBehindPlayer();
            }
        }
    }
}
