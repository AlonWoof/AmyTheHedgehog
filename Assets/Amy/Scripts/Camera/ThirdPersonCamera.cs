using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/* Copyright 2021 Jason Haden */

namespace Amy
{

	public class ThirdPersonCamera : MonoBehaviour
	{
		CinemachineVirtualCamera vCam;

		public Vector2 currentAngle = Vector2.zero;

		public Transform playerTransform;
        public Transform lockedTargetTransform;

        public Player mPlayer;

		public Vector3 targetOffset = new Vector3(0, 0.75f, -1.75f);

        public Vector3 offset_near = new Vector3(0, 0.75f, -1.75f);
        public Vector3 offset_far = new Vector3(0, 1.5f, -3.5f);

        Vector3 lookPosition;

		float maxPitch = 50.0f;

		public float heightOffset = 0.55f;

        const float defaultHeightOffset = 0.55f;
        const float crouchedHeightOffset = 0.35f;

        LayerMask colMask;

        Vector3 desiredPosition;

        public bool isLockedOn = false;
        public bool lockPosition = false;

        public bool rightSide = false;

        public float playerVelocity = 0.0f;
        public bool playerIsCrouched = false;

        CameraDOFBox dof;

        private void Awake()
        {
			vCam = gameObject.AddComponent<CinemachineVirtualCamera>();
            colMask = 1 << LayerMask.NameToLayer("Collision");

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            dof = FindObjectOfType<CameraDOFBox>();
        }

        // Start is called before the first frame update
        void Start()
		{
            centerBehindPlayer();
        }

        public void setPlayerTransform(Transform t)
        {
            playerTransform = t;

            if(t.GetComponent<Player>())
            {
                mPlayer = t.GetComponent<Player>();
            }
        }

        private void OnEnable()
        {
            centerBehindPlayer();
        }

        private void Update()
        {
            handleInput();
            interpolate();
            handlePlayerActions();
        }

        // Update is called once per frame
        void FixedUpdate()
		{
			if (!playerTransform)
				return;

            updateLookPosition();
            updateDesiredPosition();


            if (playerTransform.GetComponent<Player>())
            {
                mPlayer = playerTransform.GetComponent<Player>();
            }

            if(dof)
            {
                dof.focusDist = Vector3.Distance(lookPosition, transform.position);
            }
        }



        public void resetHeightOffset()
        {
            heightOffset = defaultHeightOffset;
        }


        void handleInput()
        {
            float camX = 0;
            float camY = 0;

            float sensitivity = 512;

            if (GameManager.Instance.cameraInputDisabled)
                return;

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


            currentAngle.x += camY * (Time.unscaledDeltaTime * (sensitivity * 0.85f)) * GameManager.Instance.config.lookSensitivity * (GameManager.Instance.config.pitchInvert ? -1.0f : 1.0f);
            currentAngle.y += camX * (Time.unscaledDeltaTime * sensitivity) * (GameManager.Instance.config.lookSensitivity * -1.0f);

            //currentAngle.x += camY * (Time.unscaledDeltaTime * (sensitivity * 0.85f)) * 0.75f;
            //currentAngle.y += camX * (Time.unscaledDeltaTime * sensitivity) * 0.75f;


            if (currentAngle.y > 360)
                currentAngle.y -= 360;

            if (currentAngle.y < -360)
                currentAngle.y += 360;

            if (currentAngle.x > maxPitch)
                currentAngle.x = maxPitch;

            if (currentAngle.x < -maxPitch * 1.5f)
                currentAngle.x = -maxPitch * 1.5f;
        }

        void updateLookPosition()
        {

            if (dof)
            {
                dof.fxEnabled = false;
            }

            lookPosition = playerTransform.position + (heightOffset * Vector3.up);


            if (!mPlayer)
                return;



            if (mPlayer.currentMode == PlayerModes.RUBBING)
            {
                lookPosition = mPlayer.hipBoneTransform.position;// + (crouchedHeightOffset * Vector3.up);

                if(dof)
                {
                    dof.fxEnabled = true;
                }
            }

            /*
            if (!mPlayer)
                return;

            if (!mPlayer.headBoneTransform || !mPlayer.hipBoneTransform)
                return;

            float adjustFac = 0;// Mathf.Clamp01(mPlayer.mForwardVelocity / 16.0f);


            if (playerIsCrouched)
                heightOffset = Mathf.Lerp(heightOffset, crouchedHeightOffset + adjustFac, 0.12f);
            else
                heightOffset = Mathf.Lerp(heightOffset, defaultHeightOffset + adjustFac, 0.2f);


            heightOffset += adjustFac;

            if(mPlayer.currentMode == PlayerModes.HANGING)
            {
                lookPosition = Vector3.Lerp(mPlayer.headBoneTransform.position, mPlayer.hipBoneTransform.position, 0.5f);
            }
            */
        }

		void updateDesiredPosition()
        {

            //Adjust FOV
            float currentFOV = 60.0f; //GameManager.Instance.config.desiredFOV;
            //currentFOV *= Mathf.Lerp(0.8f, 1.2f, currentAngle.x.Remap(-maxPitch, maxPitch, 0, 1));

            if (playerIsCrouched)
                currentFOV *= 0.8f;

            if (mPlayer.currentMode == PlayerModes.RUBBING)
                currentFOV = 45.0f;

            vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView,currentFOV, 0.1f);

