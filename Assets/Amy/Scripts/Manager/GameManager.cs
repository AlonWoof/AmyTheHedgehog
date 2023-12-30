using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/* Copyright 2022 Jason Haden */
namespace Amy
{



    [System.Serializable]
    public class GameConfig
    {

        public float desiredFOV = 60.0f;
        public float lookSensitivity = 1.0f;
        public bool pitchInvert = false;
    }

    public enum AnalogStickDirection
    {
        leftStick_Up,
        leftStick_Down,
        leftStick_Right,
        leftStick_Left,

        rightStick_Up,
        rightStick_Down,
        rightStick_Right,
        rightStick_Left
    }


    public class GameManager : Singleton<GameManager>
	{

        public Camera mainCamera;


        public bool usingController = true;
        public bool cameraInputDisabled = false;
        public bool playerInputDisabled = false;

        public bool isLoading = false;
        public bool cutsceneMode = false;

        public GameConfig config;
        public SystemData systemData;

        bool[] analogStickState;
        bool[] analogStickFirstFrame;

        AudioSource bgmSource;
        AudioSource systemSoundSource;
        public float gameSFXVolume;

        public Cinemachine.CinemachineBlendDefinition blend_instant;
        public Cinemachine.CinemachineBlendDefinition blend_fast;
        public Cinemachine.CinemachineBlendDefinition blend_slow;

        public enum blend_mode
        {
            instant,
            fast,
            slow
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoadRuntimeMethod()
        {
            GameManager.Instance.Init();
            UIManager.Instance.Init();
            PlayerManager.Instance.Init();
           // EnemyManager.Instance.Init();
           //TimeManager.Instance.Init();
            MusicManager.Instance.Init();

            //GameManager.Instance.loadTitleScreen();

            Application.targetFrameRate = Screen.currentResolution.refreshRate * 2;
            QualitySettings.vSyncCount = 0;
        }

        // Start is called before the first frame update
        void Awake()
        {
            Debug.Log("Loading System Data...");
            systemData = Resources.Load("SystemData") as SystemData;
            //systemAudio = gameObject.AddComponent<AudioSource>();
            config = new GameConfig();

            
            mainCamera = spawnMainCamera();

            /*
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.spatialBlend = 0.0f;
            bgmSource.volume = 0.25f;
            bgmSource.playOnAwake = false;
            */

            Application.targetFrameRate = 60;

            systemSoundSource = gameObject.AddComponent<AudioSource>();
            systemSoundSource.spatialBlend = 0.0f;
            systemSoundSource.volume = 0.75f;
            systemSoundSource.playOnAwake = false;
            systemSoundSource.outputAudioMixerGroup = systemData.AUDIO_MenuSFXMixer.outputAudioMixerGroup;
            

            analogStickState = new bool[8];
            analogStickFirstFrame = new bool[8];
            

            //loadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public Camera spawnMainCamera()
        {

            blend_instant.m_Time = 0.0f;


            //Clear duplicates
            if (FindObjectOfType<Cinemachine.CinemachineBrain>())
            {
                Destroy(FindObjectOfType<Cinemachine.CinemachineBrain>().gameObject);
            }
            

            GameObject inst = GameObject.Instantiate(systemData.RES_mainCamera);
            DontDestroyOnLoad(inst);

            inst.tag = "MainCamera";

            return inst.GetComponent<Camera>();
        }

        

        public void changeCameraBlendMode(blend_mode mode)
        {

            Cinemachine.CinemachineBrain mCam = mainCamera.GetComponentInChildren<Cinemachine.CinemachineBrain>();

            switch (mode)
            {
                case blend_mode.instant:
                    mCam.m_DefaultBlend.m_Style = Cinemachine.CinemachineBlendDefinition.Style.Cut;
                    mCam.m_DefaultBlend.m_Time = 0.0f;
                    break;
                case blend_mode.fast:
                    mCam.m_DefaultBlend.m_Style = Cinemachine.CinemachineBlendDefinition.Style.EaseInOut;
                    mCam.m_DefaultBlend.m_Time = 0.25f;
                    break;
                case blend_mode.slow:
                    mCam.m_DefaultBlend.m_Style = Cinemachine.CinemachineBlendDefinition.Style.EaseInOut;
                    mCam.m_DefaultBlend.m_Time = 1.0f;
                    break;
            }
        }

        public void resetCameraBlendMode()
        {
            changeCameraBlendMode(blend_mode.instant);
        }

        public void Init()
        {
            Debug.Log("GameManager Initialized!");
        }

        // Start is called before the first frame update
        void Start()
    	{
    	    
    	}

    	// Update is called once per frame
    	void Update()
    	{
            checkController();
            debugFunctions();

            systemData.AUDIO_GameSFXMixer.SetFloat("GameSFXVolume", gameSFXVolume);
        }

        void debugFunctions()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                loadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            if (Input.GetKeyDown(KeyCode.F3))
            {
               // PlayerManager.Instance.loadSavedGame(0, true);
               // PlayerManager.Instance.reloadCurrentSave();
            }

            //ZA WARUDO!
            if (Input.GetKey(KeyCode.F5))
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0.01f, Time.unscaledDeltaTime * 3.0f);

