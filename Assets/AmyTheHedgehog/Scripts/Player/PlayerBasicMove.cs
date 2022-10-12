using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jason Haden */
namespace Amy
{
	public class PlayerBasicMove : PlayerMode
	{

        public const float baseRunSpeed = 5.0f;
        public const float baseWalkSpeed = 0.75f;

        public const float runSpeedAccel = 4.0f;
        public const float runSpeedDeccel = 6.0f;
        public const float slopeInfluence = 0.5f;

        //Max slope when standing still.
        public const float baseMaxSlope = 45.0f;

        //Amount of slope tolerance when running full speed.
        const float slopeVariance = 20.0f;

        const float baseJumpPower = 4.5f;
        const float baseJumpHangTime = 1.5f;

        public float slopeAmount = 0.0f;
        public float jumpTimer = 0.0f;
        public float verticalVelocity = 0.0f;

        int framesAirborne = 0;
        bool isSliding = false;
        bool isSkidding = false;
        float skidTimeout = 0.0f;

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
            verticalVelocity = 0.0f;
            mPlayer.groundNormal = Vector3.up;
        }

        // Update is called once per frame
        void Update()
    	{
            handleInput();
            
        }

        private void FixedUpdate()
        {
            
            calculateGravity();
            handleMovement();
            lerpValues();
        }

        private void LateUpdate()
        {
            updateIKSolver();
            updateIsGrounded();
        }

        void lerpValues()
        {
            
            if (mPlayer.mForwardVelocity > 0.0f && mPlayer.mDesiredMovement.magnitude <= 0.05f)
            {
                mPlayer.mForwardVelocity -= (runSpeedDeccel * Time.fixedDeltaTime);
            }

            if (mPlayer.mForwardVelocity < 0.0f && mPlayer.mDesiredMovement.magnitude <= 0.05f)
            {
                mPlayer.mForwardVelocity += (runSpeedDeccel * Time.fixedDeltaTime);
            }

            if(mPlayer.mForwardVelocity > baseRunSpeed)
            {
                mPlayer.mForwardVelocity -= ((runSpeedDeccel * 0.5f) * Time.fixedDeltaTime);
            }

            //if (mPlayer.mForwardVelocity <= 0.0f)
              //  mPlayer.mForwardVelocity = 0.0f;

            if (Mathf.Abs(mPlayer.mForwardVelocity) < ((runSpeedDeccel * Time.fixedDeltaTime)))
                mPlayer.mForwardVelocity = Mathf.Lerp(mPlayer.mForwardVelocity, 0, Time.fixedDeltaTime * 3.0f);

            float animSpeedMult = Mathf.Clamp((mPlayer.mForwardVelocity / baseRunSpeed), 0.5f, 8.0f);

            mAnimator.SetFloat("animSpeedMult", animSpeedMult);

            if (footStepFX)
            {
                if (jumpTimer > 0.0f || !mPlayer.isGrounded)
                {
                    footStepFX.globalVolume = 1.0f;
                }
                else
                {
                    float power = Helper.remapRange(mPlayer.mForwardVelocity, baseWalkSpeed, baseRunSpeed, 0, 1.0f);
                    power = Mathf.Clamp(power, 0.35f, 1.0f);
                    footStepFX.globalVolume = power;
                }
            }

        }

        void updateIKSolver()
        {

            if (!grounderIK)
                return;

            if (!mPlayer.isGrounded || mRigidBody.isKinematic)
                grounderIK.weight = Mathf.Lerp(grounderIK.weight, 0.0f, 0.5f);
            else if(Mathf.Abs(mPlayer.mForwardVelocity) < 0.15f)
                grounderIK.weight = Mathf.Lerp(grounderIK.weight, 1.0f, 0.5f);
        }

        void handleInput()
        {


            if (skidTimeout > 0.0f)
            {
                mPlayer.mForwardVelocity = Mathf.Lerp(0.0f, baseRunSpeed, skidTimeout);

                if (skidTimeout - Time.fixedDeltaTime <= 0.0f)
                    mPlayer.mForwardVelocity = 0.0f;


                skidTimeout -= Time.fixedDeltaTime;
            }

               

            if (GameManager.Instance.playerInputDisabled)
                return;

            if (Input.GetButtonDown("Jump"))
                Jump();

            if (!Input.GetButton("Jump"))
                jumpTimer = 0.0f;

            float h = InputFunctions.getLeftAnalogX();
            float v = InputFunctions.getLeftAnalogY();

            //DEBUG
            if (Input.GetKeyDown(KeyCode.Alpha9))
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

            if (mPlayer.mForwardVelocity > (baseRunSpeed * 0.9f) && dirChange < -0.5f && skidTimeout <= 0.0f)
            {
                skidTimeout = 1.0f;
                mAnimator.Play("Skid");
            }

            if (skidTimeout > 0.0f)
                return;

            if (mPlayer.mForwardVelocity < getRunSpeed(mPlayer.mDesiredMovement.magnitude))
                mPlayer.mForwardVelocity += (runSpeedAccel * (1.0f + (slopeAmount * slopeInfluence)) * Time.fixedDeltaTime);

            //Debug.Log("NORMAL DOT: " + slopeAmount + " ANGLE: " + Vector3.Angle(Vector3.up, mPlayer.groundNormal));

            mPlayer.mDirection = mPlayer.mDesiredMovement.normalized;

            if(skidTimeout <= 0.0f)
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
            mAnimator.Play("Mouth_Jumping");
            mVoice.playVoiceDelayed(Random.Range(0.05f,0.1f),mVoice.jumping);
            mPlayer.spawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_basicJump, transform.position);
           // mPlayer.isBallMode = true;
        }

        #endregion

    }

}