            Vector3 translatedOffset = targetOffset;
    

            Vector3 rotatedOffset = Quaternion.Euler(currentAngle.x, currentAngle.y, 0)  * (translatedOffset);

            desiredPosition = lookPosition + rotatedOffset;


            if (!lockPosition)
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.fixedDeltaTime * 30.0f);

            transform.LookAt(lookPosition);

            Occlusion();

        }


        public void centerBehindPlayer()
        {

            if (!playerTransform)
                return;

            currentAngle.x = -10;
            currentAngle.y = (Quaternion.LookRotation(playerTransform.forward).eulerAngles.y);

        }

        public void centerBehindPlayerSmooth(float rate)
        {
            if (!playerTransform)
                return;

            currentAngle.x = Mathf.Lerp(currentAngle.x, -10, rate);
            currentAngle.y = Mathf.Lerp(currentAngle.y, (Quaternion.LookRotation(playerTransform.forward).eulerAngles.y), rate);
        }

        void interpolate()
        {

		}

        public void handlePlayerActions()
        {
            //react to player state stuffs
            //first, reset the variables
            playerIsCrouched = false;
            playerVelocity = 0.0f;

            targetOffset = offset_near;

            //then, check if we have a player instance
            if (!mPlayer)
                return;

            if (mPlayer.currentMode == PlayerModes.RUBBING)
                targetOffset = offset_near * 0.5f;

            //The crouched state is part of the GroundMove component.
            // playerIsCrouched = mPlayer.GetComponent<PlayerBasicMove>().isCrouching;

            float speedFac = (mPlayer.acceleration.z / 16.0f);


            targetOffset = Vector3.Lerp(offset_near, offset_far, speedFac);

            //targetOffset = offset_far;

            targetOffset.y *= mPlayer.mParam.height;

            if(mPlayer.currentMode == PlayerModes.RUBBING)
            {

            }
        }

        public void Occlusion()
        {

            for (int i = 0; i < 4; i++)
            {
                float near = CheckCameraPoints(lookPosition, transform.position);

                if (near != -1.0f)
                {
                    //Debug.Log(near);

                    transform.position = (lookPosition - (transform.forward * near));
                    desiredPosition = transform.position;
                }
            }


        }
        float CheckCameraPoints(Vector3 from, Vector3 to)
        {
            float nearestDist = -1.0f;


            RaycastHit hitInfo;


            Helper.ClipPlanePoints clipPlanePoints = Helper.ClipPlaneAtNear(to);

            // Do I really need debug lines? Come on, guys....
            Debug.DrawLine(from, to + transform.forward * -Camera.main.nearClipPlane);

            Collider col = null;

            //
            if (Physics.Linecast(from, clipPlanePoints.UpperLeft, out hitInfo, colMask))
            {
                if (hitInfo.collider)
                {
                    col = hitInfo.collider;

                    //  if (Helper.isCollidable(hitInfo.collider))
                    nearestDist = hitInfo.distance;
                }
            }

            if (Physics.Linecast(from, clipPlanePoints.LowerLeft, out hitInfo, colMask))
            {
                if (hitInfo.collider)
                {
                    if (hitInfo.distance < nearestDist || nearestDist == -1)
                    {
                        col = hitInfo.collider;

                        //  if (Helper.isCollidable(hitInfo.collider))
                        nearestDist = hitInfo.distance;
                    }
                }
            }

            if (Physics.Linecast(from, clipPlanePoints.UpperRight, out hitInfo, colMask))
            {
                if (hitInfo.collider)
                {
                    if (hitInfo.distance < nearestDist || nearestDist == -1)
                    {
                        col = hitInfo.collider;

                        // if (Helper.isCollidable(hitInfo.collider))
                        nearestDist = hitInfo.distance;
                    }
                }
            }

            if (Physics.Linecast(from, clipPlanePoints.LowerRight, out hitInfo, colMask))
            {
                if (hitInfo.collider)
                {
                    if (hitInfo.distance < nearestDist || nearestDist == -1)
                    {
                        col = hitInfo.collider;

                        // if (Helper.isCollidable(hitInfo.collider))
                        nearestDist = hitInfo.distance;
                    }
                }
            }

            if (Physics.Linecast(from, to + transform.forward * -Camera.main.nearClipPlane, out hitInfo, colMask))
            {
                if (hitInfo.collider)
                {
                    if (hitInfo.distance < nearestDist || nearestDist == -1)
                    {
                        col = hitInfo.collider;

                        //  if (Helper.isCollidable(hitInfo.collider))
                        nearestDist = hitInfo.distance;
                    }
                }
            }


            if (col)
            {
                if (Helper.isCollidable(col))
                {
                    Debug.DrawLine(hitInfo.point, hitInfo.point + Vector3.up * 3, Color.red);
                    //Debug.Log("Collision with: " + col.name);
                }

                if (!Helper.isCollidable(col))
                    nearestDist = -1.0f;
            }

            if (!hitInfo.collider)
            {

                //  nearestDist = -1.0f;
            }

            return nearestDist;
        }

    }
	
}