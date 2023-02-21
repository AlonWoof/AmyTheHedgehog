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

		public PlayerStatus baseStats;
	}

	public enum PlayerModes
    {
		NORMAL,
		SPRING,
		RAIL,
		LISTENING,
		CUTSCENE,
		KILLED
    }

	public class Player : MonoBehaviour
	{

		public Rigidbody mRigidBody;
		public Animator mAnimator;
		public FootstepFX fx_footsteps;
		public ThirdPersonCamera tpc;
		public PlayerVoice mVoice;

		public Vector3 speed = Vector3.zero;
		public Vector3 acceleration = Vector3.zero;
		public Vector3 direction = Vector3.zero;
		public Vector3 prev_direction = Vector3.zero;
		public Vector3 groundNormal = Vector3.up;
		public float slopeAmount = 0.0f;

		public Vector3 stickAngle;
		public float stickPower;

		public PlayerParameters mParam;

		public LayerMask mColMask;

		public bool isOnGround = false;
		public bool isSliding = false;
		public int framesAirborne = 0;

		public float stickTimeout = 0.0f;
		public float jumpTimer = 1.0f;

		//Modes
		public PlayerModes currentMode;
		public PlayerModes lastMode;

		public PlayerBasicMove modeBasic;
		public PlayerSpringBounce modeSpring;
		public PlayerRail modeRail;

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

			//WetnessDirtynessProxy wetdirt = inst.AddComponent<WetnessDirtynessProxy>();
			//inst.AddComponent<ActorOpacity>();


			//newPlayer.fx_waterWadingFX = GameObject.Instantiate(GameManager.Instance.systemData.RES_WaterWadingFX);

			newPlayer.direction = dir;

			foreach (Transform t in inst.GetComponentsInChildren<Transform>())
			{
				if (t.gameObject.name.ToLower().Contains("hitbox"))
				{
					//PlayerHitbox hb = t.gameObject.AddComponent<PlayerHitbox>();
					//hb.mPlayer = newPlayer;
				}
			}

			//newPlayer.mChara = chara;
			newPlayer.mParam = cpar;
			//newPlayer.playerHeight = amy_height;

			return newPlayer;
		}

		public void addAllModes()
        {
			modeBasic = gameObject.AddComponent<PlayerBasicMove>();
			modeSpring = gameObject.AddComponent<PlayerSpringBounce>();
			modeRail = gameObject.AddComponent<PlayerRail>();
		}

		public void disableAllModes()
        {
			modeBasic.enabled = false;
			modeSpring.enabled = false;
			modeRail.enabled = false;
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

				case PlayerModes.RAIL:
					modeRail.enabled = true;
					break;
            }
        }

		public void getBaseComponents()
		{
			mRigidBody = GetComponent<Rigidbody>();
			mAnimator = GetComponent<Animator>();
			mVoice = GetComponent<PlayerVoice>();
		}

		private void Awake()
		{
			addAllModes();
			getBaseComponents();
			mColMask = LayerMask.GetMask("Collision");

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

			if (runAnimSpeed < 0.5f && runAnimSpeed > 0.0f)
				runAnimSpeed = 0.5f;

			if (runAnimSpeed > -0.5f && runAnimSpeed < 0.0f)
				runAnimSpeed = -0.5f;



			mAnimator.SetFloat("y_accel", speed.y);
			mAnimator.SetFloat("z_accel", runAnimProgress);
			mAnimator.SetFloat("run_anim_speed", runAnimSpeed);

			
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

		public void CalcVerticalVelocity()
		{
			float gravityMult = mParam.gravityMult;

			float verticalVelocity = acceleration.y;

			if (isOnGround && !checkFallOffWall() && !checkFallOffCeiling())
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
		}

		bool checkFallOffWall()
        {


			if(Mathf.Abs(slopeAmount) > 0.65f && acceleration.z < 3.0f)
            {

				return true;
            }

			return false;
        }

		bool checkFallOffCeiling()
		{
			return false;

			if (groundNormal.y < 0.75f && acceleration.z < 3.0f)
			{
				return true;
			}

			return false;
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

				if (Vector3.Dot(groundNormal, new_ground) > 0.25f)
                {
					groundNormal = new_ground;

					if (!isOnGround)
					{
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
						mAnimator.Play("Airborne");

					}
				}

			}

			slopeAmount = Vector3.Dot(direction.normalized, groundNormal);

			if (Mathf.Abs(slopeAmount) < 0.03f)
				slopeAmount = 0.0f;
		}

		public void Jump(bool ignoreGrounded)
        {
			if (!isOnGround && !ignoreGrounded)
				return;

			float slopeMult = Mathf.Clamp01(1.0f + slopeAmount);

			if (slopeMult < 0.45f)
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

		public void checkForJump()
        {
			if (Input.GetButtonDown("Jump"))
				Jump(false);

			if (!Input.GetButton("Jump"))
				jumpTimer = 0.0f;

			if (jumpTimer > 0.0f)
				jumpTimer -= Time.deltaTime;
		}

		public void checkStickPower()
		{

			if(stickTimeout > 0.0f)
            {
				stickTimeout -= Time.deltaTime;
				return;
			}

			stickPower = 0.0f;

			float h = InputFunctions.getLeftAnalogX();
			float v = InputFunctions.getLeftAnalogY();

			if (Mathf.Abs(h) < 0.1f && Mathf.Abs(v) < 0.1f)
				return;

			stickAngle =  Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1.0f);
			stickPower = stickAngle.magnitude;

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


			//Debug.Log(Vector3.Dot(direction, prev_direction));

			float dirChange = Mathf.Clamp01(Vector3.Dot(direction, prev_direction));

			float forward_accel = (targetDirection.magnitude * mParam.forwardAccel);
			forward_accel += (slopeAmount * mParam.forwardAccel);

			//forward_accel *= dirChange;

			if(isOnGround)
				acceleration.z *= dirChange;

			acceleration.z += forward_accel * Time.deltaTime;
			

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
				groundNormal = Vector3.up;
				isOnGround = false;
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
				acceleration *= 0.5f;
            }
        }

		public void updatePosition()
		{
			speed = Vector3.Lerp(speed, WorldToPlayerSpace(acceleration), Time.deltaTime * 8.0f);
			mRigidBody.velocity = speed;

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
