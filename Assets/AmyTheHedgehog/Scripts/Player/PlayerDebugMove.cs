using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jason Haden */
namespace Amy
{

    public class PlayerDebugMove : PlayerMode
    {

        public Vector3 mVelocity;
        public float moveSpeed = 12.0f;

        // Start is called before the first frame update
        public void Start()
        {
            getBaseComponents();
        }

        private void OnEnable()
        {
            getBaseComponents();

            if (mPlayer.currentMode != PlayerModes.DEBUG_MOVE)
                return;

            mRigidBody.isKinematic = true;
        }

        private void OnDisable()
        {
            if (mPlayer.currentMode == PlayerModes.DEBUG_MOVE)
                return;

            mRigidBody.isKinematic = false;
        }

        private void Update()
        {
            handleInput();
        }


        void handleInput()
        {

            mVelocity = Vector3.zero;

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (GameManager.Instance.usingController)
            {
                h = Input.GetAxis("Left Analog X");
                v = Input.GetAxis("Left Analog Y");
            }


            float deadZone = 0.1f;

            if (Mathf.Abs(h) > deadZone || Mathf.Abs(v) > deadZone)
            {
                // Create a new vector of the horizontal and vertical inputs.
                Vector3 targetDirection = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1.0f);

                // Create a rotation based on this new vector assuming that up is the global y axis.
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

                Vector3 camAngle = GameManager.Instance.mainCamera.transform.forward;
                camAngle.y = 0;
                camAngle.Normalize();

                mVelocity = (targetRotation * camAngle) * (targetDirection.magnitude);
            }


            //Veritcal movement

            if (Input.GetKey(KeyCode.KeypadPlus))
                mVelocity.y = 1.0f;

            if (Input.GetKey(KeyCode.KeypadMinus))
                mVelocity.y = -1.0f;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (mVelocity.magnitude < 0.1f)
                return;

            mRigidBody.transform.position += mVelocity * (moveSpeed * Time.fixedDeltaTime);
        }


    }

}
