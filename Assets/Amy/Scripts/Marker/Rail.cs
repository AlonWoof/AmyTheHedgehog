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

		[Range(0.01f,1.0f)]
		public float testProgress = 0.01f;

		[Range(-1.0f, 64.0f)]
		public float testDistance = 0.0f;

		public float checkForPlayerTimeout = 0.03f;

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

			//Gizmos.DrawSphere(getPosOnRail(testProgress), 0.5f);

			Gizmos.DrawSphere(getPosOnRailFromDistance(testDistance), 0.5f);
		}

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {

		}

        private void FixedUpdate()
        {
			doPlayerCheckOnRail();

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

		void doPlayerCheckOnRail()
        {
			if(checkForPlayerTimeout > 0.0f)
            {
				checkForPlayerTimeout -= Time.deltaTime;
				return;
			}


			for (int i = 0; i < points.Length - 1; i++)
			{

				Vector3 start = points[i].transform.position;
				Vector3 end = points[i + 1].transform.position;

				RaycastHit hitInfo = new RaycastHit();

				if(Physics.SphereCast(start, 1.0f, Helper.getDirectionTo(start,end), out hitInfo, Vector3.Distance(start,end)))
                {
					if(hitInfo.collider.GetComponent<Player>())
                    {
						

						Player pl = hitInfo.collider.GetComponent<Player>();

						if (pl.currentMode == PlayerModes.NORMAL)
						{

							float d = Vector3.Distance(start, hitInfo.point);
							float railDist = getDistanceFromNode(i) + d;
							float railProgress = (railDist / totalDistance);

							Vector3 pos = getPosOnRailFromDistance(getDistanceFromNode(i) + d);

							if (railProgress < 0.8f)
							{
								pl.changeCurrentMode(PlayerModes.RAIL);
								pl.modeRail.MountRail(this, pos);
								pl.modeRail.railDistance = getDistanceFromNode(i) + d;
							}

						}

					}
                }

			}

			checkForPlayerTimeout = 0.03f;
		}

		public Vector3 getPosOnRail(float progress)
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

		public Vector3 getPosOnRailFromDistance(float dist)
		{
			float d = Mathf.Clamp(dist, 0.001f, totalDistance - 0.001f);
			//float d = dist;
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


			return Vector3.Lerp(points[node - 1].transform.position, points[node].transform.position, 1 - subProgress);

		}

		public RailNode getNodeFromDistance(float dist)
        {
			float d = Mathf.Clamp(dist, 0.001f, totalDistance);
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

		public int getNodeIndexFromDistance(float dist)
        {
			float d = Mathf.Clamp(dist, 0.001f, totalDistance);
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

		public float getDistanceFromNode(int index)
        {
			float dist = 0.0f;

			for (int i = 0; i < index; i++)
			{
				float ld = Vector3.Distance(points[i].transform.position, points[i + 1].transform.position);
				points[i].distToNextNode = ld;
				dist += ld;
			}

			return dist;
		}

	}
}
