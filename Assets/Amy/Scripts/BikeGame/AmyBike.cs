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
		float moveInput, steerInput;
		public float maxSpeed, acceleration, steerStrength;
		public Rigidbody mRigidBody;
        public Transform bikeModel;
        public Animator bikeAnimator;

        private void Start()
        {
            mRigidBody.transform.SetParent(null);
        }

        private void Update()
        {
            handleInput();
            handleAnimation();
            transform.position = mRigidBody.position;
        }

        private void FixedUpdate()
        {
            handleMovement();
            handleRotation();

        }

        void handleMovement()
        {
            mRigidBody.velocity = Vector3.Lerp(mRigidBody.velocity, maxSpeed * moveInput *transform.forward, Time.fixedDeltaTime * acceleration);

        }

        void handleRotation()
        {
            transform.Rotate(0, steerInput * moveInput * steerStrength * Time.fixedDeltaTime, 0, Space.World);
            //bikeModel.transform.localRotation = Quaternion.Euler(0,0, steerInput * moveInput * -20.0f);
        }

        void handleAnimation()
        {
            bikeAnimator.SetFloat("accel", moveInput);
            bikeAnimator.SetFloat("turn", steerInput);
        }

        void handleInput()
        {
            moveInput = 0.0f;
            steerInput = 0.0f;

            if (GameManager.Instance.playerInputDisabled)
                return;

            float h = InputFunctions.getLeftAnalogX();
            float v = InputFunctions.getLeftAnalogY();

            if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f && !Input.GetButton("Action"))
                return;


            if (!GameManager.Instance.usingController)
                moveInput = v;
            else
                moveInput = (Input.GetButton("Action")) ? 1.0f : 0.0f;



            steerInput = h;
        }
    }
}
