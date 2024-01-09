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

		public float detectRadius = 20.0f;

		float timeTilUpdate = 3.0f;

		// Start is called before the first frame update
		void Start()
	    {
			nearbyEnemies = new List<Enemy>();
			nearbyNPCs = new List<NPC>();
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

	    }

		void refreshLists()
        {
			nearbyNPCs.Clear();
			nearbyEnemies.Clear();

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
		}

		public int getNearbyEnemyCount()
        {
			return nearbyEnemies.Count;

		}

		public int getNearbyNPCCount()
        {
			return nearbyNPCs.Count;
		}

		public int getNearbyActorCount()
        {
			return getNearbyNPCCount() + getNearbyEnemyCount();
        }

	}
}
