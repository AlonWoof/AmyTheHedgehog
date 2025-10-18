using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class BikeCam : MonoBehaviour
	{

		public AmyBike mBike;

		Vector3 currentOffset = new Vector3(0, 1.0f, -1.5f);
		Vector3 farOffset = new Vector3(0, 1.25f, -2.0f);
		Vector3 nearOffset = new Vector3(0, 1.0f, -1.5f);

		CinemachineVirtualCamera vCam;

		void getAmyBike()
        {
			mBike = FindObjectOfType<AmyBike>();
			vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        }

	    // Start is called before the first frame update
	    void Start()
	    {
			

		}
	
	    // Update is called once per frame
	    void Update()
	    {
	        if(!mBike || !vCam)
			{
				getAmyBike();
				return;
            }

			float accelFac = mBike.accel / AmyBike.maxAccel;
			Vector3 calculatedPos = mBike.transform.position + (mBike.transform.rotation * currentOffset);

			calculatedPos += transform.up * (Mathf.Sin(Time.time * Mathf.Lerp(30.0f, 75.0f, accelFac)) * 0.001f);

			transform.position = Vector3.Lerp(transform.position, calculatedPos, 0.5f);
			transform.LookAt(mBike.transform.position + Vector3.up * 0.75f + mBike.transform.forward);

			

			currentOffset = Vector3.Lerp(nearOffset, farOffset, accelFac);
			vCam.m_Lens.FieldOfView = Mathf.Lerp(GameManager.Instance.config.desiredFOV * 0.95f, GameManager.Instance.config.desiredFOV * 1.1f, accelFac);

		}
	}
}
