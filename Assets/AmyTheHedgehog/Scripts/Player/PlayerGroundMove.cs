using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jason Haden */
namespace Amy
{

    public class PlayerGroundMove : PlayerMode
    {

        float verticalVelocity = 0.0f;
        float forwardVelocity = 0.0f;

        const float maxGravity = 53.0f;
        const float hangTime = 1.5f;
        const float holdJumpPower = 0.15f;

        float topRunSpeed = 3.0f;
        float slopeRunTime = 1.0f;

        const float accelFactor = 0.1f;
        const float deccelFactor = 0.2f;

        int framesAirborne = 0;
        float jumpTimeLeft = 0.0f;

        

        Vector3 groundPos;
        Vector3 relativeMovement = Vector3.zero;
        public float slopeAmount;
        public bool isSteepSlope = false;

        public AnimationCurve runCurve;

        public bool isCrouching;

        // Start is called before the first frame update
        void Start()
        {
            getBaseComponents();
        }

        void Update()
        {
            handleInput();
            updateAnimator();



        }

        void FixedUpdate()
        {
            updateGroundedStatus();
            calculateVerticalVelocity();
            handleMovement();
            updateIKSolver();
        }


        void handleInput()
        {

            mPlayer.mDesiredMovement = Vector3.zero;



            if (GameManager.Instance.playerInputDisabled)
                return;

            if (Input.GetButtonDown("Jump"))
                Jump();

            if (Input.GetButtonDown("Crouch"))
            {
                isCrouching = !isCrouching;
                mAnimator.SetBool("Crouching", isCrouching);

            }

            if (!Input.GetButton("Jump"))
                jumpTimeLeft = 0.0f;

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            if (GameManager.Instance.usingController)
            {
                h = Input.GetAxis("Left Analog X");
                v = Input.GetAxis("Left Analog Y");
            }

            float deadZone = 0.15f;

            if (Mathf.Abs(h) > deadZone || Mathf.Abs(v) > deadZone)
            {
                // Create a new vector of the horizontal and vertical inputs.
                Vector3 targetDirection = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1.0f);

                // Create a rotation based on this new vector assuming that up is the global y axis.
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

                Vector3 camAngle = GameManager.Instance.mainCamera.transform.forward;
                camAngle.y = 0;
                camAngle.Normalize();

                mPlayer.mDesiredMovement = (targetRotation * camAngle) * (targetDirection.magnitude);

                if(mPlayer.mDesiredMovement.magnitude > 0.2f)
                    mPlayer.mDirection = mPlayer.mDesiredMovement.normalized;

               // mPlayer.mCurrentMovement = Vector3.Lerp(mPlayer.mCurrentMovement, mPlayer.mDesiredMovement, Time.deltaTime * 3.0f);
            }


            mPlayer.mCurrentMovement = Vector3.Lerp(mPlayer.mCurrentMovement, mPlayer.mDesiredMovement, (Time.deltaTime * 8.0f));
        }

        float getCurrentSpeed()
        {
            float baseRunSpeed = 4.0f;

            float finalSpeed = (baseRunSpeed + PlayerManager.Instance.status.speedBonus);

            finalSpeed *= forwardVelocity;

            float turnRatioPenalty = 1.0f - mPlayer.turnRatio;

            finalSpeed *= turnRatioPenalty;

            if (isCrouching)
                finalSpeed *= 0.35f;

            return finalSpeed;
        }

        void handleMovement()
        {

            if (mPlayer.isGrounded && mRigidBody.velocity.y < 0.01f)
                mRigidBody.MovePosition(groundPos);

            if (mPlayer.mDesiredMovement.magnitude > 0.1f && mPlayer.mCurrentMovement.magnitude > 0.1f)
                forwardVelocity = Mathf.Lerp(forwardVelocity, 1, Time.deltaTime * 1.0f);
            else
                forwardVelocity = Mathf.Lerp(forwardVelocity, 0.1f, Time.deltaTime * 1.0f);


            Vector3 movevec = Vector3.zero;

            movevec = mPlayer.mCurrentMovement * getCurrentSpeed();

            movevec.y = verticalVelocity;

            mRigidBody.velocity = movevec;
        }

