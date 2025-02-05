using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using Cinemachine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerFirstPerson : PlayerMode
	{

		public CinemachineVirtualCamera fpCam;
		public Transform headBone;
        Vector3 headBonePos;


        public Vector2 camRot = Vector2.zero;
        public bool acceptInput = false;

		// Start is called before the first frame update
		void Start()
	    {
			getBaseComponents();
            initializedFirstPersonCam();

        }

        void initializedFirstPersonCam()
        {
            headBone = mPlayer.getBoneByName("head");

            GameObject inst = new GameObject("FirstPersonCam");
            inst.transform.SetParent(gameObject.transform);
            inst.transform.position = headBone.transform.position;


            fpCam = inst.AddComponent<CinemachineVirtualCamera>();
            fpCam.m_Priority = 100;
            fpCam.m_Lens.FieldOfView = GameManager.Instance.config.desiredFOV;

            fpCam.enabled = false;
        }

        private void OnEnable()
        {
			getBaseComponents();

			if (mPlayer.currentMode != PlayerModes.FIRSTPERSON)
			{
				enabled = false;
				return;
			}

            if (!fpCam)
            {
                initializedFirstPersonCam();
            }

            mPlayer.clearAccel();
            mPlayer.clearSpeed();

			mPlayer.tpc.centerBehindPlayer();
			Timing.RunCoroutine(doViewTransitionIn());

            mPlayer.clearActivatible();
            mPlayer.areaDetector.enabled = false;
            mPlayer.lookAtController.lookingAtTarget = false;
            headBonePos = headBone.transform.position;
        }

        private void OnDisable()
        {
            if (!fpCam)
                return;

            fpCam.enabled = false;

            if (headBone)
                headBone.transform.localScale = Vector3.one;

            mPlayer.areaDetector.enabled = true;

            //vcam.Priority = -1;
            //toggleShadows(true);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            

        }

        private void FixedUpdate()
        {
            
        }

        void UpdateRotation()
        {
            Quaternion rot = Quaternion.Euler(camRot.x, camRot.y, 0.0f);

            if (acceptInput)
                headBone.transform.localScale = Vector3.one * 0.001f;

            fpCam.transform.rotation = Quaternion.LookRotation(transform.forward) * rot;


            fpCam.transform.position = headBonePos + (headBone.transform.up * 0.1f) + ((headBone.transform.forward * 0.1f));
        }

        void handleInput()
        {

            if (GameManager.Instance.playerInputDisabled)
                return;

            if (GameManager.Instance.gamePaused || PlayerManager.Instance.itemMenuOpen)
                return;

            float camX = 0;
            float camY = 0;

            float sensitivity = 256;

            if (!GameManager.Instance.usingController)
            {
                camX = Input.GetAxisRaw("Mouse X");
                camY = Input.GetAxisRaw("Mouse Y");

            }
            else
            {
                if (!Input.GetButton("Zoom"))
                {
                    camX = Input.GetAxisRaw("Right Analog X");
                    camY = Input.GetAxisRaw("Right Analog Y");
                    sensitivity = 128;
                }
            }

            camRot.x -= camY * (Time.unscaledDeltaTime * (sensitivity * 0.85f)) * GameManager.Instance.config.lookSensitivity * (GameManager.Instance.config.pitchInvert ? -1.0f : 1.0f);
            camRot.y -= camX * (Time.unscaledDeltaTime * sensitivity) * (GameManager.Instance.config.lookSensitivity * -1.0f);

            //if (Input.GetButton("Action"))
            //     desiredFOVMult = 0.5f;
            //else
            //   desiredFOVMult = 1.0f;

            if (Mathf.Abs(camRot.y) > 69)
            {

                Vector3 dir = fpCam.transform.forward;
                dir.y = 0;
                dir.Normalize();

                //mPlayer.mDirection = Vector3.Lerp(mPlayer.mDirection, dir, Time.deltaTime * 8.0f);


                mPlayer.direction = (Vector3.Lerp(mPlayer.direction, dir, Time.deltaTime * 0.85f));
                transform.rotation = Quaternion.LookRotation(mPlayer.direction, Vector3.up);

                camRot.y = Mathf.Lerp(camRot.y, 0, 0.25f * Time.deltaTime);
            }

            camRot.x = Mathf.Clamp(camRot.x, -80, 65);
            camRot.y = Mathf.Clamp(camRot.y, -70, 70);

            if (Input.GetButtonDown("View"))
            {
                acceptInput = false;
                headBone.transform.localScale = Vector3.one;
                //mPlayer.changeCurrentMode(mPlayer.lastMode);
                Timing.RunCoroutine(doViewTransitionOut());
            }
        }

        IEnumerator<float> doViewTransitionIn()
        {
            yield return Timing.WaitForOneFrame;

            //Fix for initialization
            if (!enabled)
                yield break;

            GameManager.Instance.playerInputDisabled = true;
            GameManager.Instance.cameraInputDisabled = true;

            mAnimator.SetInteger("idleAnimation", 0);

            mPlayer.tpc.disableCameraCollision();
            mPlayer.biped.fixTransforms = false;

            if (mPlayer.lastMode != PlayerModes.SWIMMING)
                mAnimator.CrossFade("Idle", 0.1f);

            mPlayer.tpc.centerBehindPlayer();

            CinemachineBrain br = FindObjectOfType<CinemachineBrain>();
            br.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            br.m_DefaultBlend.m_Time = 0.15f;
            yield return Timing.WaitForOneFrame;

            fpCam.enabled = true;

            float dist = Vector3.Distance(br.transform.position, fpCam.transform.position);



            while (br.IsBlending)
            {
                mPlayer.tpc.centerBehindPlayer();
                yield return 0f;
            }

            yield return Timing.WaitForSeconds(0.15f);
            acceptInput = true;

            br.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

            GameManager.Instance.playerInputDisabled = false;
            GameManager.Instance.cameraInputDisabled = false;
        }

        IEnumerator<float> doViewTransitionOut()
        {

            GameManager.Instance.playerInputDisabled = true;
            GameManager.Instance.cameraInputDisabled = true;

            CinemachineBrain br = FindObjectOfType<CinemachineBrain>();
            br.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            br.m_DefaultBlend.m_Time = 0.15f;
            yield return 0f;
            fpCam.enabled = false;

            float dist = Vector3.Distance(br.transform.position, fpCam.transform.position);

            yield return 0f;

            while (br.IsBlending)
            {
                yield return 0f;
                headBone.transform.localScale = Vector3.one;
                mPlayer.tpc.centerBehindPlayer();
            }

            br.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;

            mPlayer.changeCurrentMode(mPlayer.lastMode);
            mPlayer.tpc.enableCameraCollision();
            mPlayer.biped.fixTransforms = true;

            GameManager.Instance.playerInputDisabled = false;
            GameManager.Instance.cameraInputDisabled = false;
        }


        // Update is called once per frame
        void Update()
	    {
            if (!acceptInput)
                return;

            handleInput();
            UpdateRotation();
        }


	}
}
