using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class SlingshotAimer : MonoBehaviour
	{
		public GameObject cam_node;
		public GameObject target_node;
		public GameObject look_node;
		public GameObject slingshot_model;
		public GameObject slingshot_sling;

		public GameObject leftFingers;

		public bool slingPulled = false;

		float speed = 0.0f;

		public Vector3 localHomePos;

		public Quaternion directionRot;
		public Vector3 slingDirection;
		public Vector3 homePosWorld;

		// Start is called before the first frame update
		void Start()
	    {
			localHomePos = slingshot_sling.transform.localPosition;

	    }
	


	    // Update is called once per frame
	    void LateUpdate()
	    {

			homePosWorld = slingshot_sling.transform.parent.TransformPoint(localHomePos);
			

			if (slingPulled)
            {
				slingshot_sling.transform.position = leftFingers.transform.position;
			}
			else
            {
				//slingshot_sling.transform.localPosition = Vector3.Lerp(slingshot_sling.transform.localPosition, localHomePos, 0.5f);

				slingshot_sling.transform.position += slingDirection * ((Vector3.Distance(homePosWorld, slingshot_sling.transform.position) * 150.0f) * Time.deltaTime);
			}

			if(Vector3.Distance(slingshot_sling.transform.localPosition, localHomePos) > 0.0001f)
            {
				slingDirection = Helper.getDirectionTo(slingshot_sling.transform.position, homePosWorld);
			}

			Debug.DrawLine(homePosWorld, homePosWorld + slingDirection.normalized, Color.red, 10.0f);

		}

		public void ReleaseSling()
        {
			slingPulled = false;
        }

		public void GrabSling()
        {
			slingPulled = true;
        }
	}
}
