using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{

    public float stopTime = 0.0f;
    public float connectionRange = 3.0f;
    public List<Waypoint> connectedWaypoints;

    private void OnDrawGizmosSelected()
    {
        grabWaypointsInRange();

        foreach (Waypoint w in connectedWaypoints)
        {
            Gizmos.DrawCube(w.transform.position, Vector3.one * 0.1f);

            foreach (Waypoint n in w.connectedWaypoints)
            {
                Gizmos.DrawLine(n.transform.position, w.transform.position);
            }
        }
    }

    void grabWaypointsInRange()
    {
        connectedWaypoints = new List<Waypoint>();

        foreach(Waypoint w in FindObjectsOfType<Waypoint>())
        {
            if(Vector3.Distance(w.transform.position,transform.position) < connectionRange)
            {
                connectedWaypoints.Add(w);
            }
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
