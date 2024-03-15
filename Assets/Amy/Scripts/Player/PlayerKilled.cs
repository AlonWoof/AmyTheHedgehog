using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerKilled : PlayerMode
	{
        float gravity = 3.0f;

        bool isWaterDeath = false;
        PlayerHurt hurt;

        public float knockPower = 0.0f;
        public Vector3 knockOrigin;

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
                mPlayer.mVoice.playVoice(mPlayer.mVoice.falling, true);
            }

            if (deathType == DeathType.Drowned)
            {
                //Replace with actual voice
                mPlayer.mVoice.playVoice(mPlayer.mVoice.largePain, true);
                mAnimator.Play("Die_Start");
            }

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

            mPlayer.CalcVerticalVelocity();
            mPlayer.applyFriction();
            mPlayer.updatePosition();
            mPlayer.updateRotation();
        }


    }
}
