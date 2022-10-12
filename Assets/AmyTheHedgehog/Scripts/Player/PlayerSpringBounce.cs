using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Copyright 2022 Jason Haden

namespace Amy
{
    public class PlayerSpringBounce : PlayerMode
    {

        public Vector3 springDirection = Vector3.up;
        public float springVelocityLeft = 0.0f;

        const float springVelDecay = 16.0f;
        const float airMoveSpeed = 4.0f;


        // Start is called before the first frame update
        void Awake()
        {
            getBaseComponents();

        }

        private void OnEnable()
        {
            if (mPlayer.currentMode != PlayerModes.SPRING)
                return;

            mRigidBody.isKinematic = false;
            mRigidBody.velocity = Vector3.zero;

            mAnimator.Play("Airborne");
        }

        // Update is called once per frame
        void Update()
        {
            handleInput();
        }

        private void FixedUpdate()
        {
            updateMovement();
        }

        public void updateMovement()
        {
            mRigidBody.velocity = springDirection * springVelocityLeft + (mPlayer.mDirection * mPlayer.mForwardVelocity);

            
            springVelocityLeft -= (springVelDecay * Time.fixedDeltaTime);

            mPlayer.groundNormal = springDirection;

            if(springVelocityLeft < 0.0f)
            {
                springVelocityLeft = 0.0f;
                mPlayer.changeCurrentMode(PlayerModes.BASIC_MOVE);
                mPlayer.isGrounded = false;
            }
        }

        void handleInput()
        {

            if (GameManager.Instance.playerInputDisabled)
                return;


            float h = InputFunctions.getLeftAnalogX();
            float v = InputFunctions.getLeftAnalogY();


            if (Mathf.Abs(h) == 0 && Mathf.Abs(v) == 0)
            {
                mPlayer.mDesiredMovement = Vector3.zero;
                return;
            }

            Vector3 targetDirection = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1.0f);
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            Vector3 camAngle = GameManager.Instance.mainCamera.transform.forward;
            camAngle.y = 0;
            camAngle.Normalize();

            mPlayer.mDesiredMovement = (targetRotation * camAngle) * (targetDirection.magnitude);

            //Direction change penalty
            float dirChange = Vector3.Dot(mPlayer.mDirection, mPlayer.mDesiredMovement.normalized);

            mPlayer.mDirection = mPlayer.mDesiredMovement.normalized;

            if (mPlayer.mForwardVelocity < airMoveSpeed)
                mPlayer.mForwardVelocity += airMoveSpeed * Time.fixedDeltaTime;


            mPlayer.mForwardVelocity *= dirChange;

            if (dirChange < -0.5f)
                mPlayer.mForwardVelocity = Mathf.Lerp(mPlayer.mForwardVelocity, 0, 0.5f);
        }

        public void setSpringVelocity(Vector3 dir, float power)
        {
            springDirection = dir;
            springVelocityLeft = power;

            mPlayer.mForwardVelocity *= 0.25f;
        }
    }
}