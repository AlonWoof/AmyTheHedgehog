using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Ladder : MonoBehaviour
	{
		public Transform startPos;
		public Transform endPos;

		public float offset = 0.1f;

	    // Start is called before the first frame update
		[ExecuteInEditMode]
	    void Awake()
	    {

		}

        private void OnValidate()
        {
			if (!startPos)
			{
				GameObject inst = new GameObject("LADDER_START");
				inst.transform.SetParent(transform);
				inst.transform.position = transform.position;
				startPos = inst.transform;
			}

			if (!endPos)
			{
				GameObject inst = new GameObject("LADDER_END");
				inst.transform.SetParent(transform);
				inst.transform.position = transform.position;
				endPos = inst.transform;
			}
		}

        private void OnDrawGizmos()
        {
			Gizmos.DrawLine(startPos.position, endPos.position);
        }

        // Update is called once per frame
        void Update()
	    {
	        
	    }

        private void OnTriggerEnter(Collider other)
        {
			Player mPlayer = other.gameObject.GetComponentInChildren<Player>();


			if (mPlayer)
            {
				mPlayer.checkForLadder(this);

			}
        }
    }
}
