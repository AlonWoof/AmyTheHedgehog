using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerKilled : PlayerMode
	{
        float gravity = 3.0f;

        bool isWaterDeath = false;
        PlayerHurt hurt;

        public enum DeathType
        {
            Normal,
            Falling,
            Drowned,
            Corrupted
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

            if (mPlayer.currentMode != PlayerModes.KILLED)
            {
                enabled = false;
                return;
            }

            hurt = GetComponent<PlayerHurt>();

            if (deathType == DeathType.Normal)
            {
                mPlayer.mVoice.playVoice(mPlayer.mVoice.die, true);
                mAnimator.Play("Die_Start");
            }

            if (deathType == DeathType.Falling)
            {
                mPlayer.tpc.lockPosition = true;
                mPlayer.mVoice.playVoice(mPlayer.mVoice.falling, true);
            }

            if (deathType == DeathType.Drowned)
            {
                //Replace with actual voice and animation
                // mPlayer.mVoice.playVoice(mPlayer.mVoice.largePain, true);
                mRigidBody.useGravity = false;

                Vector3 velo = mRigidBody.velocity;
                float yVel = velo.y;
                velo *= 0.5f;
                velo.y = Mathf.Clamp(yVel, -6.0f, -2.0f);
                mRigidBody.velocity = velo;

                mAnimator.Play("Drown_Start");
            }

            if(!mPlayer.isAiControlled)
                PlayerManager.Instance.PlayerDieRespawn(deathType);

        }

        // Update is called once per frame
        void Update()
	    {
            PlayerHurt hurt = GetComponent<PlayerHurt>();

            Vector3 dir = Helper.getDirectionTo(transform.position, hurt.knockOrigin);
            dir.y = 0.0f;

            if(hurt.knockPower > 0.1f)
                mPlayer.setAngleInstantly(dir.normalized);

            if (hurt.knockPower > 0.5f)
            {
                //mPlayer.acceleration.y = knockPower * 0.25f;
            }

            mPlayer.acceleration.z = -hurt.knockPower;


            float knockDecay = 8.0f;

            checkForKillTrigger();

            if (hurt.knockPower > 0.0f)
            {
                hurt.knockPower -= (knockDecay * Time.fixedDeltaTime);
            }

            hurt.knockPower = Mathf.Clamp(hurt.knockPower, 0.0f, 128.0f);

            mPlayer.CalcSlope();

            mPlayer.groundNormal = Vector3.up;

        }



        private void FixedUpdate()
        {
            if (deathType == DeathType.Drowned)
                return;

            mPlayer.CalcVerticalVelocity();
            mPlayer.applyFriction();
            mPlayer.updatePosition();
            mPlayer.updateRotation();
        }


        void checkForKillTrigger()
        {
            Vector3 yeetDirection = -transform.forward;

            Vector3 start = transform.position + yeetDirection * 5.0f;
            Vector3 end = start + (Vector3.down * 128.0f);

            RaycastHit hitInfo = new RaycastHit();

            Debug.DrawLine(start, end, Color.yellow, 1.0f);

            if (Physics.Linecast(start, end, out hitInfo))
            {
                if (hitInfo.collider.GetComponentInChildren<FallingKillPlane>())
                {
                    hurt.knockPower *= 0.5f;

                    Debug.DrawLine(start, end, Color.red, 1.0f);
                }
            }
        }

    }
}
