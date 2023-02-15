using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class PlayerHurt : PlayerMode
    {

        public float knockPower = 0.0f;
        public Vector3 knockOrigin;

        float verticalVelocity;
        int framesAirborne = 0;

        private void Awake()
        {
            getBaseComponents();
        }

        private void OnEnable()
        {
            if (mPlayer.currentMode != PlayerModes.HURT)
                return;

            mRigidBody.isKinematic = false;
            grounderIK.enabled = false;
            mPlayer.isGrounded = false;
            verticalVelocity = 0.0f;
            framesAirborne = 0;
        }


        public void setKnockBack(Vector3 origin, float power = 5.0f)
        {
            knockPower = power;
            knockOrigin = origin;
            verticalVelocity = 3.0f;

            mAnimator.Play("Hurt");
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (mPlayer.currentMode != PlayerModes.HURT)
                return;

            updateIsGrounded();

            Vector3 dir = Helper.getDirectionTo(transform.position, knockOrigin);
            dir.y = 0.0f;

            mPlayer.mDirection = dir.normalized;

            transform.rotation = Quaternion.LookRotation(mPlayer.mDirection);

            mPlayer.mForwardVelocity = 0.0f;
            mPlayer.mCurrentMovement = Vector3.zero;

            calculateGravity();

            Vector3 moveVec = -(mPlayer.mDirection * knockPower);
            moveVec += Vector3.up * (knockPower * 0.65f);

            moveVec += verticalVelocity * Vector3.up;

            mRigidBody.velocity = moveVec;

            float knockDecay = 8.0f;

            if(knockPower > 0.0f)
            {
                knockPower -= (knockDecay * Time.fixedDeltaTime);
            }

            knockPower = Mathf.Clamp(knockPower, 0.0f, 128.0f);
        }

        void calculateGravity()
        {

            float gravityMult = mPlayer.mPars.gravity_mult;


            if (mPlayer.isGrounded)
            {
                if (knockPower < 0.01f)
                    verticalVelocity = Mathf.Lerp(verticalVelocity, -1, 0.5f);
            }
            else
            {
                verticalVelocity = Mathf.Lerp(verticalVelocity, Physics.gravity.y * gravityMult, Time.fixedDeltaTime);
            }
        }

        void updateIsGrounded()
        {

            Vector3 start = transform.position + (0.15f * Vector3.up);
            Vector3 end = transform.position - (Vector3.up * 0.15f);
            RaycastHit hitInfo = new RaycastHit();

            Debug.DrawLine(start, end, Color.cyan, 0.1f);

            float angle = Vector3.Angle(Vector3.up, mPlayer.groundNormal);

            if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
            {
                if (!mPlayer.isGrounded && knockPower < 0.5f)
                {
                    //mPlayer.isBallMode = false;
                    mPlayer.changeCurrentMode(PlayerModes.BASIC_MOVE);
                    mAnimator.Play("Hurt_Land");
                    mPlayer.isGrounded = true;
                }

                mPlayer.groundNormal = hitInfo.normal;
                framesAirborne = 0;

            }
            else
            {
                framesAirborne++;

                if (framesAirborne > 5)
                {
                    if (mPlayer.isGrounded)
                    {

                        mPlayer.isGrounded = false;
                        mPlayer.groundNormal = Vector3.up;

                    }
                }
            }

        }
    }
}