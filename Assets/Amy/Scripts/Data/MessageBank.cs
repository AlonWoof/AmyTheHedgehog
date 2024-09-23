using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[System.Serializable]
	[CreateAssetMenu(fileName = "New MessageBank", menuName = "MessageBank", order = 52)]
	public class MessageBank : ScriptableObject
	{
		public SpeakerProfile profile;
		public Message[] messages;

		public Message getRandomMessage()
        {
			int rand = Random.Range(0, messages.Length);
			return messages[rand];
        }
	}
}
