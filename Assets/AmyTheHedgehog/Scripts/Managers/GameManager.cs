using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
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

        public GameConfig config;
        public SystemData systemData;

        bool[] analogStickState;
        bool[] analogStickFirstFrame;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoadRuntimeMethod()
        {
            GameManager.Instance.Init();
           // UIManager.Instance.Init();
            PlayerManager.Instance.Init();
           //TimeManager.Instance.Init();
           // MusicManager.Instance.Init();

            //GameManager.Instance.loadTitleScreen();
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

            systemSoundSource = gameObject.AddComponent<AudioSource>();
            systemSoundSource.spatialBlend = 0.0f;
            systemSoundSource.volume = 0.75f;
            systemSoundSource.playOnAwake = false;
            systemSoundSource.outputAudioMixerGroup = systemData.AUDIO_MenuSFXMixer.outputAudioMixerGroup;
            */

            analogStickState = new bool[8];
            analogStickFirstFrame = new bool[8];
            

            //loadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        public Camera spawnMainCamera()
        {
            
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

    }

}