            //Toki wo ugoki dasu
            if (Input.GetKey(KeyCode.F6))
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1.0f, Time.unscaledDeltaTime * 3.0f);


            if (Input.GetKeyDown(KeyCode.F2))
            {
                Timing.KillCoroutines();
                SceneManager.LoadScene("MapSelect");
            }

            //Emergency exit key
            if (Input.GetButton("RightBumper") && Input.GetButton("LeftBumper") && Input.GetButtonDown("Action"))
            {
                Timing.KillCoroutines();
                SceneManager.LoadScene("MapSelect");
            }

            //if(Input.GetButtonDown())

        }
        void checkController()
        {

            if (usingController)
            {
                if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f
                    || Mathf.Abs(Input.GetAxis("Mouse X")) > 0.15f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.15f)
                {
                    usingController = false;
                }
            }
            else
            {
                if (Mathf.Abs(Input.GetAxis("Left Analog X")) > 0.1f || Mathf.Abs(Input.GetAxis("Left Analog Y")) > 0.1f
                    || Mathf.Abs(Input.GetAxis("Right Analog X")) > 0.1f || Mathf.Abs(Input.GetAxis("Right Analog Y")) > 0.1f)
                {
                    usingController = true;
                }

            }

            setAnalogPressedFlags();
        }

        void setAnalogPressedFlags()
        {
            float h = Input.GetAxisRaw("Left Analog X");
            float v = Input.GetAxisRaw("Left Analog Y");

            float deadZone = 0.5f;

            for (int i = 0; i < 7; i++)
            {
                analogStickFirstFrame[i] = false;
            }


            if (h > deadZone && !analogStickState[(int)AnalogStickDirection.leftStick_Right])
            {
                analogStickState[(int)AnalogStickDirection.leftStick_Right] = true;
                analogStickFirstFrame[(int)AnalogStickDirection.leftStick_Right] = true;
            }

            if (h < deadZone)
                analogStickState[(int)AnalogStickDirection.leftStick_Right] = false;

            if (h < -deadZone && !analogStickState[(int)AnalogStickDirection.leftStick_Left])
            {
                analogStickState[(int)AnalogStickDirection.leftStick_Left] = true;
                analogStickFirstFrame[(int)AnalogStickDirection.leftStick_Left] = true;
            }

            if (h > -deadZone)
                analogStickState[(int)AnalogStickDirection.leftStick_Left] = false;

            if (v < -deadZone && !analogStickState[(int)AnalogStickDirection.leftStick_Down])
            {
                analogStickState[(int)AnalogStickDirection.leftStick_Down] = true;
                analogStickFirstFrame[(int)AnalogStickDirection.leftStick_Down] = true;
            }

            if (v > -deadZone)
                analogStickState[(int)AnalogStickDirection.leftStick_Down] = false;

            if (v > deadZone && !analogStickState[(int)AnalogStickDirection.leftStick_Up])
            {
                analogStickState[(int)AnalogStickDirection.leftStick_Up] = true;
                analogStickFirstFrame[(int)AnalogStickDirection.leftStick_Up] = true;
            }

            if (v < deadZone)
                analogStickState[(int)AnalogStickDirection.leftStick_Up] = false;
        }


        public bool isAnalogDown(AnalogStickDirection dir)
        {
            // Debug.Log("State " + (int)dir + " is " + analogStickFirstFrame[(int)dir]);
            return analogStickFirstFrame[(int)dir];

        }

        public void playSystemSound(AudioClip snd, float volume = 1.0f)
        {
            systemSoundSource.volume = volume;
            systemSoundSource.PlayOneShot(snd);
        }

        #region Scene Transition

        public void loadTitleScreen()
        {
            loadScene("Title");
        }

        public void loadScene(string sceneName, bool whiteFade = false, float delayBeforeLoading = 0.0f)
        {
            Timing.RunCoroutine(loadSceneRoutine(sceneName, whiteFade, delayBeforeLoading), Segment.RealtimeUpdate);
        }

        public static SystemData getSystemData()
        {
            return GameManager.Instance.systemData;
        }

        IEnumerator<float> loadSceneRoutine(string sceneName, bool whiteFade, float delay = 0.0f)
        {
            yield return Timing.WaitForSeconds(delay);

            playerInputDisabled = true;
            cameraInputDisabled = true;
            isLoading = true;
            cutsceneMode = true;
            Time.timeScale = 1.0f;


            //Not relevant yet.
            /*
            if (UIManager.Instance.gamePaused)
            {

                UIManager.Instance.mIngameMenu.gameObject.SetActive(false);
                UIManager.Instance.gamePaused = false;
            }
            */

            UIManager.Instance.fadeScreen(false, 0.75f, whiteFade);


            while (gameSFXVolume > -80.0f)
            {
                gameSFXVolume = Mathf.Lerp(gameSFXVolume, -81.0f, Time.unscaledDeltaTime * 3);

                yield return 0f;
            }

            gameSFXVolume = -80.0f;



            yield return Timing.WaitForSeconds(0.5f);

            //UIManager.Instance.hideGameOverScreen();


            AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

            if (load == null)
            {
                load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("default");
            }


            while (!load.isDone)
            {
                yield return 0f;
            }

            //preloadAssets();

            //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));


            bool playerShouldSpawn = true;

            SceneInfo scn = FindObjectOfType<SceneInfo>();

            if (scn != null)
            {
                MusicManager.Instance.changeSongs(scn.bgmData);

                if (scn.dontSpawnPlayer)
                    playerShouldSpawn = false;

                //PlayerManager.Instance.isOutdoors = scn.isOutdoors;



            }
            else
            {
                GameObject inst = new GameObject("MISSING SCENE INFO");
                scn = inst.AddComponent<SceneInfo>();
                playerShouldSpawn = false;
            }

            float waitTime = 0.1f;

            if (playerShouldSpawn)
            {
                PlayerManager.Instance.spawnPlayerAtExit();
                waitTime = 1.0f;
            }


            yield return Timing.WaitForSeconds(waitTime);

            
            while (gameSFXVolume < 0.0f)
            {
                gameSFXVolume = Mathf.Lerp(gameSFXVolume, 0.1f, Time.unscaledDeltaTime * 3);

                yield return 0f;
            }

            gameSFXVolume = 0.0f;

            if (PlayerManager.Instance.mPlayerInstance)
                PlayerManager.Instance.mPlayerInstance.tpc.centerBehindPlayer();

            

            yield return Timing.WaitForSeconds(waitTime * 0.5f);

            if (PlayerManager.Instance.mPlayerInstance)
                PlayerManager.Instance.mPlayerInstance.changeCurrentMode(PlayerModes.NORMAL);


           // EnemyManager.Instance.currentEnemyPhase = ENEMY_PHASE.PHASE_SNEAK;

            UIManager.Instance.fadeScreen(true, 0.75f);
            yield return Timing.WaitForSeconds(0.75f);

            if (scn.showTitleCard)
            {

               // if (PlayerManager.Instance.saveGame.getCutsceneFlag(scn.titleCardStoryFlagHash) || scn.titleCardStoryFlagHash == -1)
               //     FindObjectOfType<TitleCard>().showTitleCard(scn.areaName, 2, 0.5f);
            }

            playerInputDisabled = false;
            cameraInputDisabled = false;
            cutsceneMode = false;

            isLoading = false;
        }



        #endregion

    }

}
