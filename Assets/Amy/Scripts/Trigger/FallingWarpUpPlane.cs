using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class FallingWarpUpPlane : MonoBehaviour
	{
        public float warpYPos = 0.0f;

        private void OnTriggerEnter(Collider other)
        {
            Player pl = other.GetComponentInChildren<Player>();

            if (pl)
            {
                Vector3 pos = pl.transform.position;
                pos.y = warpYPos;

                pl.transform.position = pos;
            }
        }
    }
}
