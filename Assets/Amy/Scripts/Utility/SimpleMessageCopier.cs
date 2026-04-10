using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2026 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class SimpleMessageCopier : MonoBehaviour
	{

		public Message globalMessage;

        private void OnValidate()
        {
            foreach(SimpleMessageNPC smnpc in GetComponentsInChildren<SimpleMessageNPC>())
            {
                smnpc.SimpleMessage = globalMessage;
            }
        }

    }
}
