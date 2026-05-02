using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class WindVolume : MonoBehaviour
	{

		public float minForce = 32.0f;
		public float maxForce = 16.0f;

        BoxCollider col;

        private void Awake()
        {
            col = GetComponentInChildren<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Player mPlayer = other.GetComponentInChildren<Player>();

            if (mPlayer)
            {
                mPlayer.startFloating();
            }
        }

        private void OnDrawGizmos()
        {


        }


        private void OnTriggerStay(Collider other)
        {
            Player mPlayer = other.GetComponentInChildren<Player>();

            if (mPlayer)
            {

                float pPos = mPlayer.transform.position.y - col.bounds.min.y;
                float boxMax = col.bounds.max.y - col.bounds.min.y;

                float fac = Mathf.Clamp01(pPos / boxMax);

                float force = Mathf.Lerp(maxForce, minForce, fac);

                mPlayer.acceleration.y = force;

            }
        }
    }
}
