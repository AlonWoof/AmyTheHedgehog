using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/* Copyright 2022 Jennifer Haden */
namespace Amy
{

    public static class SystemColors
    {
        public static Color AmyColor = new Color(0.9647059f, 0.6392157f, 0.7333333f);
        public static Color CreamColor = new Color(0.972549f, 0.8784314f, 0.7215686f);
        public static Color YumeColor = new Color(0.9294118f, 0.8666667f, 0.427451f);

        public static Color choiceHighlightColor = new Color(1.0f, 1.0f, 1.0f);
        public static Color choiceDefaultColor = new Color(0.6f, 0.6f, 0.6f);
        public static Color choiceInactiveColor = new Color();
    }

    [System.Serializable]
    public class GameConfig
    {

        public float desiredFOV = 60.0f;
        public float lookSensitivity = 1.0f;
        public bool pitchInvert = false;
        public bool yawInvert = false;
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
        public bool gamePaused = false;

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


            //Override for the sake of my Steam Deck for now
            Application.targetFrameRate = 61;

            Time.fixedDeltaTime = 1.0f / ((float)Application.targetFrameRate);
            Time.maximumDeltaTime = Time.fixedDeltaTime * 0.5f;
            QualitySettings.vSyncCount = 0;


            Debug.Log("Target Refresh Rate: " + Application.targetFrameRate);
            Debug.Log("Fixed Timestep: " + Time.fixedDeltaTime);

        }

        public static SystemData LoadSystemDataStandalone()
        {
            return Resources.Load("SystemData") as SystemData;
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

            #if !UNITY_EDITOR
            PlayerManager.Instance.saveFileSlot = -1;
            loadScene("Init");
            #endif

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

        void resetGameState()
        {
            gamePaused = false;
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
            checkPauseGame();

            
            systemData.AUDIO_GameSFXMixer.SetFloat("GameSFXVolume",  gameSFXVolume);
        }

        public void pauseGame()
        {
            Time.timeScale = 0.0f;
            mainCamera.GetComponent<Cinemachine.CinemachineBrain>().m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.LateUpdate;
            gamePaused = true;
        }

        public void unPauseGame()
        {
            Time.timeScale = 1.0f;
            mainCamera.GetComponent<Cinemachine.CinemachineBrain>().m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate;
            gamePaused = false;
        }

        public void enablePlayerInput()
        {
            playerInputDisabled = false;
        }

        public void disablePlayerInput()
        {
            playerInputDisabled = true;
        }

        public void enableCameraInput()
        {
            cameraInputDisabled = false;
        }

        public void disableCameraInput()
        {
            cameraInputDisabled = true;
        }

        public void disableInput()
        {
            disablePlayerInput();
            disableCameraInput();
        }

        public void enableInput()
        {
            enablePlayerInput();
            enableCameraInput();
        }


        void checkPauseGame()
        {

            if (!canPauseGame())
                return;

            if (!Input.GetButtonDown("Pause"))
                return;

            if(!gamePaused)
            {
                pauseGame();
            }
            else
            {
                unPauseGame();
            }
        }

        bool canPauseGame()
        {
            if (cutsceneMode)
                return false;

            if (isLoading)
                return false;

            if (cameraInputDisabled || playerInputDisabled)
                return false;

            if(!PlayerManager.Instance.mPlayerInstance)
                return false;

            if (PlayerManager.Instance.itemMenuOpen)
                return false;

            return true;
        }

        void debugFunctions()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                loadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

            //Go anywheres
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Timing.KillCoroutines();
                SceneManager.LoadScene("MapSelect");
            }

            //F3 reserved for DebugInfo pages

