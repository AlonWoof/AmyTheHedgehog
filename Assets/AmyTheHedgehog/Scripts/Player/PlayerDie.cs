using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{



    public class PlayerDie : PlayerMode
    {

        float gravity = 3.0f;

        bool isWaterDeath = false;

        public enum DeathType
        {
            Normal,
            Falling,
            Drowned
        }

        public DeathType deathType;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            getBaseComponents();

            if (mPlayer.currentMode != PlayerModes.DIE)
                return;

            if(deathType == DeathType.Normal)
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
            groundCheck();
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
    }


}