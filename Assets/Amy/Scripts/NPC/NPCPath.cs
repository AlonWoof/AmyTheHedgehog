using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MEC;
using System.Linq;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	
	public class NPCPath : MonoBehaviour
	{

		NavMeshAgent mAgent;
		List<Waypoint> waypoints;

		List<Waypoint> previousWaypoints;


		public int maxPreviousWaypoints = 3;
		public int waypointGroup = 0;
		public float moveSpeed = 3.0f;
		public float stopDist = 0.1f;
		public float z_offset = 0.0f;
		bool moveOverride = false;

		Vector3 currentDirection = Vector3.forward;

		CoroutineHandle currentRoutine;
		Waypoint currentWaypoint;
		public Animator mAnimator;

	    // Start is called before the first frame update
	    void Start()
	    {
			currentDirection = transform.forward;
			mAgent = GetComponent<NavMeshAgent>();

			mAnimator = GetComponentInChildren<Animator>();

			buildWaypointList();
			currentRoutine = Timing.RunCoroutine(goToPointRoutine(findNextWaypoint()).CancelWith(gameObject));

			mAgent.speed = moveSpeed;
			mAgent.stoppingDistance = 0.0f;
			mAgent.updateRotation = false;
			mAgent.angularSpeed = 1000.0f;
		}
	
		void buildWaypointList()
        {
			if (waypoints == null)
				waypoints = new List<Waypoint>();

			if (previousWaypoints == null)
				previousWaypoints = new List<Waypoint>();

			waypoints.Clear();

			foreach(Waypoint w in FindObjectsOfType<Waypoint>())
            {
				if(w.group == waypointGroup)
                {
					waypoints.Add(w);
                }
            }
        }

	    // Update is called once per frame
	    void Update()
	    {
	        if(Time.frameCount % 60 == 0)
            {
				if(!currentRoutine.IsRunning)
                {
					previousWaypoints.Clear();
					currentRoutine = Timing.RunCoroutine(goToPointRoutine(findNextWaypoint()).CancelWith(gameObject));
				}

			}

			if(mAgent.velocity.magnitude > 0.25f)
            {
				Vector3 mDir = mAgent.velocity;
				currentDirection = mDir.normalized;
			}

			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentDirection), Time.deltaTime * 8.0f);

			if (mAnimator)
			{
				if(mAgent.enabled)
					mAnimator.SetFloat("velocity", mAgent.speed / mAgent.velocity.magnitude);
				else
					mAnimator.SetFloat("velocity", 0);
			}
		}

        private void LateUpdate()
        {
			snapToGround();
        }

        void snapToGround()
        {
			Vector3 start = transform.position + Vector3.up;
			Vector3 end = transform.position - Vector3.up;

			RaycastHit hitInfo = new RaycastHit();

			if(Physics.Linecast(start,end,out hitInfo, LayerMask.NameToLayer("Collision")))
            {
				transform.position = hitInfo.point + Vector3.up * z_offset;
            }
        }

		IEnumerator<float> goToPointRoutine(Waypoint w)
        {
			if (w == null)
				yield break;

			currentWaypoint = w;

			mAgent.enabled = true;
			mAgent.SetDestination(w.transform.position);

			float dist = Vector3.Distance(Helper.zeroAltitude(transform.position), Helper.zeroAltitude(w.transform.position));

			while(dist > stopDist)
            {
				dist = Vector3.Distance(Helper.zeroAltitude(transform.position), Helper.zeroAltitude(w.transform.position));

				while (isFacingPlayer() || moveOverride)
				{
					mAgent.velocity = Vector3.zero;
					mAgent.enabled = false;
					yield return 0f;
				}

				if(!mAgent.enabled)
					mAgent.enabled = true;

				mAgent.SetDestination(w.transform.position);
				mAgent.speed = moveSpeed;

				yield return 0f;
            }

			if(w.waitTime > 0.0f)
            {
				mAgent.enabled = false;
				yield return Timing.WaitForSeconds(Random.Range(w.waitTime * 0.9f, w.waitTime * 1.1f));
			}

			updatePreviousWaypointList();
			previousWaypoints.Add(w);

			currentRoutine = Timing.RunCoroutine(goToPointRoutine(findNextWaypoint()).CancelWith(gameObject));
		}

		void updatePreviousWaypointList()
        {
			//previousWaypoints = previousWaypoints.Distinct().ToList();

			if (previousWaypoints.Count > maxPreviousWaypoints)
            {
				previousWaypoints.Remove(previousWaypoints.First());
            }
			
        }

		bool isFacingPlayer()
		{
			Player pl = PlayerManager.Instance.mPlayerInstance;

			if (!pl)
				return false;

			if (Vector3.Distance(transform.position, pl.transform.position) < 1.5f)
			{
				if (Vector3.Dot(currentDirection, Helper.getDirectionTo(transform.position, pl.transform.position)) > 0.75f)
				{
					Debug.DrawLine(transform.position + Vector3.up * 0.5f, pl.transform.position + Vector3.up * 0.5f, Color.red);
					return true;

				}

				Debug.DrawLine(transform.position + Vector3.up * 0.5f, pl.transform.position + Vector3.up * 0.5f, Color.yellow);
				return false;
			}

			Debug.DrawLine(transform.position + Vector3.up * 0.5f, pl.transform.position + Vector3.up * 0.5f, Color.green);
			return false;
		}


		Waypoint findNextWaypoint(bool previousCounts = false)
        {
			List<Waypoint> candidates = new List<Waypoint>();

			foreach(Waypoint w in waypoints)
            {

				bool prev = false;

				foreach(Waypoint n in previousWaypoints)
                {
					if(w == n)
                    {
						prev = true;
                    }
                }

				if (!prev)
				{
					float dist = Vector3.Distance(Helper.zeroAltitude(w.transform.position), Helper.zeroAltitude(transform.position));

					if (currentWaypoint != w)
					{
						if (dist < 10.1f && !isBehind(w))
						{
							candidates.Add(w);
						}
					}
				}


            }

			//DEAD END
			if(candidates.Count == 0)
            {
				return null;
            }

			int rng = Random.Range(0, candidates.Count);

			return candidates[rng];
        }

		public bool isBehind(Vector3 pos)
        {
			Vector3 tDir = Helper.getHorizontalDirectionTo(transform.position, pos);

			if (Vector3.Dot(currentDirection, tDir) < -0.2f)
				return true;

			return false;
        }

		public bool isBehind(Waypoint w)
		{
			return isBehind(w.transform.position);
		}

		public void disableMovement()
        {
			moveOverride = true;
		}

		public void enableMovement()
        {
			moveOverride = false;

		}

		private void OnDrawGizmosSelected()
        {
			if (previousWaypoints == null || currentWaypoint == null)
				return;

            foreach(Waypoint w in previousWaypoints)
            {
				Gizmos.color = Color.red;
				Gizmos.DrawSphere(w.transform.position, 1.0f);
            }

			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, currentWaypoint.transform.position);
        }

    }
}
