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
		public const float accelRate = 3.5f;
		public const float maxAccel = 15.0f;

		public Vector3 horizVelocity = Vector3.zero;
		public Vector3 acceleration = Vector3.zero;
		public Vector3 speed = Vector3.zero;
		public float friction = 0.2f;
		public float slopeAmount = 0.0f;
		public float forwardAccel = 10.0f;

		Vector3 stickAngle = Vector3.zero;
		float stickPower = 0.0f;

		public LayerMask mColMask;

		public Rigidbody mRigidBody;
		public Animator mAnimator;

		public bool isGrounded = false;
		public int framesAirborne = 0;

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
			//snapToGround();
			getFloorAverageNormal();
			updateRotation();
			handleInput();
	    }

        private void FixedUpdate()
        {
			calculateVerticalVelocity();
			applyFriction();
			handleMovement();

        }

        void handleMovement()
        {
			speed = Vector3.Lerp(speed, transform.rotation * acceleration, Time.deltaTime * 8.0f);
			mRigidBody.velocity = speed;

		}

        private void LateUpdate()
        {
			
		}

        void handleInput()
        {

			float h = InputFunctions.getLeftAnalogX();
			float v = InputFunctions.getLeftAnalogY();

			if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
				return;

			stickAngle = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1.0f);

			if (Input.GetButton("Jump"))
				stickPower = Mathf.Lerp(stickPower, 1, Time.deltaTime * accelRate);
			else
				stickPower = Mathf.Lerp(stickPower, 0, Time.deltaTime * accelRate);


			float slopePenalty = Mathf.Clamp(slopeAmount, 0, 2.0f);

			float forward_accel = (stickPower * forwardAccel);
			forward_accel += (slopeAmount * forwardAccel);

			acceleration.z += forward_accel * Time.deltaTime;

			mAnimator.SetFloat("turn", h);
			mAnimator.SetFloat("accel", accel);

			if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
				return;

			direction = Quaternion.Euler(0, h, 0) * direction;
		}



		void getFloorAverageNormal()
        {
			//Front wheel
			Vector3 start = (transform.position + transform.forward * 0.35f) + transform.up;
			Vector3 end = (transform.position + transform.forward * 0.35f) - (transform.up * Time.deltaTime);

			Vector3 frontNorm = Vector3.up;
			float frontYPos = transform.position.y;

			RaycastHit hitInfo = new RaycastHit();

			Debug.DrawLine(start, end, Color.green);

			if (Physics.Linecast(start, end, out hitInfo, mColMask))
			{
				frontNorm = hitInfo.normal;
				frontYPos = hitInfo.point.y;

				framesAirborne = 0;
				isGrounded = true;
			}
			else
            {
				framesAirborne++;
			}


			//Back wheel
			start = (transform.position - transform.forward * 0.35f) + transform.up;
			end = (transform.position - transform.forward * 0.35f) - (transform.up * Time.deltaTime);

			Vector3 backNorm = Vector3.up;
			float backYPos = transform.position.y;

			hitInfo = new RaycastHit();

			Debug.DrawLine(start, end, Color.red);

			if (Physics.Linecast(start, end, out hitInfo, mColMask))
			{
				backNorm = hitInfo.normal;
				backYPos = hitInfo.point.y;
				framesAirborne = 0;
				isGrounded = true;
			}

			groundNormal = Vector3.Lerp(frontNorm, backNorm, 0.5f);

			Debug.DrawLine(transform.position, transform.position + groundNormal, Color.red, 10.0f);

			//transform.rotation =Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.forward, groundNormal), 0.5f);

			Vector3 pos = transform.position;
			pos.y = Mathf.Lerp(frontYPos, backYPos, 0.25f);
			transform.position = pos;


			if (framesAirborne > 5)
			{
				isGrounded = false;
			}

			slopeAmount = Vector3.Dot(direction.normalized, groundNormal);

			if (Mathf.Abs(slopeAmount) < 0.03f)
				slopeAmount = 0.0f;
		}

		void updateRotation()
        {

			Quaternion dirRot = Quaternion.LookRotation(direction.normalized, Vector3.up);

			Quaternion slopeRot = Quaternion.FromToRotation(Vector3.up, groundNormal) * dirRot;

			transform.rotation = Quaternion.Lerp(transform.rotation, slopeRot, 0.15f);
		}

		void calculateVerticalVelocity()
        {
			float gravityMult = 1.0f;


			float verticalVelocity = acceleration.y;

			if (isGrounded)
			{
				verticalVelocity = Mathf.Lerp(verticalVelocity, 0, 0.25f);
			}
			else
			{
				verticalVelocity = Mathf.Lerp(verticalVelocity, Physics.gravity.y * gravityMult, Time.fixedDeltaTime * 1.5f);

			}

			acceleration.y = verticalVelocity;

			if (Mathf.Abs(acceleration.y) < 0.01f)
				acceleration.y = 0.0f;

			//if (Mathf.Abs(speed.y) < 0.01f)
			//	speed.y = 0.0f;
		}

		public void applyFriction()
		{

			Vector3 mAccel = acceleration;

			//TODO: groundFriction and airResistance should be consts at the top
			float groundFriction = 0.0f;
			float airResistance = 0.0f;

			float friction = (groundFriction * speed.magnitude) * Time.fixedDeltaTime;

			if (isGrounded)
				friction = (airResistance * speed.magnitude) * Time.fixedDeltaTime;

			//if (stickPower < 0.01f)
			//	friction *= 6.0f;

			if (mAccel.z > friction)
				mAccel.z -= friction;

			if (mAccel.z < -friction)
				mAccel.z += friction;

			if (mAccel.x > friction)
				mAccel.x -= friction;

			if (mAccel.x < -friction)
				mAccel.x += friction;


			if (Mathf.Abs(mAccel.z) < 0.01f)
				mAccel.z = 0.0f;

			if (Mathf.Abs(mAccel.x) < 0.01f)
				mAccel.x = 0.0f;


			mAccel.y -= (0.1f * mAccel.z) * Time.fixedDeltaTime;

			acceleration = mAccel;


		}

	}
}
