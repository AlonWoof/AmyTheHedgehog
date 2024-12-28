using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CommonEventFunctions : MonoBehaviour
	{

		public void event_triggerReturnSpell()
        {
			PlayerManager.Instance.getPlayer().startWarp("WarpCenter", 7);
		}
	}
}
