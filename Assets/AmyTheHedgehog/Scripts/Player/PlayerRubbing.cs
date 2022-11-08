using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Amy
{
    public class PlayerRubbing : PlayerMode
    {

        public const float minTimeTilOrgasm = 15.0f;
        public const float maxTimeTilOrgasm = 30.0f;
        public float timeTilOrgasm = 15.0f;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            getBaseComponents();

            if (mPlayer.currentMode != PlayerModes.RUBBING)
                return;

            //Like Professor Oak once said via telepathy, there's a time and place for everything.
            if (!isSafeToRub())
            {
                mPlayer.changeCurrentMode(mPlayer.lastMode);
                return;
            }

            mRigidBody.velocity = Vector3.zero;

            timeTilOrgasm = Random.Range(minTimeTilOrgasm,maxTimeTilOrgasm);

            mAnimator.Play("Masturbate");
        }

        // Update is called once per frame
        void Update()
        {

            if(timeTilOrgasm > 0.0f)
            {
                timeTilOrgasm -= Time.deltaTime;
                mPlayer.healMood(Time.deltaTime * 0.0125f);
            }
            else
            {
                //do a cum
                mPlayer.healMood(Random.Range(0.08f,0.16f));

                //TODO: Replace with a sequence where we do the cum. Leg shaky, maybe a bit of drippy FX too.
                mPlayer.changeCurrentMode(mPlayer.lastMode);
                mAnimator.CrossFade("Idle_Base",0.2f);
            }
        }

        private void LateUpdate()
        {
            groundCheck();
        }


        public bool isSafeToRub()
        {
            //Are we safe enough to rub the bean?

            if (EnemyManager.Instance.currentEnemyPhase != ENEMY_PHASE.PHASE_SNEAK)
                return false;

            foreach(Robot r in FindObjectsOfType<Robot>())
            {
                Debug.Log("ROBO DIST: " + Vector3.Distance(transform.position, r.transform.position));

                if(Vector3.Distance(transform.position,r.transform.position) < 64.0f)
                {
                    return false;
                }
            }

            //Take a bath, stinky. Sorry to all the people that are into that, but no thanks.
            if (PlayerManager.Instance.getCharacterStatus(mPlayer.mChara).dirtiness > 0.5f)
                return false;

            return true;
        }

        void groundCheck()
        {

            Vector3 start = transform.position + Vector3.up * 0.5f;
            Vector3 end = transform.position - (Vector3.up * 0.1f);

            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
            {
                transform.position = hitInfo.point;
                mRigidBody.velocity = Vector3.Lerp(mRigidBody.velocity, Vector3.zero, 0.5f);

            }
            else
            {
                Vector3 velo = mRigidBody.velocity;
                velo.y = -30.0f;
                mRigidBody.velocity = velo;
            }
        }
    }
}