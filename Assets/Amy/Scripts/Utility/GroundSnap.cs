using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class GroundSnap : MonoBehaviour
	{

		public float rayLength = 1.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void LateUpdate()
	    {
			Vector3 start = transform.position + Vector3.up;
			Vector3 end = transform.position - (Vector3.up * rayLength);

			RaycastHit hitInfo = new RaycastHit();
			LayerMask mask = LayerMask.GetMask("Collision");

			if (Physics.Linecast(start, end, out hitInfo, mask))
			{
				transform.position = hitInfo.point;
			}
		}
	}
}
