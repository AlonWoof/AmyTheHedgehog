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
        protected bool canSkipScene = false;

        public string hashOverride;
        public bool useStoryFlag = true;

        public Transform playerLocationMarker;

        // Update is called once per frame
        void Update()
    	{
            if (mPlayer == null)
                getPlayerInstance();

            if (cutsceneThread.IsValid)
            {
                if (cutsceneThread.IsRunning)
                {
                    handleInput();
                }
            }
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
                if (PlayerManager.Instance.getStoryFlag(getStoryHash()))
                {
                    Destroy(gameObject);
                    return;
                }
            }


            GameManager.Instance.cutsceneMode = true;
            sceneInProgress = true;
            cutsceneThread = Timing.RunCoroutine(doCutscene().CancelWith(gameObject));
            onStartScene.Invoke();
        }

        public virtual void endCutscene()
        {
            if (useStoryFlag)
                addStoryFlag();

            GameManager.Instance.cutsceneMode = false;
            sceneInProgress = false;
            onEndScene.Invoke();
            Timing.KillCoroutines(cutsceneThread);
        }

        protected void enableSkip()
        {
            canSkipScene = true;
        }

        protected void disableSkip()
        {
            canSkipScene = false;
        }

        protected virtual void skipCutscene()
        {
            endCutscene();
        }

        public void handleInput()
        {
            if(Input.GetButtonDown("Pause") && canSkipScene)
            {
                skipCutscene();
            }
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
            PlayerManager.Instance.setStoryFlag(getStoryHash(), true);
           // PlayerManager.Instance.saveGame.setCutsceneFlag(getStoryHash(),true);
        }
	}

}
