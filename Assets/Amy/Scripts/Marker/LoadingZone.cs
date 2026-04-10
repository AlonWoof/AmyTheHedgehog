using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using Cinemachine;

/* Copyright 2026 Jennifer Haden */
namespace Amy
{

	public class LoadingZone : MonoBehaviour
	{
        public string targetScene = "default";
        public int exitNumber = 0;
        public bool whiteFade = false;

        public Transform moveTarget;
        public CinemachineVirtualCamera vCam;
        public float timer = 1.0f;

        Player mPlayer;

        public bool waitForMove = false;
        public bool isTrigger = true;


    	// Start is called before the first frame update
    	void Start()
    	{
            mPlayer = PlayerManager.Instance.getPlayer();

            if (vCam)
            {
                vCam.m_Priority = -100;
                vCam.gameObject.SetActive(false);
            }
        }

    	// Update is called once per frame
    	void Update()
    	{
    	    if(!mPlayer)
                mPlayer = PlayerManager.Instance.getPlayer();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isTrigger)
                return;

            if(other.GetComponent<Player>())
            {
                Timing.RunCoroutine(sceneTransitionCutscene(),Segment.RealtimeUpdate, gameObject);
            }
        }

        public void activateWarp()
        {
            Timing.RunCoroutine(sceneTransitionCutscene(), Segment.RealtimeUpdate);
        }

        IEnumerator<float> sceneTransitionCutscene()
        {
            GameManager.Instance.disableInput();
            GameManager.Instance.cutsceneMode = true;

            if (vCam)
            {
                vCam.m_Priority = 999;
                vCam.gameObject.SetActive(true);
                vCam.m_Lens.FieldOfView = GameManager.Instance.config.desiredFOV;
            }

            if (moveTarget)
            {

                // mPlayer.changeCurrentAction(PlayerActionState.Cutscene);
                
                if(mPlayer.currentMode != PlayerModes.NORMAL || mPlayer.currentMode != PlayerModes.SWIMMING)
                    mPlayer.changeCurrentMode(PlayerModes.NORMAL);

                CoroutineHandle moveAction = Timing.RunCoroutine(movePlayerToTarget(moveTarget.transform.position));

                if (waitForMove)
                {
                    while (moveAction.IsRunning)
                    {
                        yield return 0f;
                    }
                }
            }

            if(!waitForMove)
                yield return Timing.WaitForSeconds(timer);

            PlayerManager.Instance.lastExit = exitNumber;

            while(Time.timeScale > 0.01f)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.0f, 0.5f);
                yield return 0f;
            }
            Time.timeScale = 0.0f;


            GameManager.Instance.loadScene(targetScene,whiteFade);
        }

        IEnumerator<float> movePlayerToTarget(Vector3 targetPos)
        {
            float startAccel = Mathf.Clamp(mPlayer.acceleration.z, 3.0f, 16.0f);



            Vector3 moveDir = Helper.getHorizontalDirectionTo(mPlayer.transform.position, targetPos);
            moveDir.Normalize();

            mPlayer.setAngleInstantly(moveDir);
            mPlayer.acceleration.z = startAccel;

            float dst = Vector3.Distance(Helper.zeroAltitude(mPlayer.transform.position), Helper.zeroAltitude(targetPos));

            while(dst > 1.0f)
            {
                mPlayer.setAngleInstantly(moveDir);
                mPlayer.acceleration.z = startAccel;

                dst = Vector3.Distance(Helper.zeroAltitude(mPlayer.transform.position), Helper.zeroAltitude(targetPos));
                yield return 0f;
            }
        }

        private void OnValidate()
        {
            name = "LoadingZone_" + targetScene + "_exit" + exitNumber;

        }
    }

}
