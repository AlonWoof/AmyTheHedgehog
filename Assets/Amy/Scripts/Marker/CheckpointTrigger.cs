using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CheckpointTrigger : MonoBehaviour
	{

		public Transform checkpointLocation;

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
            if(other.GetComponentInChildren<Player>())
            {
				PlayerManager.Instance.playerCheckpoint.position = checkpointLocation.position;
				PlayerManager.Instance.playerCheckpoint.rotation = checkpointLocation.rotation;
            }
        }
    }
}
