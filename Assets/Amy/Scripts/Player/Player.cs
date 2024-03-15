using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		//public CharacterPhysicsData jiggleData;
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
		DEBUG_MOVE
    }

	public class Player : MonoBehaviour
	{

		public Rigidbody mRigidBody;
		public Animator mAnimator;
		public FootstepFX fx_footsteps;
		public WeaponTrailFX fx_hammerTrail;
		public ThirdPersonCamera tpc;
		public PlayerVoice mVoice;

		public PlayerParameters mParam;
		public PlayableCharacter mChara;

		public Vector3 speed = Vector3.zero;
		public Vector3 acceleration = Vector3.zero;
		public Vector3 platformVelocity = Vector3.zero;
		public Vector3 direction = Vector3.zero;
		public Vector3 prev_direction = Vector3.zero;
		public Vector3 groundNormal = Vector3.up;
		public float slopeAmount = 0.0f;

		public Vector3 stickAngle;
		public float stickPower;

		

		public LayerMask mColMask;

		public bool isOnGround = false;
		public bool isSliding = false;
		public bool isHammerJumping = false;
		public bool isAttacking = false;
		public float mutekiTimer = 0.0f;
		public float attackTimer = 0.0f;
		public int framesAirborne = 0;

		public float stickTimeout = 0.0f;
		public float jumpTimer = 1.0f;

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
		public PlayerHurt modeHurt;
		public PlayerKilled modeKilled;
		public PlayerDebugMove modeDebug;
		

		public PlayerAreaDetector areaDetector;

		public List<Hitbox> hitBoxes;

		public static Player Spawn(Vector3 pos, Vector3 dir, PlayableCharacter chara = PlayableCharacter.Amy)
		{
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
			col.height = amy_height * 0.5f;
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

			if (!anim)
				inst.AddComponent<Animator>();

			anim.runtimeAnimatorController = cpar.ingameAnimator;
			anim.applyRootMotion = false;

			//CharacterPhysics jiggles = inst.AddComponent<CharacterPhysics>();
			//jiggles.mData = cpar.jiggleData;

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


			//newPlayer.fx_waterWadingFX = GameObject.Instantiate(GameManager.Instance.systemData.RES_WaterWadingFX);

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

			//Status effects
			foreach(StatusEffect s in PlayerManager.Instance.gameObject.GetComponentsInChildren<StatusEffect>())
            {
				s.setPlayer(newPlayer);
            }

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
			modeHurt = gameObject.AddComponent<PlayerHurt>();
			modeKilled = gameObject.AddComponent<PlayerKilled>();
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
			modeHurt.enabled = false;
			modeKilled.enabled = false;
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

				case PlayerModes.HURT:
					modeHurt.enabled = true;
					break;

				case PlayerModes.KILLED:
					modeKilled.enabled = true;
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


		void doLeanAnimation()
        {
			float angle = (20.0f * leanAmount) * Mathf.Clamp01(Mathf.Abs(acceleration.z) / 6.0f);

			leanAmount = Mathf.Lerp(leanAmount, 0.0f, Time.deltaTime * 3.0f);

			hipBoneTransform.rotation = hipBoneTransform.rotation * Quaternion.Euler(0, 0, angle);

			rThighBoneTransform.rotation = rThighBoneTransform.rotation * Quaternion.Euler(0, 0, angle);
			lThighBoneTransform.rotation = lThighBoneTransform.rotation * Quaternion.Euler(0, 0, angle);

			headBoneTransform.rotation = headBoneTransform.rotation * Quaternion.Euler(0, Mathf.Clamp(-angle * 5.0f, -40, 40), 0);
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
					result = PlayerManager.Instance.AmyStatus;
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

		}

		// Update is called once per frame
		void Update()
		{

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

				if(attackTimer <= 0.0f)
                {
					isAttacking = false;
					attackTimer = 0.0f;

					if (fx_hammerTrail)
						fx_hammerTrail.disableFX();
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

			mAnimator.SetFloat("y_accel", speed.y);
			mAnimator.SetFloat("z_accel", runAnimProgress);
			mAnimator.SetFloat("run_anim_speed", runAnimSpeed);

			debugControls();
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

		public bool takeDamage(Damage dmg, float multiplier = 1.0f)
        {
			if (mutekiTimer > 0.0f && dmg.damageType != DamageType.Crush)
				return false;

			int rings = PlayerManager.Instance.getRings();

			//Rings protect you from ouchies.
			if (rings > 0)
				dmg.damageAmount *= 0.5f;

			float force = 16.0f;

			if (rings > 0)
				force *= 0.75f;

			if (currentMode == PlayerModes.NORMAL)
            {
				PlayerStatus pstats = getStatus();



				if (currentMode == PlayerModes.NORMAL && force > 1.0f)
				{
					modeHurt.setKnockBack(dmg.transform.position, force);
				}


				pstats.currentHealth -= (dmg.damageAmount * multiplier);

				if (rings > 0)
					pstats.currentHealth = Mathf.Clamp(pstats.currentHealth, 1, pstats.maxHealth);

				if (pstats.currentHealth > 0)
					changeCurrentMode(PlayerModes.HURT);
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

			mutekiTimer = 0.5f;
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
        }

        private void LateUpdate()
        {
			doLeanAnimation();

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

			//Debug.Log("DOT OF WALL: " + Vector3.Dot(groundNormal, Vector3.up));


			if(Vector3.Dot(groundNormal, Vector3.up) < 0.65f && acceleration.z < 3.0f)
            {

				return true;
            }

			return false;
        }

		bool checkFallOffCeiling()
		{


			if (Vector3.Dot(groundNormal, Vector3.down) > 0.75f && acceleration.z < 3.0f)
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

		void getGroundNormal()
		{
			Vector3 start = transform.position + WorldToPlayerSpace(Vector3.up * 0.5f);
			Vector3 end = transform.position - WorldToPlayerSpace(Vector3.up * 0.2f);

			//start += mRigidBody.velocity * Time.deltaTime;
			//end += mRigidBody.velocity * Time.deltaTime;

			Debug.DrawLine(start, end, Color.green, 10.0f);


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

					Debug.DrawLine(hitInfo.point, hitInfo.point + norm, Color.red, 10.2f);
				}

				//Debug.Log("DIFF: " + Vector3.Dot(Vector3.up, new_ground));

				if (Vector3.Dot(Vector3.up, new_ground) > 0.5f)
                {
					isSliding = false;
                }

				if (Vector3.Dot(groundNormal, new_ground) > 0.25)
                {
						groundNormal = new_ground;

						if (!isOnGround)
						{
							isHammerJumping = false;
							isOnGround = true;
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

			if (PlayerManager.Instance.isHubRoom)
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

			acceleration *= 0.8f;
			acceleration.y = jumpPower;
			mAnimator.Play("Jump");
			//mAnimator.Play("Mouth_Jumping");
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
			//mAnimator.Play("Mouth_Jumping");
			mVoice.playVoiceDelayed(Random.Range(0.05f, 0.1f), mVoice.altJumping);
			spawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_basicJump, transform.position);
			spawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_pikoHammerJump, transform.position + transform.forward + Vector3.up * 0.2f);
			jumpTimer = mParam.jump_hangTime;
		}

		public void groundAttack()
        {

			if (isAttacking)
				return;

			if (PlayerManager.Instance.isHubRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			acceleration = Vector3.zero;

			mAnimator.Play("Attack");
			isAttacking = true;
			attackTimer = 0.6f;

			if(fx_hammerTrail)
				fx_hammerTrail.enableFX();

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

		public void airHammerAttack()
        {
			if (isOnGround)
				return;

			if (isAttacking)
				return;

			mAnimator.CrossFade("AirAttack",0.2f);
			isAttacking = true;
			attackTimer = 0.6f;

			if (fx_hammerTrail)
				fx_hammerTrail.enableFX();

			//mAnimator.Play("Mouth_Jumping");
			mVoice.playVoiceDelayed(Random.Range(0.05f, 0.1f), mVoice.airAttack, true);
			//spawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_basicJump, transform.position);
			//spawnFX(GameManager.Instance.systemData.RES_AmyPlayerFX.fx_pikoHammerJump, transform.position + transform.forward + Vector3.up * 0.2f);
			//jumpTimer = mParam.jump_hangTime;

			//Homing
			if (areaDetector.closestEnemy)
			{
				float dst = Vector3.Distance(transform.position + Vector3.up * 0.5f, areaDetector.closestEnemy.transform.position);

				if (dst < 8.0f)
				{
					Vector3 dir = Helper.getDirectionTo(transform.position, areaDetector.closestEnemy.transform.position);
					dir.y = 0;

					setAngleInstantly(dir);
					acceleration.z = dst * 4.0f;

				}
			}
		}

		public void checkForHammerJump()
        {
			if (acceleration.z < 5.3f)
				return;

			if (PlayerManager.Instance.isHubRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			if (Input.GetButtonDown("Attack"))
				hammerJump();
		}

		public void checkForGroundAttack()
        {
			if (acceleration.magnitude > 1.0f || !isOnGround)
				return;

			if (PlayerManager.Instance.isHubRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

			if (Input.GetButtonDown("Attack"))
				groundAttack();
        }

		public void checkForAirAttack()
        {
			if (PlayerManager.Instance.isHubRoom)
				return;

			if (!PlayerManager.Instance.hasHammer)
				return;

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

			if (!Input.GetButton("Jump") && !isHammerJumping)
				jumpTimer = 0.0f;

			if (jumpTimer > 0.0f)
				jumpTimer -= Time.deltaTime;
		}

		public void checkForFlying()
        {
			if (getAltitudeFromGround() > 0.4f && mChara == PlayableCharacter.Cream)
			{
				if (Input.GetButtonDown("Jump"))
				{
					changeCurrentMode(PlayerModes.FLY);
				}
			}
		}


		public void checkForSlingshot()
        {
			if (Input.GetAxis("Shoot") < 0.5f)
				return;

			if (PlayerManager.Instance.isHubRoom)
				return;

			if (!PlayerManager.Instance.hasSlingshot)
				return;

			if (speed.magnitude > 0.1f)
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


			Debug.Log(stickPower);

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

			if(Mathf.Abs(turningFactor) > 0.5f)
				Debug.Log("TURNING: " + turningFactor);

			//Debug.Log(Vector3.Dot(direction, prev_direction));

			float dirChange = Mathf.Clamp01(Vector3.Dot(direction, prev_direction));

			

			float forward_accel = (targetDirection.magnitude * mParam.forwardAccel);
			forward_accel += (slopeAmount * mParam.forwardAccel);

			//forward_accel *= dirChange;

			if(isOnGround)
				acceleration.z *= dirChange;

			acceleration.z += forward_accel * Time.deltaTime;

			if (PlayerManager.Instance.isHubRoom)
				acceleration.z = Mathf.Clamp(acceleration.z, 0, 1.2f);
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
	}

}
