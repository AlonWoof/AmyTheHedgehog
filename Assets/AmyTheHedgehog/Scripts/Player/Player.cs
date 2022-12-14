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
        INIT,
        BASIC_MOVE,
        SPRING,
        HANGING,
        SWIMMING,
        FLYING,
        LISTENING,
        CUTSCENE,
        RUBBING,
        DIE,
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
        public CharacterParams mPars;

        //This vector must always be normalized.
        public Vector3 mDirection = Vector3.forward;

        public float mForwardVelocity = 0.0f;
        public Vector3 mCurrentMovement = Vector3.zero;
        public Vector3 mDesiredMovement = Vector3.zero;

        public float groundAltitude = 0.0f;
        public float waterDepth = 0.0f;
        public float playerHeight = 0.9f;
        public float lungCapacity = 20.0f;
        public float airLeft = 20.0f;

        //Fun statistics
        public float distanceTraveledSinceSpawn = 0.0f;
        Vector3 lastPos = Vector3.zero;

        public Vector3 groundNormal;
        public bool isGrounded;

        public LayerMask mColMask;

        public ThirdPersonCamera tpc;
        public GameObject fx_waterWadingFX;
        public WetnessDirtynessProxy fx_wetDirty;
        public ActorOpacity fx_ActorOpacity;

        public List<Renderer> mRenderers;

        //Modes
        public PlayerModes currentMode;
        public PlayerModes lastMode;


        PlayerBasicMove modeBasicMove;
        PlayerSpringBounce modeSpring;
        PlayerSwimming modeSwimming;
        PlayerFlying modeFlying;
        PlayerHanging modeHanging;
        PlayerRubbing modeRubbing;
        PlayerDie modeDie;
        PlayerDebugMove modeDebugMove;

        //Special "modes". Or submodes, really.
        PlayerActivator activator;


        public Transform hipBoneTransform;
        public Transform headBoneTransform;

        public float headOffsetFromGround;

        public bool isBallMode = false;

        public float hitStun = 0.0f;

        private void Awake()
        {
            mColMask = LayerMask.GetMask("Collision");
           
        }


        public static Player Spawn(Vector3 pos, Vector3 dir, PlayableCharacter chara = PlayableCharacter.Amy)
        {
            CharacterParams cpar = GameManager.getSystemData().AmyParams;

            if(chara == PlayableCharacter.Cream)
                cpar = GameManager.getSystemData().CreamParams;


            float amy_height = cpar.height;


            //Fandom wiki says 25 kg, official sources cheekily say ヒ・ミ・ツ！ ("it's a secret!")
            //So let's assume 25kg, seems about right.
            float amy_weight = cpar.weight;



            GameObject inst = GameObject.Instantiate(cpar.ingameModel);     
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

            anim.runtimeAnimatorController = cpar.ingameAnimator;

            CharacterPhysics jiggles = inst.AddComponent<CharacterPhysics>();
            jiggles.mData = cpar.jiggleData;

            FootstepFX footsteps = inst.AddComponent<FootstepFX>();
            footsteps.isPlayer = true;

            Player newPlayer = inst.AddComponent<Player>();

            GameObject camInst = new GameObject("ThirdPersonCamera");
            ThirdPersonCamera tpc = camInst.AddComponent<ThirdPersonCamera>();

            tpc.setPlayerTransform(inst.transform);
            tpc.centerBehindPlayer();

            WetnessDirtynessProxy wetdirt = inst.AddComponent<WetnessDirtynessProxy>();
            inst.AddComponent<ActorOpacity>();

            newPlayer.tpc = tpc;
            newPlayer.fx_waterWadingFX = GameObject.Instantiate(GameManager.Instance.systemData.RES_WaterWadingFX);

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
            newPlayer.mPars = cpar;
            newPlayer.playerHeight = amy_height;

            return newPlayer;
        }

        void getPlayerBones()
        {
            hipBoneTransform = getBoneByName("hips");
            headBoneTransform = getBoneByName("head");

            headOffsetFromGround = (headBoneTransform.position.y - 0.05f) - transform.position.y;
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

            fx_wetDirty = GetComponent<WetnessDirtynessProxy>();
            fx_ActorOpacity = GetComponent<ActorOpacity>();

            mAnimator.applyRootMotion = false;
        }

        void addAllModes()
        {
            modeBasicMove = gameObject.AddComponent<PlayerBasicMove>();
            modeHanging = gameObject.AddComponent<PlayerHanging>();
            modeSpring = gameObject.AddComponent<PlayerSpringBounce>();
            modeSwimming = gameObject.AddComponent<PlayerSwimming>();
            modeFlying = gameObject.AddComponent<PlayerFlying>();
            modeRubbing = gameObject.AddComponent<PlayerRubbing>();
            modeDie = gameObject.AddComponent<PlayerDie>();
            modeDebugMove = gameObject.AddComponent<PlayerDebugMove>();

            activator = gameObject.AddComponent<PlayerActivator>();
        }

        void disableAllModes()
        {
            modeBasicMove.enabled = false;
            modeHanging.enabled = false;
            modeSpring.enabled = false;
            modeSwimming.enabled = false;
            modeFlying.enabled = false;
            modeRubbing.enabled = false;
            modeDie.enabled = false;
            modeDebugMove.enabled = false;


            activator.enabled = false;
        }


        public float getAltitudeFromGround()
        {
            float altitude = 1000.0f;

            Vector3 start = transform.position + Vector3.up * 0.2f;
            Vector3 end = transform.position - 1000.0f * Vector3.up;

            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Linecast(start, end, out hitInfo, mColMask))
            {
                altitude = transform.position.y - hitInfo.point.y;
            }

            return altitude;
        }

        public float getWaterYPos()
        {
            //ITS UNDER NEGATIVE NINE THOUSAAAAND
            float water_y = -9001.0f;

            Vector3 start = transform.position + 2048.0f * Vector3.up;
            Vector3 end = transform.position - 2048.0f * Vector3.up;

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

        public void updateWaterFX()
        {

            fx_wetDirty.dirtLevel = PlayerManager.Instance.getCharacterStatus(mChara).dirtiness;

            float depth = getWaterDepth();

            if (depth > 0.0f && depth < playerHeight)
            {
                fx_waterWadingFX.gameObject.SetActive(true);

                Vector3 wpos = transform.position;
                wpos.y = getWaterYPos();
            }
            else
            {
                fx_waterWadingFX.transform.position = Vector3.down * 100000.0f;
            }


            if (depth > 0.5f)
            {
                fx_wetDirty.inWater = true;
                fx_wetDirty.wetLevel = 0.75f;

                if(PlayerManager.Instance.getCharacterStatus(mChara).dirtiness > 0.0f)
                    PlayerManager.Instance.getCharacterStatus(mChara).dirtiness -= Time.deltaTime * 0.5f;
            }
            else
            {
                fx_wetDirty.inWater = false;
            }

            if(depth > playerHeight * 1.1f)
            {
                airLeft -= Time.deltaTime;

                if (airLeft < 0.0f)
                    airLeft = 0.0f;

                if (airLeft <= 0.0f)
                    damageHealth(Time.deltaTime * 0.2f);
            }
            if (depth < playerHeight)
            {
                airLeft += Time.deltaTime * 5.0f;

                if (airLeft > lungCapacity)
                    airLeft = lungCapacity;
            }

        }

    	// Start is called before the first frame update
    	void Start()
    	{
            getBaseComponents();
            getPlayerBones();
            addAllModes();
            disableAllModes();

            updateCurrentMode();

            mRenderers = new List<Renderer>();

            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                mRenderers.Add(r);
            }

            Invoke("initComplete", 0.1f);
        }

        void initComplete()
        {
            changeCurrentMode(PlayerModes.BASIC_MOVE);
        }

        void handleOpacity()
        {
            


            if (hitStun > 0.0f)
            {

                int frame = 15;

                if (hitStun < 0.25f)
                    frame = 15;

                if (Time.frameCount % frame == 0)
                {
                    if (fx_ActorOpacity.opacity > 0.0f)
                    {
                        fx_ActorOpacity.opacity = 0.0f;
                    }
                    else if (fx_ActorOpacity.opacity < 1.0f)
                    {
                        fx_ActorOpacity.opacity = 1.0f;
                    }

                }

                return;
            }

            fx_ActorOpacity.opacity = 1.0f;

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
            updateDistance();

        }

        void updateDistance()
        {
            if(!GameManager.Instance.playerInputDisabled)
            {
                distanceTraveledSinceSpawn += Vector3.Distance(lastPos, transform.position);
            }

            lastPos = transform.position;
        }


        private void LateUpdate()
        {
            // Debug.Log("Delta Timestep: " + Time.deltaTime);

            handleOpacity();
            updateWaterFX();
            checkHealth();
            updateMood();
            
        }
        
        private void FixedUpdate()
        {
            //Debug.Log("Fixed Timestep: " + Time.fixedDeltaTime);
            
        }

        public void updateMood()
        {
            PlayerStatus mStats = PlayerManager.Instance.getCharacterStatus(mChara);

            float moodAverage = (mStats.currentHealth + mStats.currentMood) * 0.5f;

            if (moodAverage > 0.25f)
                mAnimator.SetFloat("mood", 1.0f);
            else
                mAnimator.SetFloat("mood", 0.0f);


            if (EnemyManager.Instance.currentEnemyPhase == ENEMY_PHASE.PHASE_ALERT)
                damageMood(0.015f * Time.deltaTime);

            if (EnemyManager.Instance.currentEnemyPhase == ENEMY_PHASE.PHASE_EVADE)
                damageMood(0.005f * Time.deltaTime);

            if (EnemyManager.Instance.currentEnemyPhase == ENEMY_PHASE.PHASE_SNEAK)
            {
                if(currentMode == PlayerModes.BASIC_MOVE && mForwardVelocity < 0.01f)
                    healMood(0.0006f * Time.deltaTime);
            }

            PlayerManager.Instance.getCharacterStatus(mChara).currentMood = Mathf.Clamp01(PlayerManager.Instance.getCharacterStatus(mChara).currentMood);
        }

        public void checkHealth()
        {

            if(hitStun > 0.0f)
                hitStun -= Time.deltaTime;

            if (currentMode == PlayerModes.DIE)
                return;

            if (GameManager.Instance.cutsceneMode)
                return;

            //Will I ever need to have a position this low?
            if (transform.position.y < -200.0f)
            {
                voidOut();
                return;
            }

            if (PlayerManager.Instance.getCurrentPlayerStatus().currentHealth <= 0.0f)
            {
                modeDie.deathType = PlayerDie.DeathType.Normal;

                if (currentMode == PlayerModes.SWIMMING)
                    modeDie.deathType = PlayerDie.DeathType.Drowned;

                changeCurrentMode(PlayerModes.DIE);
            }


            PlayerManager.Instance.getCharacterStatus(mChara).currentHealth = Mathf.Clamp01(PlayerManager.Instance.getCharacterStatus(mChara).currentHealth);
        }

        //TODO: Add character specific modifiers later
        //IE Cream isn't as good at controlling stress and gets hurt easier

        public void healMood(float amnt)
        {
            PlayerManager.Instance.getCharacterStatus(mChara).currentMood += amnt;
            PlayerManager.Instance.getCharacterStatus(mChara).currentMood = Mathf.Clamp01(PlayerManager.Instance.getCharacterStatus(mChara).currentMood);

        }

        public void damageMood(float amnt)
        {
            PlayerManager.Instance.getCharacterStatus(mChara).currentMood -= amnt;
            PlayerManager.Instance.getCharacterStatus(mChara).currentMood = Mathf.Clamp01(PlayerManager.Instance.getCharacterStatus(mChara).currentMood);
        }

        public void healHealth(float amnt)
        {
            PlayerManager.Instance.getCharacterStatus(mChara).currentHealth += amnt;
            PlayerManager.Instance.getCharacterStatus(mChara).currentHealth = Mathf.Clamp01(PlayerManager.Instance.getCharacterStatus(mChara).currentHealth);
        }
        public void damageHealth(float amnt)
        {
            PlayerManager.Instance.getCharacterStatus(mChara).currentHealth -= amnt;
            PlayerManager.Instance.getCharacterStatus(mChara).currentHealth = Mathf.Clamp01(PlayerManager.Instance.getCharacterStatus(mChara).currentHealth);
        }

        public void onTakeDamage(float amnt, Vector3 srcpos)
        {
            if (hitStun > 0.0f)
                return;


            hitStun = 1.0f;
            damageHealth(amnt);

        }

        public void voidOut()
        {
            tpc.lockPosition = true;
            modeDie.deathType = PlayerDie.DeathType.Falling;
            changeCurrentMode(PlayerModes.DIE);

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
                    activator.enabled = true;
                    break;

                case PlayerModes.SPRING:
                    modeSpring.enabled = true;
                    break;

                case PlayerModes.SWIMMING:
                    modeSwimming.enabled = true;
                    activator.enabled = true;
                    break;

                case PlayerModes.FLYING:
                    modeFlying.enabled = true;
                    break;

                case PlayerModes.LISTENING:
                    break;

                case PlayerModes.HANGING:
                    modeHanging.enabled = true;
                    break;

                case PlayerModes.RUBBING:
                    modeRubbing.enabled = true;
                    break;

                case PlayerModes.DIE:
                    modeDie.enabled = true;
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
                // PlayerManager.Instance.killHer();
                //changeCurrentMode(PlayerModes.RUBBING);
                onTakeDamage(0.15f, transform.position + transform.forward);

            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                // PlayerManager.Instance.killHer();
                changeCurrentMode(PlayerModes.RUBBING);
               // onTakeDamage(0.15f, transform.position + transform.forward);

            }


            if (Input.GetKeyDown(KeyCode.Alpha7))
                PlayerManager.Instance.givePlayerStealthCamo();
        }

    }

}
