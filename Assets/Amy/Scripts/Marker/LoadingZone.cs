using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using Cinemachine;

/* Copyright 2021 Jennifer Haden */
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
            if(other.GetComponent<Player>())
            {
                Timing.RunCoroutine(sceneTransitionCutscene());
            }
        }

        IEnumerator<float> sceneTransitionCutscene()
        {
            GameManager.Instance.cameraInputDisabled = true;
            GameManager.Instance.playerInputDisabled = true;
            GameManager.Instance.cutsceneMode = true;

            if (vCam)
            {
                vCam.m_Priority = 999;
                vCam.gameObject.SetActive(true);
            }



            if (moveTarget)
            {

               // mPlayer.changeCurrentAction(PlayerActionState.Cutscene);

               // CoroutineHandle moveAction = mPlayer.GetComponent<PlayerCutscene>().moveToPoint(moveTarget.transform.position, 0.2f, 1.0f, true);

                if (waitForMove)
                {
                   // while (moveAction.IsRunning)
                  //  {
                        yield return 0f;
                   // }
                }
            }

            if(!waitForMove)
                yield return Timing.WaitForSeconds(timer);

            PlayerManager.Instance.lastExit = exitNumber;
            GameManager.Instance.loadScene(targetScene,whiteFade);
        }
	}

}
