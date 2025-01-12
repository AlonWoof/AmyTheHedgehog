using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;
using MEC;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CameraFree : MonoBehaviour
	{
		public Vector3 moveVector = Vector3.zero;
		public Vector3 lookAngle = Vector3.zero;
		public Vector3 stickAngle;
		public float stickPower;
		public CinemachineVirtualCamera vCam;

		public bool controlWasDisabled = false;

		public float fov_mult = 1.0f;
		public float actionTimeout = 0.0f;


	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

        private void OnEnable()
        {
			vCam = GetComponent<CinemachineVirtualCamera>();

			vCam.Priority = 99999;
			Time.timeScale = 0;
			GameManager.Instance.mainCamera.GetComponent<Cinemachine.CinemachineBrain>().m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.LateUpdate;

			controlWasDisabled = GameManager.Instance.playerInputDisabled;
			GameManager.Instance.disablePlayerInput();

			transform.position = GameManager.Instance.mainCamera.transform.position;
			lookAngle = GameManager.Instance.mainCamera.transform.rotation.eulerAngles;
			moveVector = Vector3.zero;
			fov_mult = 1.0f;
		}

        private void OnDisable()
        {
			vCam.Priority = -99999;
			Time.timeScale = 1.0f;
			GameManager.Instance.mainCamera.GetComponent<Cinemachine.CinemachineBrain>().m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate;


			if (!controlWasDisabled)
				GameManager.Instance.enablePlayerInput();

		}

        // Update is called once per frame
        void LateUpdate()
	    {

			handleInput();
			updateRotation();
			updateFOV();
		}

		void updateFOV()
        {
			fov_mult = Mathf.Clamp(fov_mult, 0.25f, 2.0f);

			float currentFOV = GameManager.Instance.config.desiredFOV * fov_mult;

			vCam.m_Lens.FieldOfView = currentFOV;
		}

		void updateRotation()
        {

			lookAngle.x = Mathf.Clamp(lookAngle.x, -90.0f, 90.0f);

			if (lookAngle.y > 360)
				lookAngle.y -= 360;

			if (lookAngle.y < -360)
				lookAngle.y += 360;

			transform.rotation = Quaternion.Euler(lookAngle);
		}

		IEnumerator<float> doFrameAdvance()
        {
			Time.timeScale = 1.0f;
			yield return Timing.WaitForOneFrame;
			Time.timeScale = 0.0f;

        }

		public IEnumerator<float> doScreenshot()
		{
			UIManager.Instance.hudEnabled = false;

			yield return Timing.WaitForSeconds(0.01f);

			string screenLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			screenLocation += "/Amy the Hedgehog/";

			if (!System.IO.Directory.Exists(screenLocation))
			{
				System.IO.Directory.CreateDirectory(screenLocation);
			}

			string fileName = "AmyScreenshot_" + System.DateTime.Now.ToFileTime() + ".png";

			ScreenCapture.CaptureScreenshot(screenLocation + fileName, 2);
			yield return Timing.WaitForSeconds(0.01f);

			UIManager.Instance.fadeScreen(false, 0.01f, true);
			yield return Timing.WaitForSeconds(0.02f);
			UIManager.Instance.fadeScreen(true, 0.38f, true);

			yield return Timing.WaitForSeconds(0.75f);
			UIManager.Instance.hudEnabled = true;
		}

		void functionInput()
        {
			if (Input.GetKey(KeyCode.RightArrow))
			{
				if (actionTimeout < 0.001f)
				{
					Timing.RunCoroutine(doFrameAdvance(), Segment.RealtimeUpdate);
					actionTimeout = 0.2f;
				}
			}

			if(Input.GetKey(KeyCode.UpArrow))
            {
				fov_mult -= Time.unscaledDeltaTime;
            }

			if (Input.GetKey(KeyCode.DownArrow))
			{
				fov_mult += Time.unscaledDeltaTime;
			}

			if (Input.GetButtonDown("RightBumper") || Input.GetKeyDown(KeyCode.R))
			{

				if (actionTimeout < 0.001f)
				{
					Timing.RunCoroutine(doScreenshot(), Segment.RealtimeUpdate);
					actionTimeout = 0.1f;
				}
			}


			if (actionTimeout > 0.0f)
				actionTimeout -= Time.unscaledDeltaTime;
		}

		void movementInput()
        {
			float h = InputFunctions.getLeftAnalogX();
			float v = InputFunctions.getLeftAnalogY();

			if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
				return;

			stickAngle = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1.0f);
			stickPower = stickAngle.magnitude;

			Vector3 targetDirection = stickAngle;
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

			Vector3 mDir = transform.rotation * stickAngle;

			float camSpeed = 5.0f;

			transform.position = transform.position + (mDir * Time.unscaledDeltaTime * camSpeed);
		}

		void lookInput()
        {
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

			lookAngle.x -= camY * (Time.unscaledDeltaTime * (sensitivity * 0.85f)) * GameManager.Instance.config.lookSensitivity * (GameManager.Instance.config.pitchInvert ? -1.0f : 1.0f);
			lookAngle.y -= camX * (Time.unscaledDeltaTime * sensitivity) * (GameManager.Instance.config.lookSensitivity * -1.0f);

		}

		void handleInput()
        {
			lookInput();
			movementInput();
			functionInput();
		}
	}
}
