using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jason Haden */
namespace Amy
{
	public class PlayerBasicMove : PlayerMode
	{

        const float baseRunSpeed = 4.0f;
        const float baseWalkSpeed = 1.5f;

        const float runSpeedAccel = 2.0f;
        const float runSpeedDeccel = 5.0f;
        const float slopeInfluence = 0.5f;

        //Max slope when standing still.
        const float baseMaxSlope = 45.0f;

        //Amount of slope tolerance when running full speed.
        const float slopeVariance = 20.0f;

        const float baseJumpPower = 6.0f;
        const float baseJumpHangTime = 1.5f;

        public float slopeAmount = 0.0f;
        public float jumpTimer = 0.0f;
        public float verticalVelocity = 0.0f;

        int framesAirborne = 0;
        bool isSliding = false;


        // Start is called before the first frame update
        void Start()
    	{
            getBaseComponents();
    	}

        private void OnEnable()
        {
            if (!mPlayer)
                return;

            if (mPlayer.currentMode != PlayerModes.BASIC_MOVE)
                return;

            mRigidBody.isKinematic = false;
        }

        // Update is called once per frame
        void Update()
    	{
            handleInput();
            lerpValues();
        }

        private void FixedUpdate()
        {
            updateIsGrounded();
            calculateGravity();
            handleMovement();
            
            updateIKSolver();
        }

        void lerpValues()
        {

            if (mPlayer.mForwardVelocity > 0.0f)
            {
                mPlayer.mForwardVelocity -= (runSpeedDeccel * Time.deltaTime);
            }

            if (mPlayer.mForwardVelocity < 0.0f)
            {
                mPlayer.mForwardVelocity += (runSpeedDeccel * Time.deltaTime);
            }

            if (Mathf.Abs(mPlayer.mForwardVelocity) < ((runSpeedDeccel * Time.deltaTime)))
                mPlayer.mForwardVelocity = Mathf.Lerp(mPlayer.mForwardVelocity, 0, Time.deltaTime * 3.0f);

            if (Mathf.Abs(mPlayer.mForwardVelocity) < 0.01f)
                mPlayer.mForwardVelocity = 0.0f;


            if (jumpTimer > 0.0f)
                jumpTimer -= Time.deltaTime;


            float animSpeedMult = Mathf.Clamp((mPlayer.mForwardVelocity / baseRunSpeed), 0.5f, 8.0f);

            mAnimator.SetFloat("animSpeedMult", animSpeedMult);

        }

        void updateIKSolver()
        {

            if (!mPlayer.isGrounded || mRigidBody.isKinematic)
                grounderIK.weight = Mathf.Lerp(grounderIK.weight, 0.0f, 0.5f);
            else
                grounderIK.weight = Mathf.Lerp(grounderIK.weight, 1.0f, 0.5f);
        }

        void handleInput()
        {

            if (Input.GetButtonDown("Jump"))
                Jump();

            if (!Input.GetButton("Jump"))
                jumpTimer = 0.0f;

            float h = InputFunctions.getLeftAnalogX();
            float v = InputFunctions.getLeftAnalogY();

            //DEBUG
            if (Input.GetButtonDown("Attack"))
                mPlayer.mForwardVelocity += baseRunSpeed * 3.0f;

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

            if (mPlayer.mForwardVelocity < getRunSpeed(mPlayer.mDesiredMovement.magnitude))
                mPlayer.mForwardVelocity += (runSpeedAccel * (1.0f + (slopeAmount * slopeInfluence)) * Time.fixedDeltaTime);

            //Debug.Log("NORMAL DOT: " + slopeAmount + " ANGLE: " + Vector3.Angle(Vector3.up, mPlayer.groundNormal));


            mPlayer.mForwardVelocity *= dirChange;

            if (dirChange < -0.5f)
                mPlayer.mForwardVelocity = Mathf.Lerp(mPlayer.mForwardVelocity, 0, 0.5f);
        }

        void handleMovement()
        {
            if (mPlayer.isGrounded && mRigidBody.velocity.y < 0.01f && jumpTimer < 0.01f)
                mRigidBody.MovePosition(getFloorPosition());

            getMaxSlope();

            Vector3 moveVec = transform.forward * mPlayer.mForwardVelocity;
            moveVec.y += verticalVelocity;

            mRigidBody.velocity = moveVec;

            mAnimator.SetFloat("move_y", verticalVelocity);
            mAnimator.SetFloat("move_z", mPlayer.mForwardVelocity / baseRunSpeed);

            if (jumpTimer > 0.0f)
                jumpTimer -= Time.fixedDeltaTime;
        }

        void calculateGravity()
        {

            float gravityMult = 1.5f;

            if (isSliding)
                gravityMult = 2.3f;

            if(mPlayer.isGrounded)
            {
                if(jumpTimer < 0.01f)
                    verticalVelocity = Mathf.Lerp(verticalVelocity, -1, 0.5f);
            }
            else
            {
                float extraJumpPower = (0.15f * (jumpTimer / baseJumpHangTime));

                verticalVelocity = Mathf.Lerp(verticalVelocity + extraJumpPower, Physics.gravity.y * gravityMult, Time.fixedDeltaTime);
            }
        }

