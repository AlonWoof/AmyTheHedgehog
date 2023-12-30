using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class MovingPlatform : MonoBehaviour
	{

		public Vector3 lastPos;
		public Vector3 velocity = Vector3.zero;

		float timeLeft = 0.0f;

		public Rigidbody testObject;

	    // Start is called before the first frame update
	    void Start()
	    {
			testObject.transform.position = transform.position + transform.right * 10.0f;

			lastPos = transform.position;
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void FixedUpdate()
        {

			

			
			if (timeLeft <= 0.0f)
			{
				velocity = (transform.position - lastPos) * Time.fixedDeltaTime * 30.0f;

				//velocity *= Time.fixedDeltaTime;

				lastPos = transform.position;

				timeLeft = 1.0f / 30.0f;

				if(testObject)
					testObject.velocity = velocity;
			}
			else
            {
				timeLeft -= Time.fixedDeltaTime;
            }

			
		}

        private void OnTriggerEnter(Collider other)
        {
			Player pl = other.gameObject.GetComponentInChildren<Player>();

			if(pl)
            {
				//pl.transform.SetParent(transform);
            }
        }

        private void OnTriggerStay(Collider other)
        {
			Player pl = other.gameObject.GetComponentInChildren<Player>();

			if (pl)
			{
				pl.platformVelocity = velocity;
			}
		}

        private void OnTriggerExit(Collider other)
        {
			Player pl = other.gameObject.GetComponentInChildren<Player>();

			if (pl)
			{
				//pl.transform.SetParent(null);
				//pl.platformVelocity = velocity;
			}
		}
    }
}
