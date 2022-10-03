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
        BASIC_MOVE,
        HANGING
    }



    public class Player : MonoBehaviour
	{

        Animator mAnimator;
        CapsuleCollider mCollider;
        Rigidbody mRigidBody;

        BallModeModel mBallModel;

        const float turningSpeed = 10.0f;

        //This vector must always be normalized.
        public Vector3 mDirection = Vector3.forward;

        public float mForwardVelocity = 0.0f;
        public Vector3 mCurrentMovement = Vector3.zero;
        public Vector3 mDesiredMovement = Vector3.zero;

        public Vector3 groundNormal;
        public bool isGrounded;

        public LayerMask mColMask;

        //Modes
        public PlayerModes currentMode;
        public PlayerModes lastMode;

        PlayerBasicMove modeBasicMove;
        PlayerHanging modeHanging;

        public Transform hipBoneTransform;
        public Transform headBoneTransform;

        public bool isBallMode = false;

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
            mBallModel = GetComponent<BallModeModel>();


            //Add a dummy animator to prevent problems later.
            if (!mAnimator)
                mAnimator = gameObject.AddComponent<Animator>();
        }

        void addAllModes()
        {
            modeBasicMove = gameObject.AddComponent<PlayerBasicMove>();
            modeHanging = gameObject.AddComponent<PlayerHanging>();
        }

        void disableAllModes()
        {
            modeBasicMove.enabled = false;
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
                changeCurrentMode(PlayerModes.BASIC_MOVE);

            if(Vector3.Angle(Vector3.up,groundNormal) < 60.0f)
                transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.FromToRotation(Vector3.up, groundNormal) * Quaternion.LookRotation(mDirection, Vector3.up), Time.deltaTime * turningSpeed);
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mDirection, Vector3.up), Time.deltaTime * turningSpeed);


            if(mBallModel)
            {
                mBallModel.isBallMode = isBallMode;
            }
        }

        private void LateUpdate()
        {

        }

        private void FixedUpdate()
        {
        }



        void updateCurrentMode()
        {
            disableAllModes();

            switch(currentMode)
            {
                case PlayerModes.BASIC_MOVE:
                    modeBasicMove.enabled = true;
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