        #region GetSet

        float getRunSpeed(float power = 1.0f)
        {

            //Debug.Log("PAWA: " + power);


            float walkSpeedTotal = baseWalkSpeed;
            walkSpeedTotal += ((baseWalkSpeed * 1.0f) * slopeAmount);

            float runSpeedTotal = baseRunSpeed;
            runSpeedTotal += ((baseRunSpeed * 0.5f) * slopeAmount);

            if (slopeAmount > 0.45f)
            {
                walkSpeedTotal = baseWalkSpeed * 2.0f;
                runSpeedTotal = baseRunSpeed * 2.0f;
            }

            Debug.Log(slopeAmount);

            if (power < 0.4f)
                return walkSpeedTotal;

            if (power > 0.95f)
                return runSpeedTotal;

            float newPower = Helper.remapRange(power, 0.4f, 1.0f, 0.0f, 1.0f);


            return Mathf.Lerp(newPower, walkSpeedTotal, runSpeedTotal);
        }

        float getMaxSlope()
        {
            float slope = baseMaxSlope;

            float runSpeed = (mPlayer.mForwardVelocity / baseRunSpeed);

            slope += Mathf.Lerp(0, slopeVariance, runSpeed);

           // Debug.Log("SLOPE TOLERANCE: " + slope);

            return slope;
        }



        #endregion
        #region PhysicsChecks

        void updateIsGrounded()
        {

            Vector3 start = transform.position + (0.15f * Vector3.up);
            Vector3 end = transform.position - (Vector3.up * 0.15f);
            RaycastHit hitInfo = new RaycastHit();

            Debug.DrawLine(start, end, Color.cyan, 0.1f);

            float angle = Vector3.Angle(Vector3.up, mPlayer.groundNormal);

            if (angle > getMaxSlope())
                isSliding = true;

            if (angle > 85.0f)
                isSliding = true;

            if(Physics.Linecast(start,end,out hitInfo,mPlayer.mColMask) && !isSliding)
            {
                if (!mPlayer.isGrounded)
                {
                    mPlayer.isBallMode = false;
                    mAnimator.Play("Land");
                }

                   

                    mPlayer.isGrounded = true;
                    mPlayer.groundNormal = hitInfo.normal;
                    framesAirborne = 0;

                    slopeAmount = Vector3.Dot(mPlayer.mDirection.normalized, hitInfo.normal);
            }
            else
            {
                framesAirborne++;

                if (framesAirborne > 5)
                {
                    if (mPlayer.isGrounded)
                    {
                        if(jumpTimer < 0.001f)
                            mAnimator.CrossFade("Airborne",0.25f);

                        mPlayer.isGrounded = false;
                        mPlayer.groundNormal = Vector3.up;
                        slopeAmount = 0.0f;

                    }
                }
            }

            if (!isSliding)
                return;


            if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
            {
                if (Vector3.Angle(Vector3.up, hitInfo.normal) < 20.0f)
                    isSliding = false;
            }
        }

        Vector3 getFloorPosition()
        {
            Vector3 vecs = Vector3.zero;

            float castDist = 0.3f * (1 - Mathf.Abs(Vector3.Dot(mPlayer.groundNormal, Vector3.up)));

            vecs += floorCast(Vector3.forward * castDist);
            vecs += floorCast(-Vector3.forward * castDist);
            vecs += floorCast(Vector3.right * castDist);
            vecs += floorCast(-Vector3.right * castDist);

            return vecs * 0.25f;
        }

        Vector3 floorCast(Vector3 offset)
        {
            Vector3 start, end;

            start = transform.position + (Vector3.up * 0.5f) + offset;
            end = transform.position - (Vector3.up * 0.5f) + offset;

            Debug.DrawLine(start, end, Color.green);

            RaycastHit hitInfo = new RaycastHit();


            if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
            {
                return hitInfo.point;
            }

            return transform.position + offset;
        }

        #endregion
        #region Actions
        public void Jump(bool ignoreGrounded = false, bool isDouble = false)
        {
            if (!mPlayer.isGrounded && !isDouble && !ignoreGrounded)
                return;

            float slopeMult = Mathf.Clamp01(1.0f + slopeAmount);

            if (slopeMult < 0.45f)
                return;

            Debug.Log("SLOPE JUMP MULT: " + slopeMult);

            float jumpPower = baseJumpPower * slopeMult;


            verticalVelocity = jumpPower;
            jumpTimer = baseJumpHangTime * slopeMult;
            mAnimator.Play("Jump");
            mVoice.playVoice(mVoice.jumping);
           // mPlayer.isBallMode = true;
        }

        #endregion

    }

}