        void updateAnimator()
        {
            if (!mAnimator)
                return;

            Vector3 rm = transform.InverseTransformDirection(mPlayer.mCurrentMovement);
            rm.y = 0;

            relativeMovement = rm.normalized * mPlayer.mCurrentMovement.magnitude;

            mAnimator.SetFloat("absVelocity", mPlayer.mCurrentMovement.magnitude);

            mAnimator.SetFloat("move_x", relativeMovement.x);
            mAnimator.SetFloat("move_y", verticalVelocity);
            mAnimator.SetFloat("move_z", relativeMovement.z);


        }

        void calculateVerticalVelocity()
        {
            float vertLimit = maxGravity;


            if (!mPlayer.isGrounded)
            {

                float extraJumpPower = (holdJumpPower * (jumpTimeLeft / (1.0f + PlayerManager.Instance.status.jumpTimeBonus)));

                float gravityMult = 1.0f;

                if (isSteepSlope)
                {
                    gravityMult = 3.0f;
                    slopeRunTime = Mathf.Lerp(slopeRunTime, 0.0f, Time.deltaTime);
                }

                verticalVelocity = Mathf.Clamp(Mathf.Lerp(mRigidBody.velocity.y + extraJumpPower, -(40 * gravityMult), 0.73f * Time.fixedDeltaTime * gravityMult), -(vertLimit * gravityMult), vertLimit);
            }
            else
            {

                if (jumpTimeLeft <= 0)
                    vertLimit = 0;

                slopeRunTime = Mathf.Lerp(slopeRunTime, 1.0f, Time.deltaTime);

                verticalVelocity = Mathf.Clamp(mRigidBody.velocity.y, 0, vertLimit);
            }
        }

        void updateIKSolver()
        {

            if (!mPlayer.isGrounded || mRigidBody.isKinematic)
                grounderIK.weight = Mathf.Lerp(grounderIK.weight, 0.0f, 0.5f);
            else
                grounderIK.weight = Mathf.Lerp(grounderIK.weight, 1.0f, 0.5f);
        }

        void updateGroundedStatus()
        {

            float checkLength = 0.1f;

            // if (!mPlayer.isGrounded)
            //     checkLength = 0.5f;

            groundPos = determineFloorPosition();

            if (!isGroundBeneathPlayer(checkLength))
            {
                framesAirborne++;

                if (framesAirborne > 5)
                {
                    if (mPlayer.isGrounded)
                    {
                        mPlayer.isGrounded = false;
                        mAnimator.Play("Airborne");
                    }
                }
            }
            else
            {
                framesAirborne = 0;

                if (!mPlayer.isGrounded)
                {
                    mPlayer.isGrounded = true;
                    mAnimator.Play("Land");
                }
            }
        }

        bool isGroundBeneathPlayer(float offset)
        {
            if (jumpTimeLeft > 0.01f)
            {
                mPlayer.groundNormal = Vector3.up;
                mPlayer.isGrounded = false;
                return false;
            }

            Vector3 start = transform.position + Vector3.up * 0.25f;
            Vector3 end = transform.position - Vector3.up * offset;

            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
            {
                mPlayer.groundNormal = hitInfo.normal;
                return true;
            }

            mPlayer.groundNormal = Vector3.up;
            return false;
        }

        Vector3 determineFloorPosition()
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
            end = transform.position - (Vector3.up * 0.2f) + offset;

            Debug.DrawLine(start, end, Color.green);

            RaycastHit hitInfo = new RaycastHit();


            if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
            {
                return hitInfo.point;
            }

            return transform.position + offset;
        }


        void Jump(bool ignoreGrounded = false, bool isDouble = false)
        {
            if (!mPlayer.isGrounded && !isDouble && !ignoreGrounded)
                return;

            float slopeMult = Vector3.Dot(Vector3.up, mPlayer.groundNormal);

            float jumpPower = 8.0f + PlayerManager.Instance.status.jumpPowerBonus;

            mRigidBody.velocity = (jumpPower * Vector3.up);

            jumpTimeLeft = hangTime + PlayerManager.Instance.status.jumpTimeBonus;
            //mAnimator.Play("Jump");

            mAnimator.Play("Airborne");
        }
    }


}
