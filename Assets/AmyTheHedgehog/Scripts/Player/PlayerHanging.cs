using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

/* Copyright 2022 Jason Haden */
namespace Amy
{

	public class PlayerHanging : PlayerMode
	{

        public float speed = 0.5f;

        bool exitingAction = false;

        // Start is called before the first frame update
        void Start()
    	{
            getBaseComponents();
            
        }

        private void OnEnable()
        {
            if (mPlayer.currentMode != PlayerModes.HANGING)
                return;

            Timing.RunCoroutine(doStartHanging());
        }

        // Update is called once per frame
        void Update()
    	{
            //TEST
            if (Input.GetButtonDown("Jump"))
            {
                Timing.RunCoroutine(doLedgePullUp());
                return;
            }
        }

        private void FixedUpdate()
        {
            snapToLedge();
        }

        IEnumerator<float> doStartHanging()
        {
            getBaseComponents();

            grounderIK.weight = 0.0f;
            mPlayer.mCurrentMovement = Vector3.zero;
            mPlayer.mDesiredMovement = Vector3.zero;

            mRigidBody.isKinematic = true;
            //mAnimator.Play("LedgeGrabStart");
            mAnimator.Play("Hanging");

            yield return Timing.WaitForSeconds(0.75f);



            yield return 0f;
        }

        IEnumerator<float> doLedgePullUp()
        {
            // GameManager.Instance.cutsceneMode = true;
            GameManager.Instance.playerInputDisabled = true;

            mPlayer.mCurrentMovement = Vector3.zero;
            mPlayer.mDesiredMovement = Vector3.zero;

            exitingAction = true;
            //acceptInput = false;


            Vector3 start = (transform.position + transform.forward * 0.3f + Vector3.up * 0.5f);
            Vector3 end = start - Vector3.up * 1.2f;

            Debug.DrawLine(start, end, Color.magenta, 15.0f);

            RaycastHit hitInfo = new RaycastHit();

            Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask);

            Vector3 mountPoint = hitInfo.point;

            mAnimator.CrossFade("Hanging_PullUp", 0.12f);

            float fac = 0.0f;
            Vector3 startPos = transform.position;

            //CinemachineBrain br = FindObjectOfType<CinemachineBrain>();
            //br.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
           // br.m_DefaultBlend.m_Time = 0.25f;

            //vCam.enabled = false;

            while (fac < 0.98f)
            {
                fac = mAnimator.GetFloat("animProgress");

                transform.position = Vector3.Lerp(startPos, mountPoint, fac);

                //mPlayer.tpCamera.centerBehindPlayer();

                mountPoint.y = determineFloorPosition().y;

                yield return 0f;
            }

            mAnimator.CrossFade("Idle_Base", 0.15f);

            mPlayer.changeCurrentMode(PlayerModes.GROUNDMOVE);

            yield return Timing.WaitForSeconds(0.2f);

            //GameManager.Instance.cutsceneMode = false;
            GameManager.Instance.playerInputDisabled = false;

            //br.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
        }

        Vector3 determineFloorPosition()
        {

            Vector3 vecs = Vector3.zero;

            float castDist = 0.3f;

            Vector3 forwardOffset = transform.forward * 0.1f;

            vecs += floorCast(Vector3.forward * castDist + forwardOffset);
            vecs += floorCast(-Vector3.forward * castDist + forwardOffset);
            vecs += floorCast(Vector3.right * castDist + forwardOffset);
            vecs += floorCast(-Vector3.right * castDist + forwardOffset);

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


        void snapToLedge()
        {

            getBaseComponents();

            //Don't interfere with the pulling up animation please.
            if (exitingAction)
                return;

            float yOffset = 0.825f;

            Vector3 offsetPos = transform.position;// + Vector3.up * yOffset;


            Vector3 forwardStart = transform.position - (transform.forward * 0.1f) - (Vector3.up * 0.5f);
            Vector3 forwardEnd = forwardStart + transform.forward * 0.35f;

            Vector3 downStart = forwardEnd + Vector3.up * 1.0f;
            Vector3 downEnd = forwardEnd;

            RaycastHit hitInfoForward = new RaycastHit();
            RaycastHit hitInfoDown = new RaycastHit();

            Debug.DrawLine(forwardStart, forwardEnd, Color.red);
            Debug.DrawLine(downStart, downEnd, Color.blue);



            if (Physics.Linecast(forwardStart, forwardEnd, out hitInfoForward, mPlayer.mColMask))
            {

                //mAnimator.Play("Hanging");

                Vector3 horizPos = hitInfoForward.point;
                horizPos.y = transform.position.y;
                transform.position = horizPos;

                mPlayer.mDirection = -hitInfoForward.normal;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mPlayer.mDirection, Vector3.up), 0.2f);

                if (Physics.Linecast(downStart, downEnd, out hitInfoDown, mPlayer.mColMask))
                {
                    //Debug.Log("WALL NORMAL DOT: " + Vector3.Dot(hitInfoForward.normal, -transform.forward));
                    if (Vector3.Dot(hitInfoDown.normal, Vector3.up) > 0.9f)
                    {
                        Vector3 pos = transform.position;
                        pos.y = hitInfoDown.point.y;

                        transform.position = pos;

                    }
                }
            }
        }
	}

}
