using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class PlayerSwimming : PlayerMode
    {

        float waterSurfacePos;

        Vector3 mDesiredMovement;
        Vector3 mCurrentMovement;

        float desiredVerticalMovement = 0.0f;
        float currentVerticalMovement = 0.0f;
        float speedMult = 0.0f;

        float swimSpeed = 6.0f;


        // Start is called before the first frame update
        void Start()
        {
            getBaseComponents();

            if (mPlayer.currentMode != PlayerModes.SWIMMING)
            {
                enabled = false;
                return;
            }
        }

        private void OnEnable()
        {
            getBaseComponents();

            if (mPlayer.currentMode != PlayerModes.SWIMMING)
                return;

            swimSpeed = mPlayer.mParam.swimSpeed;


            //mRigidBody.velocity = Vector3.up * mPlayer.GetComponent<PlayerBasicMove>().verticalVelocity;

            Vector3 pos = transform.position;
            pos.y = mPlayer.getWaterYPos() - mPlayer.headOffsetFromGround;

            mPlayer.mAnimator.Play("Swimming");

            mCurrentMovement = mPlayer.speed;

            if (Mathf.Abs(mRigidBody.velocity.y) > 0.2f)
            {
                GameObject splish = GameObject.Instantiate(GameManager.Instance.systemData.RES_ActorWaterSplashFX);
                Vector3 fxpos = transform.position;
                fxpos.y = mPlayer.getWaterYPos();
                splish.transform.position = fxpos;
            }

            // transform.position = pos;

        }

        // Update is called once per frame
        void Update()
        {
            handleInput();

            mCurrentMovement = Vector3.Lerp(mCurrentMovement, mDesiredMovement, Time.deltaTime * 8.0f);
            currentVerticalMovement = Mathf.Lerp(currentVerticalMovement, desiredVerticalMovement, Time.deltaTime * 8.0f);

            speedMult = mPlayer.mAnimator.GetFloat("animProgress");
            mPlayer.mAnimator.SetFloat("swim_z", mCurrentMovement.magnitude);
            mPlayer.mAnimator.SetFloat("swim_y", currentVerticalMovement);
        }

        private void FixedUpdate()
        {
            handleMovement();
            checkForSurfacing();
        }

        private void LateUpdate()
        {
            preventGroundClipping();


        }

        void checkForSurfacing()
        {

            if (mPlayer.getWaterDepth() < mPlayer.mParam.height * 0.5f)
            {
                Vector3 start = mPlayer.headBoneTransform.position;
                Vector3 end = transform.position - (Vector3.up * 0.2f);

                RaycastHit hitInfo = new RaycastHit();

                if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
                {
                   // if(Vector3.Dot(hitInfo.normal,Vector3.up) < 0.5f)
                   // {
                        mPlayer.changeCurrentMode(PlayerModes.NORMAL);
                        mPlayer.mAnimator.Play("Walk");
                  // }
                }

            }
        }     
        
        void preventGroundClipping()
        {
                Vector3 start = transform.position + (Vector3.up * mPlayer.mParam.height * 0.5f);
                Vector3 end = transform.position;

                RaycastHit hitInfo = new RaycastHit();

                if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
                {
                    transform.position = hitInfo.point;
                }
        }

        bool isAtSurface()
        {
            if (mPlayer.getWaterDepth() <= mPlayer.headOffsetFromGround)
                return true;

            return false;
        }

        public void bindToWaterBounds()
        {
            float y_limit = mPlayer.getWaterYPos() - mPlayer.headOffsetFromGround;

            if(transform.position.y > y_limit)
            {
                Vector3 npos = transform.position;
                npos.y = Mathf.Clamp(npos.y, -9000.0f, y_limit);
                transform.position = npos;
            }
        }

        void handleMovement()
        {

            Vector3 moveVec = ((mCurrentMovement * swimSpeed) * speedMult) + Vector3.up * (currentVerticalMovement * (swimSpeed * 1.5f));

            if (transform.position.y < mPlayer.getWaterYPos() - mPlayer.headOffsetFromGround && currentVerticalMovement > -0.1f)// && diveTimer < 0.01f)
            {
                //float up
                moveVec.y = Mathf.Clamp(moveVec.y + (Mathf.Clamp(Mathf.Abs(mPlayer.getWaterDepth()),0,1.0f) * 1.5f), moveVec.y + 0.5f, 8.0f);
            }

            if (transform.position.y > mPlayer.getWaterYPos() - (mPlayer.headOffsetFromGround - 0.02f))
            {
                //fall down
                moveVec.y -= (Mathf.Abs(mPlayer.getWaterDepth()) * 8.0f);
                moveVec.y = Mathf.Clamp(moveVec.y, -32, -3);
            }


            // mRigidBody.velocity = ((mPlayer.mCurrentMovement * swimSpeed) * speedMult) + (Vector3.up * 0.23f) + Vector3.up * (currentVerticalMovement * swimSpeed);// * mPlayer.mForwardVelocity;
            //mRigidBody.velocity = ;
            mPlayer.setVelocityDirectly(Vector3.Lerp(mRigidBody.velocity, moveVec, Time.fixedDeltaTime * 2.0f));


            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mPlayer.direction, Vector3.up), Time.deltaTime * 3.0f);
        }

        void handleInput()
        {

            desiredVerticalMovement = 0.0f;

            if (isAtSurface() && Input.GetButtonDown("Jump"))
            {
                mPlayer.changeCurrentMode(PlayerModes.NORMAL);
                mPlayer.acceleration.z = mRigidBody.velocity.magnitude;
                mPlayer.speed = mRigidBody.velocity;
                mPlayer.Jump(true);
                mPlayer.acceleration.y += mPlayer.mParam.jumpSpeed;
               
            }

            if (Input.GetButton("Jump") && !isAtSurface())
                desiredVerticalMovement = 1.0f;


            if (Input.GetButton("Action"))
                desiredVerticalMovement = -1.0f;

            float h = InputFunctions.getLeftAnalogX();
            float v = InputFunctions.getLeftAnalogY();


            if (Mathf.Abs(h) == 0 && Mathf.Abs(v) == 0)
            {
                mDesiredMovement = Vector3.zero;
                
                return;
            }




            Vector3 targetDirection = Vector3.ClampMagnitude(new Vector3(h, 0, v), 1.0f);
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            Vector3 camAngle = GameManager.Instance.mainCamera.transform.forward;
            camAngle.y = 0;
            camAngle.Normalize();

            

            mDesiredMovement = (targetRotation * camAngle) * (targetDirection.magnitude);
           
            mPlayer.direction = mDesiredMovement.normalized;

        }
    }
}
