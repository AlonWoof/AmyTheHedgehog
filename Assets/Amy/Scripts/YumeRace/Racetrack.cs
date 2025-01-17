using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Racetrack : MonoBehaviour
	{

		public List<Waypoint> racetrackNodes;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void OnDrawGizmos()
        {

			if (racetrackNodes.Count < 2)
				return;

			for(int i = 0; i < racetrackNodes.Count; i++)
            {
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireSphere(racetrackNodes[i].transform.position, 3.0f);

				if(i - 1 > -1)
                {
					Gizmos.DrawLine(racetrackNodes[i - 1].transform.position, racetrackNodes[i].transform.position);
                }

				if((i + 1) == racetrackNodes.Count)
                {
					Gizmos.DrawLine(racetrackNodes[i].transform.position, racetrackNodes[0].transform.position);
				}
			}


        }
    }
}
