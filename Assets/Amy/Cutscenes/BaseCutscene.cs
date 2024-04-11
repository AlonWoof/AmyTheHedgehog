using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MEC;
using UnityEngine.SceneManagement;

/* Copyright 2024 Jennifer Haden */
namespace Amy
{

	public class BaseCutscene : MonoBehaviour
	{
        protected Player mPlayer;


        public UnityEvent onStartScene;
        public UnityEvent[] subEvents;
        public UnityEvent onEndScene;

        public CoroutineHandle cutsceneThread;

        protected bool prevPlayerLockState;
        protected bool prevCameraLockState;

        protected bool sceneInProgress = false;
        protected bool sceneSkipped = false;

        public string hashOverride;
        public bool useStoryFlag = true;

        public Transform playerLocationMarker;

        // Update is called once per frame
        void Update()
    	{
            if (mPlayer == null)
                getPlayerInstance();
        }


        Player getPlayerInstance()
        {
            if(mPlayer == null)
                mPlayer = PlayerManager.Instance.getPlayer();

            return mPlayer;
        }

        public void playCutscene()
        {
            if(useStoryFlag)
            {
                //Do not see it again.
              //  if (PlayerManager.Instance.saveGame.getCutsceneFlag(getStoryHash()))
               //     return;
            }


            GameManager.Instance.cutsceneMode = true;
            sceneInProgress = true;
            cutsceneThread = Timing.RunCoroutine(doCutscene());
            onStartScene.Invoke();
        }

        public void endCutscene()
        {
            if (useStoryFlag)
                addStoryFlag();

            GameManager.Instance.cutsceneMode = false;
            sceneInProgress = false;
            onEndScene.Invoke();
            Timing.KillCoroutines(cutsceneThread);
        }

        public void skipCutscene()
        {
            sceneSkipped = true;
        }

        protected virtual IEnumerator<float> doCutscene()
        {
            //Placeholder stuff
            yield return 0f;
        }

        public int getStoryHash()
        {
            string toHash = gameObject.name + SceneManager.GetActiveScene().name + transform.position.y;

            if (hashOverride.Length > 3)
                toHash = hashOverride;

            return Animator.StringToHash(toHash);
        }

        public void addStoryFlag()
        {
            
           // PlayerManager.Instance.saveGame.setCutsceneFlag(getStoryHash(),true);
        }
	}

}