            //Switch girls.
            if (Input.GetKeyDown(KeyCode.F4))
            {
                if(PlayerManager.Instance.currentCharacter == PlayableCharacter.Amy)
                    PlayerManager.Instance.characterSwitch(PlayableCharacter.Cream);
                else if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Cream)
                    PlayerManager.Instance.characterSwitch(PlayableCharacter.YoungAmy);
                else
                    PlayerManager.Instance.characterSwitch(PlayableCharacter.Amy);
            }

            //Insta-period lol
            if (Input.GetKeyDown(KeyCode.F5))
            {
                PlayerManager.Instance.daysTilMenstruation = 0;
            }

            //Randomize Day Events
            if (Input.GetKeyDown(KeyCode.F6))
            {
                PlayerManager.Instance.randomizeDayEvents();
            }

            //How fucking brutal
            if (Input.GetKey(KeyCode.F7))
                PlayerManager.Instance.killHer();


            //Heals
            if (Input.GetKey(KeyCode.F8))
            {
                PlayerStatus pstats = PlayerManager.Instance.getCurrentPlayerStatus();
                pstats.currentHealth = pstats.maxHealth;
                pstats.currentStamina = pstats.maxStamina;
            }

            //ALL the things
            if (Input.GetKeyDown(KeyCode.F9))
            {
                foreach(ItemData i in systemData.itemData)
                {
                    PlayerManager.Instance.mPlayerInstance.getStatus().addItem(i.getHash());
                }
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

        public void controllerRumble(float time, float power_left, float power_right)
        {
            Timing.RunCoroutine(doControllerRumble(time, power_left, power_right).CancelWith(gameObject), Segment.RealtimeUpdate);
        }

        IEnumerator<float> doControllerRumble(float time, float power_left, float power_right)
        {
            float maxTime = time;

            if(Gamepad.current != null)
                Gamepad.current.SetMotorSpeeds(power_left, power_right);

            while(time > 0.0f)
            {
                float fac = time / maxTime;
                float pl = Mathf.Lerp(power_left, 0.0f, fac);
                float pr = Mathf.Lerp(power_right, 0.0f, fac);

                if (Gamepad.current != null)
                    Gamepad.current.SetMotorSpeeds(pl, pr);

                Debug.Log("VIB LEFT: " + pl + " RIGHT: " + pr);

                time -= Time.unscaledDeltaTime;
                yield return 0f;
            }

            if (Gamepad.current != null)
                Gamepad.current.SetMotorSpeeds(0.0f, 0.0f);

            yield return 0f;
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
            PlayerManager.Instance.saveFileSlot = -1;
            loadScene("Title");
        }

        public void loadScene(string sceneName, bool whiteFade = false, float delayBeforeLoading = 0.0f)
        {
            //Special exceptions...

            if(PlayerManager.Instance.todayEvents.stationCircleNight)
            {
                if (sceneName.ToLower() == "city")
                    sceneName = "City_Night";
            }

            if(PlayerManager.Instance.isBadDay())
            {
                int rng = Random.Range(0, 100);

                if(rng % 16 == 0)
                {
                    sceneName = "NULL";
                }
            }
            

            Timing.RunCoroutine(loadSceneRoutine(sceneName, whiteFade, delayBeforeLoading), Segment.RealtimeUpdate);
        }


        public static SystemData getSystemData()
        {
            return GameManager.Instance.systemData;
        }

        IEnumerator<float> loadSceneRoutine(string sceneName, bool whiteFade, float delay = 0.0f)
        {
            yield return Timing.WaitForSeconds(delay);

            gamePaused = false;
            PlayerManager.Instance.itemMenuOpen = false;
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

            //Night chance?
            int rng = Random.Range(0, 64);

            if (rng == 13)
                PlayerManager.Instance.isNightTime = true;
            else
                PlayerManager.Instance.isNightTime = false;

            AsyncOperation load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

            if (load == null)
            {
                load = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("default");
            }


            while (!load.isDone)
            {
                
                yield return 0f;
            }

            Time.timeScale = 1.0f;

            //preloadAssets();

            //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

            foreach(ReflectionProbe r in FindObjectsOfType<ReflectionProbe>())
            {
                if(r.mode == UnityEngine.Rendering.ReflectionProbeMode.Realtime)
                {
                    r.RenderProbe();
                }
            }

            bool playerShouldSpawn = true;
            PlayerManager.Instance.isSmallRoom = false;
            PlayerManager.Instance.isHubWorld = false;
            //PlayerManager.Instance.decidePlayerNPCLocation();

            SceneInfo scn = FindObjectOfType<SceneInfo>();

            if (scn != null)
            {
                MusicManager.Instance.changeSongs(scn.bgmData);

                if (scn.dontSpawnPlayer)
                    playerShouldSpawn = false;

                //PlayerManager.Instance.isOutdoors = scn.isOutdoors;

                if(scn.isSmallRoom)
                    PlayerManager.Instance.isSmallRoom = true;

                if (scn.isHubWorld)
                    PlayerManager.Instance.isHubWorld = true;

            }
            else
            {
                GameObject inst = new GameObject("MISSING SCENE INFO");
                scn = inst.AddComponent<SceneInfo>();
                playerShouldSpawn = false;
            }

            //Do with a fancy coroutine cutscene of rings being deposited.
            if (PlayerManager.Instance.isHubWorld && PlayerManager.Instance.getRings() > 0)
            {
                GameObject inst = GameObject.Instantiate(systemData.RES_RingBankTransferScene);

                BankTransferScene bankTransfer = inst.GetComponentInChildren<BankTransferScene>();

                CoroutineHandle ch = bankTransfer.startRingBankTransfer();

                while(ch.IsRunning)
                {
                    yield return 0f;
                }

                Destroy(inst);
            }

            

            float waitTime = 0.1f;

            if (PlayerManager.Instance.isBadDay())
            {
                rng = Random.Range(0, 100);

                //IT'S HIP TO FUCK TEXTURES
                if (rng % 4 == 0)
                {
                    GameObject fucker = new GameObject("Fucker");
                    fucker.AddComponent<TexlistFucker>();
                }

            }

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

            //Auto-save
            SaveGame.writeSaveGame(PlayerManager.Instance.saveFileSlot);

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

        public void findSceneInfo()
        {
            SceneInfo scn = FindObjectOfType<SceneInfo>();
            if (scn != null)
            {
                //MusicManager.Instance.changeSongs(scn.bgmData);

                if (scn.isSmallRoom)
                    PlayerManager.Instance.isSmallRoom = true;

                if (scn.isHubWorld)
                    PlayerManager.Instance.isHubWorld = true;
            }
            else
            {
                GameObject inst = new GameObject("MISSING SCENE INFO");
                scn = inst.AddComponent<SceneInfo>();
            }

        }

        #endregion

    }



}
