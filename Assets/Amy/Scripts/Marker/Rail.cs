using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Rail : MonoBehaviour
	{

		public Transform[] points;
		public UnityEvent[] events;
		public GameObject start;
		public GameObject end;

		Player mPlayer;

        private void OnDrawGizmos()
        {
			for(int i = 0; i < points.Length-1; i++)
            {
				Gizmos.color = Color.red;
				Gizmos.DrawLine(points[i].transform.position, points[i + 1].transform.position);

            }
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {

		}
	}
}
