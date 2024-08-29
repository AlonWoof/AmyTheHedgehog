using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Waypoint : MonoBehaviour
	{

		public int group = 0;
		public float waitTime = 0.0f;
		public float speedModifier = 1.0f; 

#if UNITY_EDITOR
		public List<Waypoint> otherNodes;

#endif

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
        {
			if (otherNodes == null)
				otherNodes = new List<Waypoint>();

			if (otherNodes.Count == 0)
			{
				otherNodes.Clear();

				foreach(Waypoint w in FindObjectsOfType<Waypoint>())
                {
					if(w.group == group)
                    {
						otherNodes.Add(w);
                    }
                }
			}

			foreach(Waypoint w in otherNodes)
            {

				foreach(Waypoint e in otherNodes)
                {
					float dst = Vector3.Distance(Helper.zeroAltitude(w.transform.position), Helper.zeroAltitude(e.transform.position));

					if (e == this)
						continue;

					if (e == w)
						continue;

					if (dst < 10.0f)
					{
						if (dst > 5.0f)
							Gizmos.color = Color.green;
						else if (dst < 0.1f)
						{
							Gizmos.color = Color.red;
							Gizmos.DrawSphere(w.transform.position, 0.4f);
						}
						else
							Gizmos.color = Color.red;

						Gizmos.DrawLine(w.transform.position, e.transform.position);
					}
				}

				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(w.transform.position, 0.1f);
            }
        }
#endif
	}
}
