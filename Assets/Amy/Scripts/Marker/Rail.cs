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

		public RailNode[] points;
		public UnityEvent[] events;
		public GameObject start;
		public GameObject end;

		Player mPlayer;

		public float totalDistance;

		[Range(0.0f,1.0f)]
		public float testProgress = 0.0f;

		private void OnDrawGizmos()
        {
			for(int i = 0; i < points.Length-1; i++)
            {
				Gizmos.color = Color.red;
				Gizmos.DrawLine(points[i].transform.position, points[i + 1].transform.position);

            }
        }

        private void OnDrawGizmosSelected()
        {
			calcTotalDistance();

			Gizmos.DrawSphere(getPosOnRail(testProgress), 0.5f);
		}

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {

		}

		void calcTotalDistance()
        {

			float dist = 0.0f;

			for (int i = 0; i < points.Length - 1; i++)
			{
				float ld = Vector3.Distance(points[i].transform.position, points[i + 1].transform.position);
				points[i].distToNextNode = ld;
				dist += ld;

			}

			totalDistance = dist;
		}


		Vector3 getPosOnRail(float progress)
        {
			float d = progress * totalDistance;
			float l = d;

			int node = 0;

			float subProgress = 0.0f;

			while(d > 0.0f)
            {

				float subdist = Vector3.Distance(points[node].transform.position, points[node + 1].transform.position);

				l = d;

				if(l - subdist <= 0.0f)
                {
					subProgress = (subdist - l)/subdist;
					Debug.Log("subProgress: " + subProgress);
				}

				d -= subdist;
				node++;
            }

			return Vector3.Lerp(points[node - 1].transform.position, points[node].transform.position, 1 - subProgress);

        }

		RailNode getNodeFromDistance(float dist)
        {
			float d = dist;
			float l = d;

			int node = 0;

			float subProgress = 0.0f;

			while (d > 0.0f)
			{

				float subdist = Vector3.Distance(points[node].transform.position, points[node + 1].transform.position);

				l = d;

				if (l - subdist <= 0.0f)
				{
					subProgress = (subdist - l) / subdist;
					Debug.Log("subProgress: " + subProgress);
				}

				d -= subdist;
				node++;
			}

			return points[node];
		}

		int getNodeIndexFromDistance(float dist)
        {
			float d = dist;
			float l = d;

			int node = 0;

			float subProgress = 0.0f;

			while (d > 0.0f)
			{

				float subdist = Vector3.Distance(points[node].transform.position, points[node + 1].transform.position);

				l = d;

				if (l - subdist <= 0.0f)
				{
					subProgress = (subdist - l) / subdist;
					Debug.Log("subProgress: " + subProgress);
				}

				d -= subdist;
				node++;
			}

			return node;
		}

	}
}
