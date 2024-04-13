using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Activatible : MonoBehaviour
	{

		public float range = 5.0f;
		public float priority = 0.0f;
		public string interactionLabel = "Talk";

		public UnityEvent onActivate;
		public bool directionDependent = false;
		public bool turnAroundPlayer = true;

		public bool canActivate(Player pl)
        {
			if (directionDependent)
			{
				Vector3 dir = Helper.getDirectionTo(transform.position, pl.transform.position + Vector3.up * 0.5f);

				//Debug.Log(Vector3.Dot(-pl.transform.forward, transform.forward));

				if (Vector3.Dot(-pl.transform.forward, transform.forward) < 0.75f)
					return false;
			}

			float dist = Vector3.Distance(pl.transform.position + Vector3.up * 0.5f, transform.position);

			if (dist > range)
				return false;

			return true;
		}

		public void Activate(Player pl)
        {
			if (!canActivate(pl))
				return;

			onActivate.Invoke();
        }
	}
}
