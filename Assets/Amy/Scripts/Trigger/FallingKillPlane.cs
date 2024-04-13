using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class FallingKillPlane : MonoBehaviour
	{

        private void OnTriggerEnter(Collider other)
        {
            Player pl = other.GetComponentInChildren<Player>();

            if(pl)
            {
                if(pl.currentMode != PlayerModes.KILLED)
                {
                    pl.modeKilled.deathType = PlayerKilled.DeathType.Falling;
                    pl.changeCurrentMode(PlayerModes.KILLED);
                }
            }
        }
    }
}
