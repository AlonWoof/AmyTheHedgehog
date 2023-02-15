using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{



    public class PlayerDie : PlayerMode
    {

        float gravity = 3.0f;

        bool isWaterDeath = false;
        PlayerHurt hurt;

        public enum DeathType
        {
            Normal,
            Falling,
            Drowned
        }

        public DeathType deathType;

        float verticalVelocity;
        int framesAirborne = 0;

        private void Awake()
        {
            getBaseComponents();
        }

        private void OnEnable()
        {
            getBaseComponents();

            if (mPlayer.currentMode != PlayerModes.DIE)
                return;

            hurt = GetComponent<PlayerHurt>();

            if (deathType == DeathType.Normal)
            {
                mVoice.playVoice(mVoice.die);
                mAnimator.Play("Die_Start");
            }

            if (deathType == DeathType.Falling)
            {
                mVoice.playVoice(mVoice.falling);
            }

            if (deathType == DeathType.Drowned)
            {
                //Replace with actual voice
                mVoice.playVoice(mVoice.largePain);
                mAnimator.Play("Die_Start");
            }

            PlayerManager.Instance.PlayerDieRespawn(deathType);

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            //groundCheck();
            updateIsGrounded();

            PlayerHurt hurt = GetComponent<PlayerHurt>();

            Vector3 dir = Helper.getDirectionTo(transform.position, hurt.knockOrigin);
            dir.y = 0.0f;

            mPlayer.mDirection = dir.normalized;

            transform.rotation = Quaternion.LookRotation(mPlayer.mDirection);
            calculateGravity();
            Vector3 moveVec = -(mPlayer.mDirection * hurt.knockPower);
            moveVec += Vector3.up * (hurt.knockPower * 0.65f);

            moveVec += verticalVelocity * Vector3.up;
            mRigidBody.velocity = moveVec;
            float knockDecay = 8.0f;

            if (hurt.knockPower > 0.0f)
            {
                hurt.knockPower -= (knockDecay * Time.fixedDeltaTime);
            }

            hurt.knockPower = Mathf.Clamp(hurt.knockPower, 0.0f, 128.0f);
        }

        void calculateGravity()
        {

            float gravityMult = mPlayer.mPars.gravity_mult;


            if (mPlayer.isGrounded)
            {
                if (hurt.knockPower < 0.01f)
                    verticalVelocity = Mathf.Lerp(verticalVelocity, -1, 0.5f);
            }
            else
            {
                verticalVelocity = Mathf.Lerp(verticalVelocity, Physics.gravity.y * gravityMult, Time.fixedDeltaTime);
            }
        }

        void groundCheck()
        {
            if (deathType != DeathType.Normal)
                return;

            Vector3 start = transform.position + Vector3.up * 0.5f;
            Vector3 end = transform.position - (Vector3.up * 0.1f);

            RaycastHit hitInfo = new RaycastHit();

            if(Physics.Linecast(start,end,out hitInfo, mPlayer.mColMask))
            {
                transform.position = hitInfo.point;
                mRigidBody.velocity = Vector3.Lerp(mRigidBody.velocity, Vector3.zero, 0.5f);

            }
            else
            {
                Vector3 velo = mRigidBody.velocity;
                velo.y = -gravity;
                mRigidBody.velocity = velo;
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
                if (!mPlayer.isGrounded && hurt.knockPower < 0.5f)
                {
                    //mPlayer.isBallMode = false;
                    //mPlayer.changeCurrentMode(PlayerModes.BASIC_MOVE);
                    // mAnimator.Play("Land");
                    transform.position = hitInfo.point;

                }

                
                mPlayer.isGrounded = true;
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