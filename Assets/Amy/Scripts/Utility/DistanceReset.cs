using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class DistanceReset : MonoBehaviour
	{
		public float z_dist = 255.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {

			Vector3 pos = transform.position;

			if (pos.z > z_dist)
			{
				pos.z -= z_dist;
				transform.position = pos;
			}
	    }
	}
}
