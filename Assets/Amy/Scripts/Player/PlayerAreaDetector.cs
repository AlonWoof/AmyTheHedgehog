using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerAreaDetector : PlayerMode
	{
		public List<Enemy> nearbyEnemies;
		public List<Vibes> nearbyVibes;
		public List<Activatible> nearbyActivatibles;

		public Enemy closestEnemy = null;
		public Activatible closestActivatible = null;
		public LookPoint closestLookPoint = null;


		public int visibleNPCCount = 0;

		public float detectRadius = 20.0f;
		public float lookRadius = 5.0f;

		float timeTilUpdate = 3.0f;

		// Start is called before the first frame update
		void Start()
	    {
			getBaseComponents();
			nearbyEnemies = new List<Enemy>();
			nearbyVibes = new List<Vibes>();
			nearbyActivatibles = new List<Activatible>();
			refreshLists();
		}
	
	    // Update is called once per frame
	    void Update()
	    {
	        
			if(timeTilUpdate <= 0.0f)
            {
				timeTilUpdate = 3.0f;
				refreshLists();
			}
			else
            {
				timeTilUpdate -= Time.deltaTime;
            }

			if(Time.frameCount % 30 == 0)
            {

			}

			refreshClosestEnemy();
			refreshClosestActivatible();
			refreshLookatTarget();

			updateVibes();

		}

		void refreshLists()
        {
			nearbyActivatibles.Clear();
			nearbyEnemies.Clear();
			nearbyVibes.Clear();

			foreach(Activatible a in FindObjectsOfType<Activatible>())
            {
				if(Vector3.Distance(transform.position, a.transform.position) < detectRadius)
                {
					nearbyActivatibles.Add(a);
                }
            }

			foreach (Enemy n in FindObjectsOfType<Enemy>())
			{
				if (Vector3.Distance(transform.position, n.transform.position) < detectRadius)
				{
					nearbyEnemies.Add(n);
				}
			}

			foreach (Vibes v in FindObjectsOfType<Vibes>())
			{
				if (Vector3.Distance(transform.position, v.transform.position) < v.range)
				{
					nearbyVibes.Add(v);
				}
			}

			refreshClosestLookPoint();
			
			
			isVisibleToNPC();
		}

		public int getNearbyEnemyCount()
        {
			return nearbyEnemies.Count;

		}

		public int getNearbyActivatiblesCount()
        {
			return nearbyActivatibles.Count;
		}

		public int getNearbyVibesCount()
		{
			return nearbyVibes.Count;
		}

		public int getNearbyActorCount()
        {
			return getNearbyEnemyCount();
        }

		public void updateVibes()
        {
			PlayerStatus pStats = mPlayer.getStatus();

			pStats.currentVibes = 0;

			foreach(Vibes v in nearbyVibes)
            {
				if(Vector3.Distance(v.transform.position, mPlayer.transform.position + Vector3.up * 0.5f) < v.range)
                {
					pStats.setVibe(v.vibeFlags);
                }
            }
        }

		public void refreshLookatTarget()
        {
			if(closestLookPoint)
            {
				mPlayer.lookAt(closestLookPoint.transform.position);
			}

			if (closestActivatible)
			{
				mPlayer.lookAt(closestActivatible.transform.position);
			}

			if (closestEnemy)
			{
				if (Vector3.Distance(transform.position + (Vector3.up * (mPlayer.mParam.height * 0.75f)), closestEnemy.transform.position) > lookRadius * 1.5f)
				{
					mPlayer.lookAt(closestEnemy.transform.position);
				}
			}

		}

		public void refreshClosestLookPoint()
        {
			closestLookPoint = null;
			float bestDist = 16.0f;

			foreach(LookPoint lp in FindObjectsOfType<LookPoint>())
            {
				float dst = Vector3.Distance(lp.transform.position, transform.position + (Vector3.up * 0.5f));

				if (dst < lp.lookDistance)
				{
					if (dst < bestDist)
					{
						bestDist = dst;
						closestLookPoint = lp;

					}
				}
			}
        }

		public void refreshClosestActivatible()
        {
			closestActivatible = null;
			float bestDist = 16.0f;

			foreach (Activatible e in nearbyActivatibles)
			{
				float dst = Vector3.Distance(e.transform.position, transform.position + (Vector3.up * 0.5f));

				if (dst < bestDist && e.canActivate(mPlayer))
				{
					bestDist = dst;
					closestActivatible = e;

					Debug.Log(closestActivatible.interactionLabel);
				}
			}

			if (closestActivatible)
			{
				mPlayer.lookAt(closestActivatible.transform.position);

				if(mPlayer.interactTimeout <= 0.0f)
					UIManager.Instance.contextButton.setActionText(closestActivatible.interactionLabel);
			}
		}

		public bool isVisibleToNPC(float range = 30.0f)
        {
			visibleNPCCount = 0;
			bool ret = false;

			foreach (NPC n in FindObjectsOfType<NPC>())
            {
				Vector3 dir = Helper.getHorizontalDirectionTo(n.transform.position, transform.position);

				if (Vector3.Dot(n.transform.forward, dir) > 0.8f)
				{
					float dst = Vector3.Distance(n.transform.position, transform.position);

					if (dst < range)
					{
						Vector3 start = n.headNode.transform.position;
						Vector3 end = transform.position + Vector3.up * 0.5f;

						RaycastHit hitInfo = new RaycastHit();

						if (Physics.Linecast(start, end, out hitInfo))
						{
							if (hitInfo.collider.gameObject == gameObject)
							{
								ret = true;
								visibleNPCCount++;
							}
						}
					}
				}
            }

			return ret;
        }

		public void refreshClosestEnemy()
        {
			closestEnemy = null;
			float bestDist = 16.0f;
			float bestAngle = 0.5f;

			foreach(Enemy e in nearbyEnemies)
            {

				if (e == null)
					continue;

				float dst = Vector3.Distance(e.transform.position, transform.position + (Vector3.up * 0.5f));
				float ang_cam = Vector3.Dot(Camera.main.transform.forward, Helper.getDirectionTo(Camera.main.transform.position, e.transform.position).normalized);
				float ang_player = Vector3.Dot(transform.forward, Helper.getDirectionTo(transform.position, e.transform.position).normalized);
				float ang = (ang_cam + ang_cam + ang_player) * 0.33333f;


				if (ang < 0.5f)
					dst = 100.0f;

				if (ang > 0.5f && ang < 0.75f)
					dst = 15.0f + dst * 0.0625f;

				if(dst < bestDist)
                {
					bestDist = dst;
					bestAngle = ang;
					closestEnemy = e;
                }

            }




			//Debug.Log("BEST ANGLE: " + bestAngle);



		}
	}
}
