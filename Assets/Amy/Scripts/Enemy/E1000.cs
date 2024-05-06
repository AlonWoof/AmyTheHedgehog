using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MEC;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public enum E1000Mode
    {
		Stand,
		Patrol,
		Search,
		Pursuit,
		Damage,
		Killed
    }

	public class E1000 : MonoBehaviour
	{

		public Enemy mEnemy;
		public Vector3 mDirection;
		public NavMeshAgent mAgent;
		public Animator mAnimator;
		public E1000Voice voice;

		public Transform eye;

		public EnemyAlertPhase alertPhase;
		public CoroutineHandle currentTask;

		public Vector3 lastKnownPosition;

		public bool isAiming = false;
		public Vector3 aimPosition;

		public float moveSpeed = 3.0f;
		public float sightRange = 16.0f;
		public float sightFOV = 60.0f;

		public float shootTimer = 0.0f;

		public bool hasPatrolRoute = false;
		public List<Waypoint> patrolRoute;

		public GameObject gunProjectile;
		public GameObject deathExplosionFX;
		public GameObject damageHitFX;

		public Transform leftGun;
		public Transform rightGun;

		public Transform bodyNode;

		E1000Mode lastMode;
		E1000Mode currentMode;

		public float minShootTime = 1.0f;
		public float maxShootTime = 3.0f;

		float minVoiceTime = 15.0f;
		float maxVoiceTime = 30.0f;

		float timeTilVoiceLine = 15.0f;

		// Start is called before the first frame update
		void Start()
	    {
			getAllComponents();

			mDirection = transform.forward;

			if (hasPatrolRoute)
			{
				changeMode(E1000Mode.Patrol);
			}
			else
			{
				changeMode(E1000Mode.Stand);
			}
		}

		void getAllComponents()
        {
			mAgent = GetComponentInChildren<NavMeshAgent>();
			mEnemy = GetComponentInChildren<Enemy>();
			mAnimator = GetComponentInChildren<Animator>();

			if(!eye)
            {
				GameObject inst = new GameObject("eye");
				inst.transform.SetParent(transform);
				inst.transform.position = transform.position + Vector3.up;
				inst.transform.rotation = transform.rotation;

				eye = inst.transform;
            }

			mAgent.updateRotation = false;
			mAgent.speed = moveSpeed;
			mAgent.angularSpeed = 1000.0f;
		}

		void turnTowardsPosition(Vector3 lookPos)
        {
			Vector3 dir = Helper.getDirectionTo(transform.position, lookPos);
			dir.y = 0;

			dir.Normalize();

			mDirection = dir;
        }

		void fireGuns()
        {
			Vector3 drift = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));

			GameObject bullet = GameObject.Instantiate(gunProjectile);

			bullet.transform.position = leftGun.transform.position;
			bullet.transform.rotation = Quaternion.LookRotation(Helper.getDirectionTo(leftGun.transform.position, aimPosition + drift));

			bullet = GameObject.Instantiate(gunProjectile);

			drift = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));

			bullet.transform.position = rightGun.transform.position;
			bullet.transform.rotation = Quaternion.LookRotation(Helper.getDirectionTo(leftGun.transform.position, aimPosition + drift));

		}

		public void burstShot(int amt)
        {
			Timing.RunCoroutine(doBurstShot(amt).CancelWith(gameObject), gameObject);
			
        }

		public IEnumerator<float> doBurstShot(int amt)
        {

			for (int i = 0; i < amt; i++)
			{
				fireGuns();
				yield return Timing.WaitForSeconds(0.05f);
			}

        }

		// Update is called once per frame
		void Update()
	    {
			Animate();


			switch(currentMode)
            {
				case E1000Mode.Patrol:
					updatePatrol();
					break;
				case E1000Mode.Stand:
					updateStanding();
					break;
				case E1000Mode.Pursuit:
					updatePursuit();
					break;
			}

			updateState();


		}

        private void OnDrawGizmos()
        {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(aimPosition, 0.2f);
        }

        public void OnDamage()
        {
			if (mEnemy.isDead)
				return;

			if (mEnemy.mutekiTimer > 0.0)
				return;

			changeMode(E1000Mode.Damage);

        }

		public void OnKilled()
        {
			changeMode(E1000Mode.Killed);
		}

		void changeMode(E1000Mode newMode)
        {
			if (currentMode == newMode)
				return;

			Timing.KillCoroutines(gameObject);

			lastMode = currentMode;
			currentMode = newMode;
			mAgent.enabled = false;

			switch(currentMode)
            {
				case E1000Mode.Stand:

					break;
				case E1000Mode.Patrol:
					startPatrolRoute();
					break;
				case E1000Mode.Pursuit:
					startPursuit();
					break;

				case E1000Mode.Damage:
					currentTask = Timing.RunCoroutine(damageRoutine().CancelWith(gameObject), gameObject);
					break;

				case E1000Mode.Killed:
					currentTask = Timing.RunCoroutine(destroyedRoutine().CancelWith(gameObject), gameObject);
					break;
			}
        }

		void updateStanding()
		{
			if (!currentTask.IsRunning)
			{
				//startPatrolRoute();
			}

			if (checkForPlayer())
				changeMode(E1000Mode.Pursuit);
		}

		void updatePatrol()
        {

			if(!currentTask.IsRunning)
            {
				startPatrolRoute();
            }

			if (checkForPlayer())
				changeMode(E1000Mode.Pursuit);
		}

		void updatePursuit()
        {
			if (!currentTask.IsRunning)
			{
				startPursuit();
			}


			if (timeTilVoiceLine > 0.0f)
				timeTilVoiceLine -= Time.deltaTime;

			if (timeTilVoiceLine < 0.0f)
            {
				timeTilVoiceLine = Random.Range(minVoiceTime, maxVoiceTime);
				voice.playRandomClip(voice.PursuitVoice, false);
			}
		}

		void updateState()
        {
			if(alertPhase == EnemyAlertPhase.CLEAR)
            {

				if(currentMode == E1000Mode.Pursuit)
                {
					if(hasPatrolRoute)
                    {
						changeMode(E1000Mode.Patrol);
                    }
					else
                    {
						changeMode(E1000Mode.Stand);
                    }
                }
            }

			if(alertPhase == EnemyAlertPhase.ALERT)
            {
				Player pl = PlayerManager.Instance.getPlayer();

				if(!pl)
                {
					alertPhase = EnemyAlertPhase.CLEAR;
                }

				if(currentMode == E1000Mode.Patrol || currentMode == E1000Mode.Stand || currentMode == E1000Mode.Search)
                {
					changeMode(E1000Mode.Pursuit);
                }

				lastKnownPosition = pl.transform.position + Vector3.up * 0.5f;

				if (pl.currentMode == PlayerModes.KILLED)
					alertPhase = EnemyAlertPhase.CLEAR;
            }

			if(alertPhase == EnemyAlertPhase.EVASION)
            {

            }
        }

		void Animate()
        {
			Vector3 velo = mAgent.velocity;

			
			if(velo.magnitude > 0.1f)
            {
				mDirection = velo.normalized;
            }

			mDirection.y = 0;
			mDirection.Normalize();

			mAnimator.SetFloat("velocity", (Mathf.Clamp(velo.magnitude, 0.0001f, velo.magnitude)));
			mAnimator.SetBool("isAiming", isAiming);

			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mDirection), Time.deltaTime * 8.0f);

        }
		

		void startPatrolRoute()
        {
			mAgent.speed = moveSpeed;
			currentTask = Timing.RunCoroutine(patrolRoutine().CancelWith(gameObject), gameObject);
		}

		void startPursuit()
		{
			currentTask = Timing.RunCoroutine(pursuitRoutine().CancelWith(gameObject), gameObject);
			turnTowardsPosition(lastKnownPosition);

			if (alertPhase != EnemyAlertPhase.ALERT)
			{
				alertPhase = EnemyAlertPhase.ALERT;

				voice.playRandomClip(voice.SpottedVoice, true);
			}
		}

		void playDamageAnimation()
        {
			currentTask = Timing.RunCoroutine(damageRoutine().CancelWith(gameObject));
        }

		void playDestroyedAnimation()
		{
			currentTask = Timing.RunCoroutine(destroyedRoutine().CancelWith(gameObject));
		}

		IEnumerator<float> patrolRoutine()
        {


			//Loop endlessly til we find something.

			bool foundSomething = false;

			while (!foundSomething)
			{
				foreach (Waypoint w in patrolRoute)
				{

					CoroutineHandle moveHandle = Timing.RunCoroutine(moveToPoint(w.transform.position, 0.25f).CancelWith(gameObject), gameObject);

					while (moveHandle.IsRunning)
					{
						yield return 0f;
					}

					if(w.waitTime > 0.1f)
                    {
						CoroutineHandle waitLookHandle = Timing.RunCoroutine(waitAndLook(w.waitTime).CancelWith(gameObject), gameObject);

						while(waitLookHandle.IsRunning)
                        {
							yield return 0f;

                        }
                    }

					yield return 0f;
				}

				yield return 0f;
			}

        }
		IEnumerator<float> damageRoutine()
        {
			mAnimator.Play("Damage");
			spawnFX(damageHitFX, bodyNode, 0.0f);

			mAgent.enabled = false;
			mEnemy.mutekiTimer = 2.5f;

			float knockBackPower = 16.0f;

			while(mEnemy.mutekiTimer > 0.0f)
            {
				transform.position += ((-transform.forward * mEnemy.mutekiTimer) * (Time.deltaTime * knockBackPower));
				yield return 0f;
            }

			lastKnownPosition = PlayerManager.Instance.mPlayerInstance.hipBoneTransform.position;
			turnTowardsPosition(lastKnownPosition);

			changeMode(E1000Mode.Pursuit);
        }
		IEnumerator<float> destroyedRoutine()
        {
			
			mAnimator.Play("Destroy");
			mAgent.enabled = false;

			yield return Timing.WaitForSeconds(2.0f);

			//Explode fx
			spawnFX(deathExplosionFX, bodyNode, 0.1f);
			Destroy(gameObject);
		}
		IEnumerator<float> pursuitRoutine()
		{
			float phaseTime = 15.0f;

			bool playerVisible = checkForPlayer();
			Player pl = PlayerManager.Instance.mPlayerInstance;
			shootTimer = minShootTime;

			notifyFriends();

			while (phaseTime > 0.0f)
			{
				playerVisible = checkForPlayer();
				float dst = Helper.horizontalDistance(transform.position, lastKnownPosition);

				if (!playerVisible)
				{
					isAiming = false;
					mAgent.enabled = true;
					mAgent.acceleration = 100.0f;
					mAgent.speed = 6.0f;
					shootTimer = Random.Range(minShootTime, maxShootTime);
					mAgent.SetDestination(lastKnownPosition);

					if (mAgent.velocity.magnitude < 0.1f)
					{
						turnTowardsPosition(pl.transform.position);
					}
				}
				else
				{
					Vector3 aimAhead = pl.speed * 0.5f;

					aimPosition = Vector3.Lerp(aimPosition, lastKnownPosition + aimAhead, 0.12f);
					mAgent.enabled = false;
					turnTowardsPosition(pl.transform.position);
					phaseTime = 15.0f;
					isAiming = true;
				}

				shootTimer -= Time.deltaTime;
				phaseTime -= Time.deltaTime;

				if (shootTimer < 0.0f)
				{
					burstShot(3);
					shootTimer = Random.Range(minShootTime, maxShootTime);
				}

				yield return 0f;
			}

			changeMode(lastMode);
		}

		void spawnFX(GameObject fx, Transform root, float zoffs = 0.0f)
        {
			GameObject inst = GameObject.Instantiate(fx);
			inst.transform.position = root.transform.position + Vector3.up * zoffs;
			inst.transform.rotation = root.transform.rotation;
		}

		void notifyFriends()
        {
			foreach(E1000 robot in FindObjectsOfType<E1000>())
            {
				if (robot == this)
					continue;

				float maxDist = 32.0f;

				if (Vector3.Distance(robot.transform.position, transform.position) > maxDist)
					continue;

				robot.lastKnownPosition = lastKnownPosition;
				robot.alertPhase = EnemyAlertPhase.ALERT;
				robot.changeMode(E1000Mode.Pursuit);
            }
        }

        bool checkForPlayer()
        {
			float range = sightRange;
			float fov = sightFOV;

			Vector3 start = eye.transform.position;
			Vector3 end = (start + eye.transform.forward * range);

			Debug.DrawLine(start, end);

			if(alertPhase == EnemyAlertPhase.ALERT)
            {
				range *= 3.0f;
				fov *= 0.95f;
            }

			if(alertPhase == EnemyAlertPhase.EVASION)
            {
				range *= 1.1f;
				fov *= 1.5f;
            }

			Player pl = PlayerManager.Instance.mPlayerInstance;

			if (EnemyHelpers.isPlayerVisible(pl, eye, fov, range))
			{
				lastKnownPosition = pl.transform.position + Vector3.up * 0.5f;
				return true;
			}

			return false;

		}


		IEnumerator<float> moveToPoint(Vector3 pos, float threshold)
        {
			mAgent.enabled = true;
			mAgent.SetDestination(pos);

			float dst = Helper.horizontalDistance(pos, transform.position);

			while(dst > threshold)
            {
				mAgent.SetDestination(pos);
				dst = Helper.horizontalDistance(pos, transform.position);
				yield return 0f;
			}
        }

		IEnumerator<float> waitAndLook(float time)
        {
			mAgent.enabled = false;

			mAnimator.Play("LookAround");

			yield return Timing.WaitForSeconds(time);

			mAnimator.Play("Idle");

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
	}
}
