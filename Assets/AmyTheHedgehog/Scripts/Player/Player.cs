using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace Amy
{

    public enum PlayerModes
    {
        GROUNDMOVE,
        HANGING
    }



    public class Player : MonoBehaviour
	{


        public float rotationSpeed = 10.0f;

        Animator mAnimator;
        CapsuleCollider mCollider;
        Rigidbody mRigidBody;

        //This vector must always be normalized.
        public Vector3 mDirection = Vector3.forward;

        public Vector3 mCurrentMovement = Vector3.zero;
        public Vector3 mDesiredMovement = Vector3.zero;

        public Vector3 groundNormal;
        public bool isGrounded;

        public LayerMask mColMask;

        public float turnRatio = 0.0f;
        public float stealthIndex = 0.0f;

        //Modes
        public PlayerModes currentMode;
        public PlayerModes lastMode;

        PlayerGroundMove modeGroundMove;
        PlayerHanging modeHanging;

        public Transform hipBoneTransform;
        public Transform headBoneTransform;

        void readPositionFromFile()
        {
            string dataPath = Application.persistentDataPath + "\\SADX_DATA.dat";
            Debug.Log(dataPath);

            TextAsset textAsset = Resources.Load(dataPath) as TextAsset;

            FileStream file = File.OpenRead(dataPath);

            BinaryReader reader = new BinaryReader(file);

            reader.ReadInt16();
            reader.ReadInt32();
            Vector3 pos = Vector3.zero;
            

            pos.x = -reader.ReadSingle();
            pos.y = reader.ReadSingle();
            pos.z = reader.ReadSingle();

            float ang = ((360.0f / 65535.0f) * reader.ReadInt32());


            reader.Close();

            transform.position = pos * 0.1f;
            transform.rotation = Quaternion.Euler(0, ang - 90.0f, 0);
        }

        private void Awake()
        {
            mColMask = LayerMask.GetMask("Collision");
            
        }

        public static Player Spawn(Vector3 pos, Vector3 dir)
        {
            float amy_height = 0.9f;

            //Fandom wiki says 25 kg, official sources cheekily say ヒ・ミ・ツ！ ("it's a secret!")
            //So let's assume 25kg, seems about right.
            float amy_weight = 25.0f;

            GameObject inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyIngameModel);
            inst.transform.position = pos;


            CapsuleCollider col = inst.AddComponent<CapsuleCollider>();
            col.radius = 0.25f;
            col.height = amy_height * 0.85f;
            col.center = (Vector3.up * amy_height) * 0.5f;


            // A super slippery physic material for platformer gameplay
            PhysicMaterial playerMat = new PhysicMaterial();
            playerMat.frictionCombine = PhysicMaterialCombine.Minimum;
            playerMat.dynamicFriction = 0;
            playerMat.staticFriction = 0;
            playerMat.bounciness = 0;
            playerMat.bounceCombine = PhysicMaterialCombine.Minimum;

            col.material = playerMat;

            Rigidbody body = inst.AddComponent<Rigidbody>();
            body.mass = amy_weight;

            body.useGravity = false;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            //Animator anim = inst.AddComponent<Animator>();
            //anim.runtimeAnimatorController = GameManager.Instance.systemData.RES_AmyIngameAnimator;

            Player newPlayer = inst.AddComponent<Player>();

            GameObject camInst = new GameObject("ThirdPersonCamera");
            ThirdPersonCamera tpc = camInst.AddComponent<ThirdPersonCamera>();

            tpc.setPlayerTransform(inst.transform);

            
            newPlayer.mDirection = dir;

            return newPlayer;
        }

        void getPlayerBones()
        {
            hipBoneTransform = getBoneByName("hips");
            headBoneTransform = getBoneByName("head");

        }

        Transform getBoneByName(string name)
        {
            foreach(Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.gameObject.name.ToLower() == name.ToLower())
                    return t;
            }

            return null;
        }

        void getBaseComponents()
        {
            mAnimator = GetComponent<Animator>();
            mCollider = GetComponent<CapsuleCollider>();
            mRigidBody = GetComponent<Rigidbody>();

            //Add a dummy animator to prevent problems later.
            if (!mAnimator)
                mAnimator = gameObject.AddComponent<Animator>();
        }

        void addAllModes()
        {
            modeGroundMove = gameObject.AddComponent<PlayerGroundMove>();
            modeHanging = gameObject.AddComponent<PlayerHanging>();
        }

        void disableAllModes()
        {
            modeGroundMove.enabled = false;
            modeHanging.enabled = false;
        }

    	// Start is called before the first frame update
    	void Start()
    	{
            getBaseComponents();
            getPlayerBones();
            addAllModes();
            disableAllModes();

            updateCurrentMode();
        }

    	// Update is called once per frame
    	void Update()
    	{
            Debug.DrawLine(transform.position, transform.position + groundNormal, Color.red, 1.0f);

            if (Input.GetKeyDown(KeyCode.F4))
                changeCurrentMode(PlayerModes.HANGING);

            if (Input.GetKeyDown(KeyCode.F3))
                changeCurrentMode(PlayerModes.GROUNDMOVE);

            
        }

        private void LateUpdate()
        {
            //readPositionFromFile();
        }


        void updateDirection()
        {



        }


        private void FixedUpdate()
        {
            updateDirection();
        }



        void updateCurrentMode()
        {
            disableAllModes();

            switch(currentMode)
            {
                case PlayerModes.GROUNDMOVE:
                    modeGroundMove.enabled = true;
                    break;

                case PlayerModes.HANGING:
                    modeHanging.enabled = true;
                    break;

                default:
                    break;
            }
        }

        public void changeCurrentMode(PlayerModes newMode)
        {
            if (currentMode == newMode)
                return;

            lastMode = currentMode;
            currentMode = newMode;

            updateCurrentMode();
        }


    }

}
