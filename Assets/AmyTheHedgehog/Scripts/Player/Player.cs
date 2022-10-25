using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace Amy
{
    [System.Serializable]
    public class AmyFXRes
    {
        public GameObject fx_basicJump;
    }

    public enum PlayerModes
    {
        BASIC_MOVE,
        SPRING,
        HANGING,
        SWIMMING,
        DEBUG_MOVE
    }

    public class Player : MonoBehaviour
	{

        Animator mAnimator;
        CapsuleCollider mCollider;
        Rigidbody mRigidBody;

        BallModeModel mBallModel;

        float turningSpeed = 10.0f;

        //Who are we?
        public PlayableCharacter mChara;

        //This vector must always be normalized.
        public Vector3 mDirection = Vector3.forward;

        public float mForwardVelocity = 0.0f;
        public Vector3 mCurrentMovement = Vector3.zero;
        public Vector3 mDesiredMovement = Vector3.zero;

        public float waterDepth = 0.0f;
        public float playerHeight = 0.9f;

        public Vector3 groundNormal;
        public bool isGrounded;

        public LayerMask mColMask;

        public ThirdPersonCamera tpc;

        //Modes
        public PlayerModes currentMode;
        public PlayerModes lastMode;


        PlayerBasicMove modeBasicMove;
        PlayerSpringBounce modeSpring;
        PlayerSwimming modeSwimming;
        PlayerHanging modeHanging;
        PlayerDebugMove modeDebugMove;

        public Transform hipBoneTransform;
        public Transform headBoneTransform;

        public bool isBallMode = false;
       

        private void Awake()
        {
            mColMask = LayerMask.GetMask("Collision");
            
        }

        public static Player Spawn(Vector3 pos, Vector3 dir, PlayableCharacter chara = PlayableCharacter.Amy)
        {
            float amy_height = 0.9f;

            if (chara == PlayableCharacter.Cream)
                amy_height = 0.7f;

            //Fandom wiki says 25 kg, official sources cheekily say ヒ・ミ・ツ！ ("it's a secret!")
            //So let's assume 25kg, seems about right.
            float amy_weight = 25.0f;

            GameObject inst;

            switch (chara)
            {
                case PlayableCharacter.Amy:
                    inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyIngameModel);
                    break;

                case PlayableCharacter.Cream:
                    inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_CreamIngameModel);
                    break;

                default:
                    inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyIngameModel);
                    break;
            }

            
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

            Animator anim = inst.GetComponent<Animator>();

            if(!anim)
                inst.AddComponent<Animator>();

            anim.runtimeAnimatorController = GameManager.Instance.systemData.RES_AmyIngameAnimator;

            CharacterPhysics jiggles = inst.AddComponent<CharacterPhysics>();

            if(chara == PlayableCharacter.Amy)
                jiggles.mData = GameManager.Instance.systemData.RES_AmyJiggleData;
            else
                jiggles.mData = GameManager.Instance.systemData.RES_CreamJiggleData;

            FootstepFX footsteps = inst.AddComponent<FootstepFX>();
            footsteps.isPlayer = true;

            Player newPlayer = inst.AddComponent<Player>();

            GameObject camInst = new GameObject("ThirdPersonCamera");
            ThirdPersonCamera tpc = camInst.AddComponent<ThirdPersonCamera>();

            tpc.setPlayerTransform(inst.transform);
            tpc.centerBehindPlayer();

            newPlayer.tpc = tpc;
            
            newPlayer.mDirection = dir;

            foreach (Transform t in inst.GetComponentsInChildren<Transform>())
            {
                if (t.gameObject.name.ToLower().Contains("hitbox"))
                {
                    PlayerHitbox hb = t.gameObject.AddComponent<PlayerHitbox>();
                    hb.mPlayer = newPlayer;
                }
            }

            newPlayer.mChara = chara;
            newPlayer.playerHeight = amy_height;

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
            modeSpring = gameObject.AddComponent<PlayerSpringBounce>();
            modeSwimming = gameObject.AddComponent<PlayerSwimming>();
            modeDebugMove = gameObject.AddComponent<PlayerDebugMove>();
        }

        void disableAllModes()
        {
            modeBasicMove.enabled = false;
            modeHanging.enabled = false;
            modeSpring.enabled = false;
            modeSwimming.enabled = false;
            modeDebugMove.enabled = false;
        }

        public float getWaterYPos()
        {
            //ITS UNDER NEGATIVE NINE THOUSAAAAND
            float water_y = -9001.0f;

            Vector3 start = transform.position + 128.0f * Vector3.up;
            Vector3 end = transform.position - 128.0f * Vector3.up;

            LayerMask waterCol = LayerMask.GetMask("Water");
            RaycastHit hitInfo = new RaycastHit();

            if(Physics.Linecast(start,end,out hitInfo, waterCol))
            {
                water_y = hitInfo.point.y;

            }

            return water_y;
        }

        public float getWaterDepth()
        {
            float water_y = getWaterYPos();

            if (water_y < transform.position.y)
                return 0f;

            return water_y - transform.position.y;
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
            //Debug.DrawLine(transform.position, transform.position + groundNormal, Color.red, 1.0f);

            if (currentMode == PlayerModes.BASIC_MOVE)
            {
                if (Vector3.Angle(Vector3.up, groundNormal) < 60.0f && mForwardVelocity > 0.1f)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, groundNormal) * Quaternion.LookRotation(mDirection, Vector3.up), Time.deltaTime * turningSpeed);
                else
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mDirection, Vector3.up), Time.deltaTime * turningSpeed);
            }

            if(currentMode == PlayerModes.SPRING)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(Vector3.up, groundNormal) * Quaternion.LookRotation(mDirection, Vector3.up), Time.deltaTime * turningSpeed);
            }

            if(mBallModel)
            {
                mBallModel.isBallMode = isBallMode;
            }

            debugControls();


        }

        private void LateUpdate()
        {
            // Debug.Log("Delta Timestep: " + Time.deltaTime);

        }

        private void FixedUpdate()
        {
            //Debug.Log("Fixed Timestep: " + Time.fixedDeltaTime);

        }

        public GameObject spawnFX(GameObject prefab, Vector3 position, bool parent = false)
        {
            GameObject inst = GameObject.Instantiate(prefab);
            inst.transform.position = position;

            if (parent)
                inst.transform.SetParent(transform);

            return inst;
        }

        void updateCurrentMode()
        {
            disableAllModes();

            switch(currentMode)
            {
                case PlayerModes.BASIC_MOVE:
                    modeBasicMove.enabled = true;
                    break;

                case PlayerModes.SPRING:
                    modeSpring.enabled = true;
                    break;

                case PlayerModes.SWIMMING:
                    modeSwimming.enabled = true;
                    break;

                case PlayerModes.HANGING:
                    modeHanging.enabled = true;
                    break;

                case PlayerModes.DEBUG_MOVE:
                    modeDebugMove.enabled = true;
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

        void debugControls()
        {

            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                
                if (currentMode == PlayerModes.DEBUG_MOVE)
                {
                    changeCurrentMode(PlayerModes.BASIC_MOVE);
                }
                else
                {
                    changeCurrentMode(PlayerModes.DEBUG_MOVE);
                }
            }

            if(Input.GetKeyDown(KeyCode.Alpha8))
            {
                changeCurrentMode(PlayerModes.SWIMMING);
                
            }

            if (Input.GetKeyDown(KeyCode.Alpha7))
                PlayerManager.Instance.givePlayerStealthCamo();
        }

    }

}
