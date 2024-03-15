using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerAreaDetector : PlayerMode
	{
		public List<NPC> nearbyNPCs;
		public List<Enemy> nearbyEnemies;
		public List<Vibes> nearbyVibes;

		public Enemy closestEnemy = null;
		public NPC closestNPC = null;

		public float detectRadius = 20.0f;

		float timeTilUpdate = 3.0f;

		// Start is called before the first frame update
		void Start()
	    {
			nearbyEnemies = new List<Enemy>();
			nearbyNPCs = new List<NPC>();
			nearbyVibes = new List<Vibes>();
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
				refreshClosestEnemy();
				refreshClosestNPC();
			}

			if(nearbyVibes.Count > 0)
            {
				foreach(Vibes v in nearbyVibes)
                {
					PlayerStatus pstats = mPlayer.getStatus();
					pstats.currentMood += (v.moodPerSecond * Time.deltaTime);
                }
            }
	    }

		void refreshLists()
        {
			nearbyNPCs.Clear();
			nearbyEnemies.Clear();
			nearbyVibes.Clear();

			foreach(NPC n in FindObjectsOfType<NPC>())
            {
				if(Vector3.Distance(transform.position, n.transform.position) < detectRadius)
                {
					nearbyNPCs.Add(n);
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

		}

		public int getNearbyEnemyCount()
        {
			return nearbyEnemies.Count;

		}

		public int getNearbyNPCCount()
        {
			return nearbyNPCs.Count;
		}

		public int getNearbyVibesCount()
		{
			return nearbyVibes.Count;
		}

		public int getNearbyActorCount()
        {
			return getNearbyNPCCount() + getNearbyEnemyCount();
        }


		public void refreshClosestNPC()
        {
			closestNPC = null;
			float bestDist = 16.0f;

			foreach (NPC e in nearbyNPCs)
			{
				float dst = Vector3.Distance(e.transform.position, transform.position + (Vector3.up * 0.5f));

				if (dst < bestDist)
				{
					bestDist = dst;
					closestNPC = e;
				}
			}
		}

		public void refreshClosestEnemy()
        {
			closestEnemy = null;
			float bestDist = 16.0f;

			foreach(Enemy e in nearbyEnemies)
            {
				float dst = Vector3.Distance(e.transform.position, transform.position + (Vector3.up * 0.5f));

				if(dst < bestDist)
                {
					bestDist = dst;
					closestEnemy = e;
                }
            }
        }
	}
}
