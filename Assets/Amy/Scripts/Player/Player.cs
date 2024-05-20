using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using RootMotion.FinalIK;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[System.Serializable]
	public class PlayerParameters
    {
		public GameObject ingameModel;
		public RuntimeAnimatorController ingameAnimator;
		public CharacterPhysicsData jiggleData;
		public HitboxData[] hitBoxes;

		public float height = 1.0f;
		public float weight = 50.0f;

		public float forwardAccel = 4.5f;
		public float airAccel = 5.0f;
		public float jumpSpeed = 5.0f;
		public float groundFriction = 0.5f;
		public float airResistance = 0.45f;
		public float slopeResistance = 4.5f;

		public float railSpeed = 6.0f;

		public float jump_hangTime = 1.0f;

		public float gravityMult = 1.5f;

		public float swimSpeed = 3.0f;

		public PlayerStatus baseStats;
	}

	public enum PlayerModes
    {
		NORMAL,
		SPRING,
		RAIL,
		SWIMMING,
		FLY,
		LISTENING,
		SLINGSHOT,
		LADDER,
		CUTSCENE,
		RUBBING,
		HURT,
		KILLED,
		FIRSTPERSON,
		DEBUG_MOVE
    }

	public class Player : MonoBehaviour
	{
		//Consts
		const float hammerJumpStaimaCost = 0.05f;
		const float jumpStaimaCost = 0.025f;
		const float hammerAttackStaminaCost = 0.025f;
		const float moveStaminaCost = 0.002f;
		const float flyStaminaCost = 0.004f;



		//Components
		public Rigidbody mRigidBody;
		public Animator mAnimator;
		public FootstepFX fx_footsteps;
		public WeaponTrailFX fx_hammerTrail;
		public ThirdPersonCamera tpc;
		public PlayerVoice mVoice;
		public ActorLookAtController lookAtController;
		public GameObject fx_waterWadingFX;
		public BipedIK biped;

		//Amy-specific
		public AmyHammer mAmyHammer;

		//Character Data
		public PlayerParameters mParam;
		public PlayableCharacter mChara;

		//Movement vectors
		public Vector3 speed = Vector3.zero;
		public Vector3 acceleration = Vector3.zero;
		public Vector3 platformVelocity = Vector3.zero;
		public Vector3 direction = Vector3.zero;
		public Vector3 prev_direction = Vector3.zero;
		public Vector3 groundNormal = Vector3.up;
		public Vector3 lastSafeGroundPosition;
		public float slopeAmount = 0.0f;

		public Vector3 stickAngle;
		public float stickPower;

		

		public LayerMask mColMask;

		//Status indicators
		public bool isOnGround = false;
		public bool isSliding = false;
		public bool isBallMode = false;
		public bool isHammerJumping = false;
		public bool isAttacking = false;
		public bool isHammerSpin = false;
		public bool canAirAttack = false;
		public float mutekiTimer = 0.0f;
		public float attackTimer = 0.0f;
		public float interactTimeout = 0.0f;
		public float hammerJumpCharge = 0.0f;
		public float airLeft = 0.0f;
		public int framesAirborne = 0;
		public int framesGrounded = 0;
		public float lookTimeLeft = 0.0f;

		public float stickTimeout = 0.0f;
		public float jumpTimer = 0.0f;

		public Transform hipBoneTransform;
		public Transform headBoneTransform;

		public Transform rThighBoneTransform;
		public Transform lThighBoneTransform;

		public float leanAmount = 0.0f;

		public float headOffsetFromGround;

		//Modes
		public PlayerModes currentMode;
		public PlayerModes lastMode;

		public PlayerBasicMove modeBasic;
		public PlayerSpringBounce modeSpring;
		public PlayerSwimming modeSwimming;
		public PlayerFly modeFly;
		public PlayerRail modeRail;
		public PlayerSlingshot modeSlingshot;
		public PlayerClimb modeLadder;
		public PlayerRubbing modeRubbing;
		public PlayerListening modeListening;
		public PlayerHurt modeHurt;
		public PlayerKilled modeKilled;
		public PlayerFirstPerson modeFirstPerson;
		public PlayerDebugMove modeDebug;
		

		public PlayerAreaDetector areaDetector;

		public List<Hitbox> hitBoxes;

		public static Player Spawn(Vector3 pos, Vector3 dir, PlayableCharacter chara = PlayableCharacter.Amy)
		{
			//Replace this with something better later.
			GameManager.Instance.findSceneInfo();

			PlayerParameters cpar = GameManager.getSystemData().AmyParams;

			if (chara == PlayableCharacter.Cream)
				cpar = GameManager.getSystemData().CreamParams;

			float amy_height = cpar.height;

			//Fandom wiki says 25 kg, official sources cheekily say ヒ・ミ・ツ！ ("it's a secret!")
			//So let's assume 25kg, seems about right.
			float amy_weight = cpar.weight;

			GameObject inst = GameObject.Instantiate(cpar.ingameModel);
			inst.transform.position = pos;

			CapsuleCollider col = inst.AddComponent<CapsuleCollider>();
			col.radius = 0.25f;
			col.height = amy_height * 0.7f;
			col.center = (Vector3.up * amy_height) * 0.7f;

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

			if (!anim)
				inst.AddComponent<Animator>();

			anim.runtimeAnimatorController = cpar.ingameAnimator;
			anim.applyRootMotion = false;

			CharacterPhysics jiggles = inst.AddComponent<CharacterPhysics>();
			jiggles.mData = cpar.jiggleData;

			FootstepFX footsteps = inst.AddComponent<FootstepFX>();
			footsteps.isPlayer = true;

			Player newPlayer = inst.AddComponent<Player>();

			GameObject camInst = new GameObject("ThirdPersonCamera");
			ThirdPersonCamera tpc = camInst.AddComponent<ThirdPersonCamera>();

			tpc.setPlayerTransform(inst.transform);
			tpc.centerBehindPlayer();
			newPlayer.tpc = tpc;

			newPlayer.fx_footsteps = footsteps;

			AmyTailAnimation tail = inst.AddComponent<AmyTailAnimation>();
			

			//WetnessDirtynessProxy wetdirt = inst.AddComponent<WetnessDirtynessProxy>();
			//inst.AddComponent<ActorOpacity>();


			newPlayer.fx_waterWadingFX = GameObject.Instantiate(GameManager.Instance.systemData.RES_WaterWadingFX);

			newPlayer.direction = dir;


			newPlayer.mChara = chara;
			newPlayer.mParam = cpar;
			//newPlayer.playerHeight = amy_height;

			foreach(Transform t in inst.GetComponentsInChildren<Transform>(true))
            {
				t.gameObject.layer = LayerMask.NameToLayer("Actor");
            }

			//Let's keep track of the hitboxes~
			newPlayer.hitBoxes = new List<Hitbox>();

			if (cpar.hitBoxes != null)
			{
				foreach (HitboxData h in cpar.hitBoxes)
				{
					Hitbox newHB = h.addToTransform(newPlayer.getBoneByName(h.boneName));
					
					if(newHB)
                    {
						newPlayer.hitBoxes.Add(newHB);
						newHB.isPlayerHitbox = true;
						newHB.damageTeam = DamageTeam.Player;
						newHB.mPlayer = newPlayer;
                    }
				}
			}

			//Weapon trail fx
			GameObject wfx = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_pikoHammerTrail);
			newPlayer.fx_hammerTrail = wfx.GetComponent<WeaponTrailFX>();
			newPlayer.fx_hammerTrail.weaponNode = newPlayer.getBoneByName("HurtBox_Hammer");

			newPlayer.mAmyHammer = inst.GetComponentInChildren<AmyHammer>();

			inst.AddComponent<WetFX>();
			DirtFX dirt = inst.AddComponent<DirtFX>();
			dirt.mChara = chara;

			newPlayer.airLeft = newPlayer.calculateLungCapacity();

			return newPlayer;
		}

		public void addAllModes()
        {
			modeBasic = gameObject.AddComponent<PlayerBasicMove>();
			modeSpring = gameObject.AddComponent<PlayerSpringBounce>();
			modeSwimming = gameObject.AddComponent<PlayerSwimming>();
			modeFly = gameObject.AddComponent<PlayerFly>();
			modeRail = gameObject.AddComponent<PlayerRail>();
			modeSlingshot = gameObject.AddComponent<PlayerSlingshot>();
			modeLadder = gameObject.AddComponent<PlayerClimb>();
			modeRubbing = gameObject.AddComponent<PlayerRubbing>();
			modeListening = gameObject.AddComponent<PlayerListening>();
			modeHurt = gameObject.AddComponent<PlayerHurt>();
			modeKilled = gameObject.AddComponent<PlayerKilled>();
			modeFirstPerson = gameObject.AddComponent<PlayerFirstPerson>();
			modeDebug = gameObject.AddComponent<PlayerDebugMove>();
		}

		public void disableAllModes()
        {
			modeBasic.enabled = false;
			modeSpring.enabled = false;
			modeSwimming.enabled = false;
			modeFly.enabled = false;
			modeRail.enabled = false;
			modeSlingshot.enabled = false;
			modeLadder.enabled = false;
			modeRubbing.enabled = false;
			modeListening.enabled = false;
			modeHurt.enabled = false;
			modeKilled.enabled = false;
			modeFirstPerson.enabled = false;
			modeDebug.enabled = false;
        }

		public void refreshMode()
        {
			disableAllModes();

			switch(currentMode)
            {
				case PlayerModes.NORMAL:
					modeBasic.enabled = true;
					break;

				case PlayerModes.SPRING:
					modeSpring.enabled = true;
					break;

				case PlayerModes.SWIMMING:
					modeSwimming.enabled = true;
					break;

				case PlayerModes.FLY:
					modeFly.enabled = true;
					break;

				case PlayerModes.RAIL:
					modeRail.enabled = true;
					break;

				case PlayerModes.SLINGSHOT:
					modeSlingshot.enabled = true;
					break;

				case PlayerModes.LADDER:
					modeLadder.enabled = true;
					break;

				case PlayerModes.RUBBING:
					modeRubbing.enabled = true;
					break;

				case PlayerModes.LISTENING:
					modeListening.enabled = true;
					break;

				case PlayerModes.HURT:
					modeHurt.enabled = true;
					break;

				case PlayerModes.KILLED:
					modeKilled.enabled = true;
					break;

				case PlayerModes.FIRSTPERSON:
					modeFirstPerson.enabled = true;
					break;

				case PlayerModes.DEBUG_MOVE:
					modeDebug.enabled = true;
					break;
            }
        }

		public void getBaseComponents()
		{
			mRigidBody = GetComponent<Rigidbody>();
			mAnimator = GetComponent<Animator>();
			mVoice = GetComponent<PlayerVoice>();
			lookAtController = GetComponent<ActorLookAtController>();
		}

		void getPlayerBones()
		{
			hipBoneTransform = getBoneByName("hips");
			headBoneTransform = getBoneByName("head");

			rThighBoneTransform = getBoneByName("thigh_r");
			lThighBoneTransform = getBoneByName("thigh_l");

			headOffsetFromGround = (headBoneTransform.position.y - 0.05f) - transform.position.y;
		}

		public Transform getBoneByName(string name)
		{
			foreach (Transform t in GetComponentsInChildren<Transform>())
			{
				if (t.gameObject.name.ToLower() == name.ToLower())
					return t;
			}

			return null;
		}

		public void lookAt(Vector3 lookPos, float time = 0.25f)
        {
			if (!lookAtController)
				return;

			lookAtController.desiredLookAt = lookPos;
			lookAtController.lookingAtTarget = true;
			lookTimeLeft = time;

		}

		public void freeLookAt()
        {
			if (!lookAtController)
				return;

			lookAtController.desiredLookAt = transform.forward * 0.2f + transform.up * (mParam.height * 0.75f);
			lookAtController.lookingAtTarget = false;
			lookTimeLeft = 0.0f;
			//lookAtController.eyeLook.overrideLook = false;
			//lookAtController.eyeLook.look_x_left = 0.0f;
		}

		void doLeanAnimation()
        {
			float angle = (20.0f * leanAmount) * Mathf.Clamp01(Mathf.Abs(acceleration.z) / 6.0f);

			leanAmount = Mathf.Lerp(leanAmount, 0.0f, Time.deltaTime * 3.0f);


			hipBoneTransform.rotation = hipBoneTransform.rotation * Quaternion.Euler(0, 0, angle);

			rThighBoneTransform.rotation = rThighBoneTransform.rotation * Quaternion.Euler(0, 0, angle);
			lThighBoneTransform.rotation = lThighBoneTransform.rotation * Quaternion.Euler(0, 0, angle);

			headBoneTransform.rotation = headBoneTransform.rotation * Quaternion.Euler(0, Mathf.Clamp(-angle * 5.0f, -40, 40), 0);
		}

		public bool canWarp()
        {
			if (currentMode != PlayerModes.NORMAL)
				return false;

			if (!isOnGround)
				return false;

			if (Vector3.Dot(groundNormal, Vector3.up) < 0.25f)
				return false;

			if (getStatus().checkStatusEffect(PlayerStatusFX.Scared))
				return false;

			return true;
        }


		public void startWarp(string sceneName, int exitNum = 0)
        {

			Timing.RunCoroutine(doWarp(sceneName, exitNum));
        }

		Vector3 getMagicCirclePos()
		{
			int iterations = 8;

			float ang = 0;

			
			float highestY = WorldToPlayerSpace(transform.position).y;

			for (int i = 0; i < iterations; i++)
            {
				float seg = (360.0f / (float)iterations);

				Vector3 rotatedOffset = Quaternion.Euler(0, seg * i, 0) * (transform.position + (transform.forward * 0.75f));

				Vector3 start = rotatedOffset + transform.up;
				Vector3 end = rotatedOffset - transform.up;

				Debug.DrawLine(start, end, SystemColors.AmyColor, 10.0f);

				RaycastHit hitInfo = new RaycastHit();
				LayerMask mask = LayerMask.GetMask("Collision");

				if(Physics.Linecast(start, end, out hitInfo, mask))
                {
					Vector3 point = WorldToPlayerSpace(hitInfo.point);

					if(point.y > highestY)
                    {
						highestY = point.y;
					}
                }
			}

			Vector3 newPos = WorldToPlayerSpace(transform.position);
			newPos.y = highestY;

			return PlayerToWorldSpace(newPos);

        }

		public IEnumerator<float> doWarp(string sceneName, int exitNum = 0)
        {
			GameManager.Instance.disablePlayerInput();
			changeCurrentMode(PlayerModes.CUTSCENE);

			clearAccel();
			clearSpeed();
			updatePosition();

			GameObject inst = null;
			
			if(mChara == PlayableCharacter.Amy)
				inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_amyMagicCircle);
			else
				inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_creamMagicCircle);

			inst.transform.position = transform.position;
			inst.transform.rotation = transform.rotation;
			inst.transform.SetParent(transform);
			mAnimator.Play("Kaeru_Start");

			yield return Timing.WaitForSeconds(3.0f);

			PlayerManager.Instance.exitType = ExitLevelType.WARP;
			PlayerManager.Instance.lastExit = exitNum;
			GameManager.Instance.loadScene(sceneName, true);

		}

		public void startWarpExit()
        {
			Timing.RunCoroutine(doWarpExit());
        }

		public IEnumerator<float> doWarpExit()
        {
			yield return Timing.WaitForSeconds(1.5f);

			GameManager.Instance.disablePlayerInput();
			

			clearAccel();
			clearSpeed();
			updatePosition();
			isOnGround = true;
			//framesAirborne = 0;

			GameObject fx = null;

			if(mChara == PlayableCharacter.Cream)
			fx = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_creamWarpIn);
				else
			fx = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_amyWarpIn);
			
			
			fx.transform.position = transform.position;
			
			
			mAnimator.Play("Exit_Warp");

			while (!mAnimator.IsInTransition(0))
			{
				if(currentMode != PlayerModes.CUTSCENE)
					changeCurrentMode(PlayerModes.CUTSCENE);

				yield return 0f;
			}

			yield return Timing.WaitForSeconds(0.5f);

			GameManager.Instance.enablePlayerInput();
			changeCurrentMode(PlayerModes.NORMAL);

		}


		public PlayerStatus getStatus()
        {
			PlayerStatus result = null;

			switch(mChara)
            {
				case PlayableCharacter.Amy:
					result = PlayerManager.Instance.AmyStatus;
					break;

				case PlayableCharacter.Cream:
					result = PlayerManager.Instance.CreamStatus;
					break;
			}

			return result;
        }



		private void Awake()
		{
			addAllModes();
			getBaseComponents();
			getPlayerBones();
			mColMask = LayerMask.GetMask("Collision");

			areaDetector = gameObject.AddComponent<PlayerAreaDetector>();
		}

		// Start is called before the first frame update
		void Start()
		{
			getBaseComponents();
			updateExpression();
		}

		// Update is called once per frame
		void Update()
		{

			if (GameManager.Instance.gamePaused)
				return;

			float runAnimProgress = Mathf.Abs(acceleration.z);

			float runAnimSpeed = acceleration.z;

			if (acceleration.z < 0.0f)
				runAnimProgress = Mathf.Clamp(runAnimProgress, 0, 3);

			if (runAnimSpeed < 0.75f && runAnimSpeed > 0.0f)
				runAnimSpeed = 0.75f;

			if (runAnimSpeed > -0.75f && runAnimSpeed < 0.0f)
				runAnimSpeed = -0.75f;

			if(attackTimer > 0.0f)
            {
				attackTimer -= Time.deltaTime;


				if (attackTimer <= 0.0f)
                {


					isAttacking = false;
					isHammerSpin = false;
					attackTimer = 0.0f;

					if (fx_hammerTrail)
						fx_hammerTrail.disableFX();

					if (mAmyHammer)
						mAmyHammer.disableHurtbox();
				}
            }

			if(mutekiTimer > 0.0f)
            {
				mutekiTimer -= Time.deltaTime;

				if (mutekiTimer <= 0.0f)
				{
					//Stop flashing
					mAnimator.CrossFade("Flash_Neutral",0.2f);

					mutekiTimer = 0.0f;
				}
			}

			if(lookTimeLeft > 0.0f)
            {
				lookTimeLeft -= Time.deltaTime;

				if(lookTimeLeft <= 0.0f)
                {
					freeLookAt();
                }
            }

			mAnimator.SetFloat("y_accel", speed.y);
			mAnimator.SetFloat("z_accel", runAnimProgress);
			mAnimator.SetFloat("run_anim_speed", runAnimSpeed);

			float moodFac = (getStatus().currentMood / getStatus().maxMood);

			mAnimator.SetFloat("mood", moodFac);

			updateWaterFX();

			debugControls();
		}

		public void updateBallState()
        {

        }

		public void updateHealth()
		{
			PlayerStatus pstats = getStatus();

			pstats.currentHealth = Mathf.Clamp(pstats.currentHealth, 0, pstats.maxHealth);
			pstats.currentMood = Mathf.Clamp(pstats.currentMood, 0, pstats.maxMood);

			if (pstats.currentHealth == 0)
			{
				if(currentMode == PlayerModes.NORMAL)
                {
					modeKilled.deathType = PlayerKilled.DeathType.Normal;
					changeCurrentMode(PlayerModes.KILLED);
				}
			}

		}

		public GameObject spawnFX(GameObject fx, float zoffs = 0.0f)
        {
			GameObject inst = GameObject.Instantiate(fx);
			inst.transform.position = transform.position + Vector3.up * zoffs;
			inst.transform.rotation = transform.rotation;

			return inst;
        }

		public bool takeDamage(Damage dmg, float multiplier = 1.0f)
        {
			if (mutekiTimer > 0.0f && dmg.damageType != DamageType.Crush)
				return false;

			if (attackTimer > 0.0f && dmg.damageType != DamageType.Crush)
				return false;

			if (dmg.damageType == DamageType.Neutral)
				spawnFX(GameManager.Instance.systemData.RES_GenericHitFX);		

			PlayerStatus pstats = getStatus();

			int rings = PlayerManager.Instance.getRings();
			float moodFac = pstats.currentMood / pstats.maxMood;

			//Rings protect you from ouchies.
			if (rings > 0)
				dmg.damageAmount *= 0.5f;

			//Can tolerate pain better if feeling good.
			if(moodFac > 0.75f)
				dmg.damageAmount *= 0.85f;

			float force = 16.0f;

			if (rings > 0)
				force *= 0.75f;

			if (currentMode == PlayerModes.NORMAL || currentMode == PlayerModes.FLY || currentMode == PlayerModes.SLINGSHOT)
            {
				

				if (currentMode == PlayerModes.NORMAL && force > 1.0f)
				{
					if (dmg.useSourceDir)
					{
						Vector3 dir = Helper.getDirectionTo(transform.position, dmg.source.transform.position);
						modeHurt.setKnockBack(dmg.source.transform.position, force);
						
					}
					else if(!isOnGround)
					{
						Vector3 knockDir = Helper.getDirectionTo(transform.position, lastSafeGroundPosition);
						knockDir.y = 0;

						modeHurt.setKnockBackDirectional(knockDir.normalized, force);
						Debug.DrawLine(transform.position, transform.position + knockDir.normalized, Color.yellow, 30.0f);
					}
					else
                    {
						modeHurt.setKnockBack(transform.position + transform.forward + Vector3.up * 0.5f, force);
					}
				}

				if (currentMode == PlayerModes.FLY)
				{
					modeHurt.setKnockBack(transform.position + Vector3.up, 3.0f);
					updateEars();
				}

				if (currentMode == PlayerModes.SLINGSHOT)
				{
					modeSlingshot.aim_y += Random.Range(-15.0f, 15.0f);
					modeSlingshot.aim_x += Random.Range(-5.0f, 5.0f);

					
				}


				pstats.currentHealth -= (dmg.damageAmount * multiplier);
				pstats.clampValues();

				if (rings > 0)
					pstats.currentHealth = Mathf.Clamp(pstats.currentHealth, 1, pstats.maxHealth);

				if (pstats.currentHealth > 0)
				{
					if(currentMode != PlayerModes.SLINGSHOT)
						changeCurrentMode(PlayerModes.HURT);
				}
				else
				{
					modeKilled.deathType = PlayerKilled.DeathType.Normal;
					changeCurrentMode(PlayerModes.KILLED);
					return true;
				}



				if (multiplier < 1.1f && force < 10.0f)
				{
					mVoice.playVoice(mVoice.smallPain);
				}
				else
				{
					mVoice.playVoice(mVoice.largePain);
				}

				mAnimator.Play("Flash_Red_Fast");

				updateHealth();
			}



			if (rings > 0)
				damageRingScatter();

			mutekiTimer = 2.75f;
			return true;
		}

		void damageRingScatter()
        {
			int rings = PlayerManager.Instance.getRings();

			if (rings > 20)
				rings = 20;

			Transform scatterer = new GameObject("scatter").transform;
			scatterer.transform.position = transform.position + Vector3.up * 0.75f;

			PlayerManager.Instance.subtractRings(rings);

			for(int i = 1; i < rings + 1; i++)
            {

				float angle = (360.0f / rings) * i;

				scatterer.transform.rotation = Quaternion.Euler(0, angle, 0);

				GameObject inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_RingTobitiri);

				inst.transform.position = scatterer.transform.position + scatterer.transform.forward * 0.25f;

				Rigidbody r = inst.GetComponent<Rigidbody>();
				r.velocity = (scatterer.transform.forward * Random.Range(2.5f,3.0f)) + Vector3.up * Random.Range(2.5f, 3.0f);

            }

			GameObject tobitiriSFX = GameObject.Instantiate(GameManager.Instance.systemData.RES_RingTobitiriFX);
			tobitiriSFX.transform.position = transform.position;

		}

        private void LateUpdate()
        {
			if (GameManager.Instance.gamePaused)
				return;

			doLeanAnimation();

			//if (acceleration.z > 0.5f)
			//	tpc.centerBehindPlayerSmooth(Time.deltaTime);
		}

		public void updateExpression()
        {
			if (currentMode == PlayerModes.RUBBING)
				mAnimator.Play("Face_Ecchi");
			else if (currentMode == PlayerModes.HURT)
				mAnimator.Play("Face_Itai");
			else if(getStatus().checkStatusEffect(PlayerStatusFX.Scared) || currentMode == PlayerModes.KILLED)
				mAnimator.Play("Face_Kowaii");
			else
				mAnimator.Play("Face_Neutral");
		}

		public void updateEars()
        {
			//For creamy's floppy bunny ears~

			if(currentMode == PlayerModes.FLY)
				mAnimator.CrossFade("Ears_Flying", 0.25f);
			else
				mAnimator.CrossFade("Ears_Normal", 0.25f);

		}

        void debugControls()
        {
			if (Input.GetKeyDown(KeyCode.Keypad5))
			{

				if (currentMode == PlayerModes.DEBUG_MOVE)
				{
					changeCurrentMode(PlayerModes.NORMAL);
				}
				else
				{
					changeCurrentMode(PlayerModes.DEBUG_MOVE);
				}
			}

			if(Input.GetKeyDown(KeyCode.Keypad7))
            {
				

				GameObject dSource = new GameObject("Dmg");

				Damage dmg = dSource.AddComponent<Damage>();

				dSource.transform.position = transform.position + Vector3.up * 0.5f + transform.forward;

				dmg.damageAmount = 5;
				dmg.damageType = DamageType.Neutral;
				dmg.source = dSource;

				takeDamage(dmg);

			}

			if (Input.GetKeyDown(KeyCode.Keypad9))
			{
				if(currentMode == PlayerModes.NORMAL)
                {
					changeCurrentMode(PlayerModes.RUBBING);
                }
			}
		}

		public void changeCurrentMode(PlayerModes newMode)
        {
			if (newMode == currentMode)
				return;

			lastMode = currentMode;
			currentMode = newMode;

			refreshMode();
			updateExpression();
			isBallMode = false;

			if (mChara == PlayableCharacter.Cream)
				updateEars();
		}

		public void CalcSlope()
		{
			getGroundNormal();

			Vector3 slide = Vector3.zero;

			slide.x += (1f - groundNormal.y) * groundNormal.x * (1f - 0.03f);
			slide.z += (1f - groundNormal.y) * groundNormal.z * (1f - 0.03f);

			if(isSliding)
            {
				acceleration.z += (slopeAmount * 3.0f) * Time.deltaTime;
            }

			//conv.x = 0;


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

			if (Physics.Linecast(start, end, out hitInfo, waterCol))
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

			float depth = getWaterDepth();

			
			if (depth > 0.0f && depth < mParam.height)
			{
				if(!fx_waterWadingFX.activeInHierarchy)
					fx_waterWadingFX.gameObject.SetActive(true);

				Vector3 wpos = transform.position;
				wpos.y = getWaterYPos();
				fx_waterWadingFX.transform.position = wpos;
			}
			else
			{
				fx_waterWadingFX.transform.position = Vector3.down * 100000.0f;
			}


			if (depth > 0.5f)
			{
				//fx_wetDirty.inWater = true;
				//fx_wetDirty.wetLevel = 0.75f;

				if (PlayerManager.Instance.getCharacterStatus(mChara).dirtiness > 0.0f)
					PlayerManager.Instance.getCharacterStatus(mChara).dirtiness -= Time.deltaTime * 0.5f;
			}
			else
			{
				//fx_wetDirty.inWater = false;
			}

			if (depth > mParam.height * 1.1f)
			{
				airLeft -= Time.deltaTime;

				if (airLeft < 0.0f)
					airLeft = 0.0f;

				if (airLeft <= 0.0f)
				{
					modeKilled.deathType = PlayerKilled.DeathType.Drowned;
					changeCurrentMode(PlayerModes.KILLED);
				}

			}
			if (depth < mParam.height)
			{
				airLeft += Time.deltaTime * 5.0f;

				if (airLeft > calculateLungCapacity())
					airLeft = calculateLungCapacity();
			}

		}

		public float calculateLungCapacity()
        {
			PlayerStatus pStats = getStatus();
			float staminaHealthAvg = (pStats.maxHealth + pStats.maxStamina) * 0.5f;

			return staminaHealthAvg * 0.5f;
		}

		public void CalcVerticalVelocity()
		{
			float gravityMult = mParam.gravityMult;

			if (checkFallOffCeiling() || checkFallOffWall())
				gravityMult *= -1.0f;

			float verticalVelocity = acceleration.y;

			if (isOnGround && !checkFallOffCeiling() && !checkFallOffWall())
			{
				if (jumpTimer < 0.01f)
					verticalVelocity = Mathf.Lerp(verticalVelocity, 0, 0.25f);
			}
			else
			{
				float extraJumpPower = (0.15f * (jumpTimer / mParam.jump_hangTime));
				verticalVelocity = Mathf.Lerp(verticalVelocity + extraJumpPower, Physics.gravity.y * gravityMult, Time.fixedDeltaTime * 1.5f);
			
			}

			acceleration.y = verticalVelocity;

			if (Mathf.Abs(acceleration.y) < 0.01f)
				acceleration.y = 0.0f;

			if (Mathf.Abs(speed.y) < 0.01f)
				speed.y = 0.0f;

			if (Mathf.Abs(platformVelocity.y) < 0.01f)
				platformVelocity.y = 0.0f;
		}

		bool checkFallOffWall()
        {

			

			float ang = Vector3.Dot(groundNormal, Vector3.up);
			float tolerance = Mathf.Lerp(0.0f, 10.0f, 1.0f - ang);

			//Debug.Log("DOT OF WALL: " + Vector3.Dot(groundNormal, Vector3.up) + "  TOLERANCE: " + tolerance);

			if (ang < 0.5f)
			{
				if (acceleration.z < tolerance)
				{

					return true;
				}
			}

			return false;
        }

		bool checkFallOffCeiling()
		{


			if (Vector3.Dot(groundNormal, Vector3.down) > 0.75f && acceleration.z < 4.0f)
			{
				return true;
			}

			return false;
		}

		public void checkIfUnderwater()
		{
			if (jumpTimer > mParam.jump_hangTime * 0.5f)
				return;

			if (getWaterDepth() >= headOffsetFromGround)
			{
				changeCurrentMode(PlayerModes.SWIMMING);
			}
		}

        private void OnDrawGizmos()
        {
			Gizmos.color = Color.yellow;

			Gizmos.DrawWireSphere(lastSafeGroundPosition, 0.5f);
        }

        void getGroundNormal()
		{
			Vector3 start = transform.position + WorldToPlayerSpace(Vector3.up * 0.5f);
			Vector3 end = transform.position - WorldToPlayerSpace(Vector3.up * 0.2f);

			//start += mRigidBody.velocity * Time.deltaTime;
			//end += mRigidBody.velocity * Time.deltaTime;

			//Debug.DrawLine(start, end, Color.green, 10.0f);

			RaycastHit hitInfo = new RaycastHit();

			if (Physics.Linecast(start, end, out hitInfo, mColMask))
			{
				Vector3 new_ground = hitInfo.normal.normalized;


				Vector3 mpos = transform.position;

				MeshCollider m = hitInfo.collider as MeshCollider;

				if (m != null)
				{
					Mesh mesh = m.sharedMesh;
					Vector3[] vertices = mesh.vertices;
					int[] triangles = mesh.triangles;
					Vector3 p0 = vertices[triangles[hitInfo.triangleIndex * 3 + 0]];
					Vector3 p1 = vertices[triangles[hitInfo.triangleIndex * 3 + 1]];
					Vector3 p2 = vertices[triangles[hitInfo.triangleIndex * 3 + 2]];

					Vector3 norm = hitInfo.collider.transform.rotation * Vector3.Cross(p2 - p1, p0 - p1).normalized;

					new_ground = norm;

					//Debug.DrawLine(hitInfo.point, hitInfo.point + norm, Color.red, 10.2f);
				}

				//Debug.Log("DIFF: " + Vector3.Dot(Vector3.up, new_ground));

				if (Vector3.Dot(Vector3.up, new_ground) > 0.5f)
                {
					isSliding = false;
                }

				if (Vector3.Dot(groundNormal, new_ground) > 0.25)
                {
						groundNormal = new_ground;
						framesGrounded++;

						if (!isOnGround)
						{
							isHammerJumping = false;
							isOnGround = true;
							canAirAttack = true;
							isBallMode = false;

							if (currentMode != PlayerModes.HURT && currentMode != PlayerModes.KILLED)
								mAnimator.Play("Land");

							acceleration.y = 0.0f;
							acceleration *= 0.95f;
							speed.y = 0.0f;
							framesAirborne = 0;
						}

						if (framesAirborne > 0 || Vector3.Distance(hitInfo.point, transform.position) > 0.01f)
						{
							if (jumpTimer < 0.05f)
							{
								transform.position = hitInfo.point;

							}
						}

				}
				else
                {
					groundNormal = Vector3.up;
					
                }


			}
			else
			{
				groundNormal = Vector3.up;
				framesAirborne++;
				framesGrounded = 0;

				//Coyote frames, also prevents state stutter.
				if (framesAirborne > 10)
				{
					if (isOnGround)
					{
						isOnGround = false;

						if(!isHammerJumping)
							mAnimator.Play("Airborne");

					}
				}

			}

			slopeAmount = Vector3.Dot(direction.normalized, groundNormal);

			if (Mathf.Abs(slopeAmount) < 0.03f)
				slopeAmount = 0.0f;
		}

		public bool canJump(bool ignoreGrounded)
        {
			if (!isOnGround && !ignoreGrounded)
				return false;


			float slopeMult = Mathf.Clamp01(1.0f + slopeAmount);

			if (slopeMult < 0.45f && isOnGround)
				return false;

			if (PlayerManager.Instance.isSmallRoom)
				return false;

			if (GameManager.Instance.playerInputDisabled || GameManager.Instance.gamePaused)
				return false;

			return true;
		}

		public void Jump(bool ignoreGrounded)
        {
			if (!isOnGround && !ignoreGrounded)
				return;

			float slopeMult = Mathf.Clamp01(1.0f + slopeAmount);

			if (slopeMult < 0.45f && isOnGround)
				return;

			float jumpPower = mParam.jumpSpeed * slopeMult;

			if (getStatus().checkStatusEffect(PlayerStatusFX.Tired))
				jumpPower *= 0.65f;


			//if (mChara == PlayableCharacter.Cream)
			//isBallMode = true;

			acceleration *= 0.8f;
			acceleration.y = jumpPower;
			mAnimator.Play("Jump");
			mAnimator.Play("Mouth_Jump");
			getStatus().currentStamina -= jumpStaimaCost;
			getStatus().clampValues();

			//if(mChara == PlayableCharacter.Amy)
			mVoice.playVoiceDelayed(Random.Range(0.05f, 0.1f), mVoice.jumping);

			spawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_basicJump, transform.position);
			jumpTimer = mParam.jump_hangTime;
		}

		public void hammerJump()
        {
			if (!isOnGround)
				return;

			if (isAttacking)
				return;

			float slopeMult = Mathf.Clamp01(1.0f + slopeAmount);

			if (slopeMult < 0.45f && isOnGround)
				return;

			float jumpPower = mParam.jumpSpeed * slopeMult;

			//acceleration *= 0.8f;
			acceleration.y = jumpPower * 1.9f;
			mAnimator.Play("HammerJump");
			isHammerJumping = true;
			getStatus().currentStamina -= hammerJumpStaimaCost;
			getStatus().clampValues();
			GameManager.Instance.controllerRumble(0.5f, 1.0f, 1.0f);
			mAnimator.Play("Mouth_Jump");
			mVoice.playVoiceDelayed(Random.Range(0.05f, 0.1f), mVoice.altJumping);
			spawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_basicJump, transform.position);
			spawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_pikoHammerJump, transform.position + transform.forward + Vector3.up * 0.2f);
			jumpTimer = mParam.jump_hangTime;
		}

		public void groundAttack()
        {

			if (isAttacking)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			acceleration = Vector3.zero;

			mAnimator.Play("Attack");
			isAttacking = true;
			getStatus().currentStamina -= hammerAttackStaminaCost;
			getStatus().clampValues();
			attackTimer = 0.6f;

			if (mAmyHammer)
			{
				mAmyHammer.enableHurtbox();
				mAmyHammer.hammerDamage.damageAmount = getStatus().baseMeleeDamage;
			}

			if (fx_hammerTrail)
				fx_hammerTrail.enableFX();

			mAnimator.Play("Mouth_Jump");
			mVoice.playVoiceDelayed(Random.Range(0.05f, 0.1f), mVoice.groundAttack, true);


			//Homing
			if(areaDetector.closestEnemy)
            {
				float dst = Vector3.Distance(transform.position + Vector3.up * 0.5f, areaDetector.closestEnemy.transform.position);

				if (dst < 4.0f)
                {
					Vector3 dir = Helper.getDirectionTo(transform.position, areaDetector.closestEnemy.transform.position);
					dir.y = 0;

					setAngleInstantly(dir);
					acceleration.z = dst * 2.0f;

				}
            }
		}

		public void runningGroundAttack()
        {
			if (isAttacking)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			//acceleration = Vector3.zero;

			mAnimator.Play("Mouth_Jump");
			mAnimator.Play("RunningGroundAttack");
			isHammerSpin = true;
			isAttacking = true;
			getStatus().currentStamina -= hammerAttackStaminaCost;
			getStatus().clampValues();
			attackTimer = 0.6f;


			if (mAmyHammer)
			{
				mAmyHammer.enableHurtbox();
				mAmyHammer.hammerDamage.damageAmount = getStatus().baseMeleeDamage * 0.75f;
			}

			if (fx_hammerTrail)
				fx_hammerTrail.enableFX();

			mVoice.playVoiceDelayed(Random.Range(0.05f, 0.1f), mVoice.groundAttack, true);
			hammerJumpCharge = 0.0f;

			//Homing
			if (areaDetector.closestEnemy)
			{
				float dst = Vector3.Distance(transform.position + Vector3.up * 0.5f, areaDetector.closestEnemy.transform.position);
				float vertDist = areaDetector.closestEnemy.transform.position.y - transform.position.y;


				if (dst < 4.0f && vertDist < 1.0f)
				{
					Vector3 dir = Helper.getHorizontalDirectionTo(transform.position, areaDetector.closestEnemy.transform.position);
					dir.y = 0;

					setAngleInstantly(dir);
					//acceleration.z = dst * 2.0f;

					if(areaDetector.closestEnemy.transform.position.y - transform.position.y > 0.8f)
                    {

						float slopeMult = Mathf.Clamp01(1.0f + slopeAmount);

						if (slopeMult < 0.45f && isOnGround)
							return;

						float jumpPower = mParam.jumpSpeed * slopeMult;

						if (getStatus().checkStatusEffect(PlayerStatusFX.Tired))
							jumpPower *= 0.65f;

						acceleration.y = jumpPower * 2;
						jumpTimer = mParam.jump_hangTime;
					}

				}
			}
		}

		public void airHammerAttack()
        {
			if (isOnGround)
				return;

			if (isAttacking)
				return;


			mAnimator.Play("AirAttack");
			isHammerSpin = true;
			isAttacking = true;
			canAirAttack = false;
			attackTimer = 0.6f;


			if (mAmyHammer)
			{
				mAmyHammer.enableHurtbox();
				mAmyHammer.hammerDamage.damageAmount = getStatus().baseMeleeDamage;
			}

			if (fx_hammerTrail)
				fx_hammerTrail.enableFX();

			mAnimator.Play("Mouth_Jump");
			mVoice.playVoiceDelayed(Random.Range(0.05f, 0.1f), mVoice.airAttack, true);
			//spawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_basicJump, transform.position);
			//spawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_pikoHammerJump, transform.position + transform.forward + Vector3.up * 0.2f);
			//jumpTimer = mParam.jump_hangTime;
			acceleration.y = 3.0f;
			acceleration.z = 9.5f;

			//Homing
			if (areaDetector.closestEnemy)
			{
				float dst = Vector3.Distance(transform.position + Vector3.up * 0.5f, areaDetector.closestEnemy.transform.position);
				float vertDist = areaDetector.transform.position.y - transform.position.y;



				if (dst < 4.0f && dst > 1.0f)
				{
					Vector3 dir = Helper.getDirectionTo(transform.position, areaDetector.closestEnemy.transform.position);
					dir.y = 0.0f;

					setAngleInstantly(dir.normalized);
					acceleration.z = dst * 4.0f;
					acceleration.y = vertDist * 4.0f;

				}
			}
		}

		public void updateHoming()
        {

			if (!isAttacking)
				return;


			if (areaDetector.closestEnemy)
			{
				float dst = Vector3.Distance(transform.position + Vector3.up * 0.5f, areaDetector.closestEnemy.transform.position);

				if (dst < 4.0f && dst > 1.0f)
				{
					Vector3 dir = Helper.getDirectionTo(transform.position, areaDetector.closestEnemy.transform.position);

					
					//acceleration.z = 16.0f;
					//setVelocityDirectly(dir * (dst * 8.0f));
					acceleration = transform.rotation * (dir * 8.0f);

					dir.y = 0.0f;
					setAngleInstantly(dir.normalized);
				}
			}
		}

		public void checkForInteract()
		{
			if (GameManager.Instance.playerInputDisabled)
			{
				clearActivatible();
				return;
			}

			if (GameManager.Instance.gamePaused)
			{
				clearActivatible();
				return;
			}

			if (interactTimeout > 0.0f)
			{
				interactTimeout -= Time.deltaTime;
				return;
			}

			if (!isOnGround)
				return;

			if (acceleration.z > 0.5f)
				return;


			if (Input.GetButtonDown("Action"))
			{
				if(areaDetector.closestActivatible)
                {
					Activatible a = areaDetector.closestActivatible;

					a.Activate(this);
					interactTimeout = 1.0f;
					UIManager.Instance.contextButton.clearActionText();

					if (a.turnAroundPlayer)
					{
						Vector3 targetDir = Helper.getDirectionTo(transform.position + Vector3.up * 0.5f, a.transform.position);
						targetDir.y = 0;

						direction = targetDir;
					}
				}
			}

		}

		public void clearActivatible(float timeOut = 0.5f)
        {
			interactTimeout = timeOut;
			UIManager.Instance.contextButton.clearActionText();


			if (areaDetector.nearbyActivatibles == null)
				return;

			areaDetector.closestActivatible = null;
			areaDetector.nearbyActivatibles.Clear();

        }

		public void checkForHammerJump()
        {
			if (GameManager.Instance.playerInputDisabled || GameManager.Instance.gamePaused)
				return;

			if (acceleration.z < 5.3f)
			{
				hammerJumpCharge = 0.0f;
				return;
			}

			if(!isOnGround)
				hammerJumpCharge = 0.0f;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			//Get this poor girl some rest jeez...
			if (getStatus().checkStatusEffect(PlayerStatusFX.Tired))
				return;

			if(Input.GetButton("Attack") && hammerJumpCharge < 1.0f)
            {
				hammerJumpCharge += Time.deltaTime * 3.0f;
			}
			else if(!Input.GetButton("Attack") && hammerJumpCharge > 0.9f)
            {
				hammerJump();
				hammerJumpCharge = 0.0f;
			}

		}

		public void checkForGroundAttack()
        {
			if (GameManager.Instance.playerInputDisabled || GameManager.Instance.gamePaused)
				return;

			if (acceleration.magnitude > 1.0f || !isOnGround)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			if (Input.GetButtonDown("Attack") )
				groundAttack();
        }

		public void checkForRunningGroundAttack()
        {
			if (GameManager.Instance.playerInputDisabled || GameManager.Instance.gamePaused)
				return;

			if (acceleration.magnitude < 1.0f || !isOnGround)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			if (!Input.GetButton("Attack") && hammerJumpCharge > 0.01f && hammerJumpCharge < 0.9f)
			{
				runningGroundAttack();
				hammerJumpCharge = 0.0f;
			}
		}

		public void checkForAirAttack()
        {
			if (GameManager.Instance.playerInputDisabled || GameManager.Instance.gamePaused)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			if (!canAirAttack)
				return;

			//Get this poor girl some rest jeez...
			if (getStatus().checkStatusEffect(PlayerStatusFX.Tired))
				return;

			//if (isAttacking && !isOnGround)
			//updateHoming();

			if (Input.GetButtonDown("Attack"))
				airHammerAttack();
		}

		public void checkForLadder(Ladder l)
        {

			if (currentMode != PlayerModes.NORMAL && currentMode != PlayerModes.SPRING && currentMode != PlayerModes.SWIMMING)
				return;

			changeCurrentMode(PlayerModes.LADDER);
			modeLadder.startClimbing(l);
		}

		public void checkForJump()
        {


			if (Input.GetButtonDown("Jump") && canJump(false))
				Jump(false);

			if (!Input.GetButton("Jump") && !isHammerJumping && framesAirborne > 5)
				jumpTimer = 0.0f;

			if (jumpTimer > 0.0f)
				jumpTimer -= Time.deltaTime;
		}

		public void checkForFlying()
        {
			if (GameManager.Instance.playerInputDisabled || GameManager.Instance.gamePaused)
				return;

			if (getAltitudeFromGround() > mParam.height && mChara == PlayableCharacter.Cream)
			{
				if (Input.GetButtonDown("Jump"))
				{
					//mVoice.playVoiceDelayed(Random.Range(0.05f, 0.1f), mVoice.jumping);
					
					changeCurrentMode(PlayerModes.FLY);
				}
			}
		}


		public void checkForSlingshot()
        {
			if (GameManager.Instance.playerInputDisabled || GameManager.Instance.gamePaused)
				return;

			if (Input.GetAxis("Shoot") < 0.5f)
				return;

			if (PlayerManager.Instance.isSmallRoom)
				return;

			if (!PlayerManager.Instance.hasSlingshot)
				return;

			if (speed.magnitude > 5.3f)
				return;

			if (!isOnGround)
				return;

			changeCurrentMode(PlayerModes.SLINGSHOT);
        }

		public void checkStickPower()
		{


			if (stickTimeout > 0.0f)
            {
				stickTimeout -= Time.deltaTime;
				return;
			}

			stickPower = 0.0f;


			if (GameManager.Instance.playerInputDisabled)
				return;

			float h = InputFunctions.getLeftAnalogX();
			float v = InputFunctions.getLeftAnalogY();

			if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
				return;

			stickAngle =  Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1.0f);
			stickPower = stickAngle.magnitude;

			//Debug.Log(stickPower);

			Vector3 targetDirection = stickAngle;
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

			//Vector3 camAngle = GameManager.Instance.mainCamera.transform.forward;
			Vector3 camAngle = Camera.main.transform.forward;
			camAngle.y = 0;
			camAngle.Normalize();

			Vector3 mDir = (targetRotation * camAngle) * (targetDirection.magnitude);

			//mPlayer.acceleration.z += ((mDir * mPlayer.mParam.forwardAccel) * Time.deltaTime);

			float slopePenalty = Mathf.Clamp(slopeAmount, 0, 2.0f);

			prev_direction = direction;
			direction = mDir.normalized;

			float turningFactor = (1 - Vector3.Dot(direction.normalized, prev_direction.normalized)) * Helper.AngleDir(direction.normalized, prev_direction.normalized, transform.up);

			turningFactor *= 16.0f;

			if (Mathf.Abs(turningFactor) > 0.01f)
				leanAmount += turningFactor * 1.5f;

			leanAmount = Mathf.Clamp(leanAmount, -1.0f, 1.0f);

			//if(Mathf.Abs(turningFactor) > 0.5f)
				//Debug.Log("TURNING: " + turningFactor);

			//Debug.Log(Vector3.Dot(direction, prev_direction));

			float dirChange = Mathf.Clamp01(Vector3.Dot(direction, prev_direction));

			

			float forward_accel = (targetDirection.magnitude * mParam.forwardAccel);
			forward_accel += (slopeAmount * mParam.forwardAccel);

			if (getStatus().checkStatusEffect(PlayerStatusFX.Tired))
				forward_accel *= 0.48f;

			//forward_accel *= dirChange;

			if(isOnGround)
				acceleration.z *= dirChange;

			acceleration.z += forward_accel * Time.deltaTime;

			if (PlayerManager.Instance.isSmallRoom)
				acceleration.z = Mathf.Clamp(acceleration.z, 0, 1.5f);
		}

		public void applyFriction()
		{

			Vector3 mAccel = acceleration;

			float friction = (mParam.groundFriction * speed.magnitude) * Time.fixedDeltaTime;

			if (!isOnGround)
				friction = (mParam.airResistance * speed.magnitude) * Time.fixedDeltaTime;

			if (stickPower < 0.01f)
				friction *= 6.0f;

			if (mAccel.z > friction)
				mAccel.z -= friction;

			if (mAccel.z < -friction)
				mAccel.z += friction;

			if (mAccel.x > friction)
				mAccel.x -= friction;

			if (mAccel.x < -friction)
				mAccel.x += friction;


			//if (mAccel.z < (friction * 0.5f) && mAccel.z < -(friction * 0.5f))
			//	mAccel.z = 0;

			if (Mathf.Abs(mAccel.z) < 0.01f)
				mAccel.z = 0.0f;

			if (Mathf.Abs(mAccel.x) < 0.01f)
				mAccel.x = 0.0f;

			//speed += (-groundNormal * (mAccel.z * 0.25f)) * Time.deltaTime;

			if (checkFallOffCeiling() && isOnGround)
            {
				//mAnimator.Play("Airborne");
				//groundNormal = Vector3.up;
				//isOnGround = false;
            }

			if (checkFallOffWall() && isOnGround)
			{
				//mAnimator.Play("Airborne");
				//groundNormal = Vector3.up;
				//isOnGround = false;
			}

			if (Mathf.Abs(slopeAmount) > 0.6f)
			{
				float slide = slopeAmount * mParam.slopeResistance * Time.fixedDeltaTime;

				//Debug.Log("slide: " + slide);

				if (checkFallOffWall())
					slide *= 3.5f;


				if(Mathf.Abs(slide) > 0.03f)
                {
					mAccel.z += slide;
                }
			}

			mAccel.y -= (0.1f * mAccel.z) * Time.fixedDeltaTime;

			//speed.y -= 0.5f * Time.deltaTime;

			acceleration = mAccel;

			
		}

		public void checkForwardWall()
        {
			Vector3 start = transform.position + WorldToPlayerSpace(Vector3.up * 0.5f);
			Vector3 end = start + (transform.forward * 0.35f);

			Debug.DrawLine(start, end, Color.magenta);

			if(Physics.Linecast(start,end,mColMask))
            {
				acceleration.z *= 0.5f;
            }
        }

		public void updatePosition()
		{
			
			speed = Vector3.Lerp(speed, WorldToPlayerSpace(acceleration), Time.deltaTime * 8.0f);
			mRigidBody.velocity = speed + platformVelocity;

			platformVelocity = Vector3.Lerp(platformVelocity, Vector3.zero, Time.deltaTime * 8.0f);

			adjustStepVolume();
			checkForwardWall();
		}

		public void setVelocityDirectly(Vector3 spd)
        {
			mRigidBody.velocity = spd;
        }

		public void clearSpeed()
        {
			speed = Vector3.zero;
			mRigidBody.velocity = speed;
        }

		public void clearAccel()
        {
			acceleration = Vector3.zero;
        }

		public void updateRotation()
		{
			updateYRotation();
			updateXZRotation();
		}

		void updateYRotation()
        {
			//Quaternion dirRot = Quaternion.LookRotation(direction.normalized, Vector3.up);
			//transform.rotation = Quaternion.Lerp(transform.rotation, dirRot, Time.deltaTime * 10.0f);
		}

		void adjustStepVolume()
        {
			float footVolume =  Mathf.Abs(acceleration.z) / 5.0f;
			footVolume = Mathf.Clamp01(footVolume);
			
			if (!isOnGround)
				footVolume = 1.0f;

			fx_footsteps.globalVolume = footVolume;

		}

		void updateXZRotation()
		{
			float time = 0.1f;

			if (!isOnGround)
				time = Time.deltaTime * 8.0f;



			Quaternion dirRot = Quaternion.LookRotation(direction.normalized, Vector3.up);

			Quaternion slopeRot = Quaternion.FromToRotation(Vector3.up, groundNormal) * dirRot;

			transform.rotation = Quaternion.Lerp(transform.rotation, slopeRot, time);
		}

		public void setAngleInstantly(Vector3 dir)
        {
			direction = dir;


			Quaternion dirRot = Quaternion.LookRotation(direction.normalized, Vector3.up);
			Quaternion slopeRot = Quaternion.FromToRotation(Vector3.up, groundNormal) * dirRot;

			transform.rotation = slopeRot;
		}

		public void setStickTimeout(float time)
        {
			stickAngle = Vector3.zero;
			stickPower = 0.0f;

			stickTimeout = time;
        }

		public Vector3 WorldToPlayerSpace(Vector3 input)
        {
			return transform.rotation * input;
        }

		public Vector3 PlayerToWorldSpace(Vector3 input)
        {
			return Quaternion.Inverse(transform.rotation) * input;
        }

		void averageGroundNormal()
        {

			Vector3 total = Vector3.zero;

			for(int i = 0; i < 4; i++)
            {
				Vector3 start = transform.position + WorldToPlayerSpace(Vector3.up * 0.5f);
				Vector3 end = transform.position - WorldToPlayerSpace(Vector3.up * 0.05f);

				Vector3 offset = Quaternion.Euler(0, 90 * i, 0) * transform.forward;

			}
        }

		Vector3 getNormal(Vector3 start, Vector3 end)
        {

			RaycastHit hitInfo = new RaycastHit();

			if (Physics.Linecast(start, end, out hitInfo, mColMask))
			{
				return hitInfo.normal;
			}

			return Vector3.up;
		}

		public GameObject spawnFX(GameObject prefab, Vector3 position, bool parent = false)
		{
			GameObject inst = GameObject.Instantiate(prefab);
			inst.transform.position = position;

			if (parent)
				inst.transform.SetParent(transform);

			return inst;
		}

		public string dbg_getModeString()
        {

			switch(currentMode)
            {
				case PlayerModes.NORMAL:
					return "NORMAL";
				case PlayerModes.SPRING:
					return "SPRING";
				case PlayerModes.RAIL:
					return "RAIL";
				case PlayerModes.SWIMMING:
					return "SWIMMING";
				case PlayerModes.FLY:
					return "FLY";
				case PlayerModes.LISTENING:
					return "LISTENING";
				case PlayerModes.SLINGSHOT:
					return "SLINGSHOT";
				case PlayerModes.LADDER:
					return "LADDER";
				case PlayerModes.CUTSCENE:
					return "CUTSCENE";
				case PlayerModes.RUBBING:
					return "RUBBING";
				case PlayerModes.HURT:
					return "HURT";
				case PlayerModes.KILLED:
					return "KILLED";
				case PlayerModes.FIRSTPERSON:
					return "FIRSTPERSON";
				case PlayerModes.DEBUG_MOVE:
					return "DEBUG_MOVE";
			}

			return "SORRY NOTHING";
		}
	}

}
