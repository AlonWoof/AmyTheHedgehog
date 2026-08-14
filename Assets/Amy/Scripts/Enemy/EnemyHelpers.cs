using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

//Common AI functions to make things easier.

namespace Amy
{

	public enum EnemyAlertPhase
    {
		CLEAR,
		WARNING,
		EVASION,
		ALERT
    }

	public class EnemyHelpers
	{
		public static bool isPlayerVisible(Player pl, Transform eye, float fov, float dist)
        {
			if (!pl)
				return false;

			if (Vector3.Angle(eye.forward, Helper.getDirectionTo(eye.position, pl.transform.position)) > fov)
				return false;

			//We're re-adding this thing. Yep.
			dist -= dist * PlayerManager.Instance.stealthIndex;

			Vector3 start = eye.transform.position;
			Vector3 end = pl.transform.position + Vector3.up * 0.5f;

			if (Vector3.Distance(start, end) > dist)
				return false;

			RaycastHit hitInfo = new RaycastHit();

			if (Physics.Linecast(start, end, out hitInfo))
			{
				Hitbox hit = hitInfo.collider.GetComponentInChildren<Hitbox>();

				if (hit)
				{
					if (hit.isPlayerHitbox)
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
