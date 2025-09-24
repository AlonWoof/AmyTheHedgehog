using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AmyBike : MonoBehaviour
	{

		public Vector3 direction = Vector3.forward;
		public Vector3 groundNormal = Vector3.up;
		public float accel = 0.0f;
		public const float accelRate = 3.0f;
		public const float maxAccel = 30.0f;

		public LayerMask mColMask;

		public Rigidbody mRigidBody;
		public Animator mAnimator;

	    // Start is called before the first frame update
	    void Start()
	    {
			mRigidBody = GetComponent<Rigidbody>();
			mRigidBody.freezeRotation = true;

			mAnimator = GetComponent<Animator>();

			mColMask = LayerMask.GetMask("Collision");
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			snapToGround();
			handleInput();
	    }

        private void FixedUpdate()
        {
			handleMovement();
        }

        void handleMovement()
        {
			mRigidBody.velocity = transform.forward * accel;
			
		}

        private void LateUpdate()
        {

		}

        void handleInput()
        {



			float h = InputFunctions.getLeftAnalogX();
			float v = InputFunctions.getLeftAnalogY();

			if (Input.GetButton("Jump"))
			{
				accel += Time.deltaTime * accelRate;

			}
			else
			{
				accel -= Time.deltaTime * accelRate;
			}

			if(v < 0)
            {
				accel -= v * Time.deltaTime * accelRate;
            }

			accel = Mathf.Clamp(accel, 0, maxAccel);

			mAnimator.SetFloat("turn", h);
			mAnimator.SetFloat("accel", accel);

			if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
				return;

			transform.rotation = transform.rotation * Quaternion.Euler(0, h, 0);
		}

		void snapToGround()
        {
			Vector3 start = transform.position + Vector3.up;
			Vector3 end = transform.position - Vector3.up * 300.0f;

			RaycastHit hitInfo = new RaycastHit();

			if(Physics.Linecast(start, end, out hitInfo, mColMask))
            {
				transform.position = hitInfo.point;
				groundNormal = hitInfo.normal;
			}

			transform.rotation = Quaternion.LookRotation(transform.forward, groundNormal);
        }
	}
}
