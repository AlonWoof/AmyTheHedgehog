using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class StoryFlagSetter : MonoBehaviour
	{

		public string storyFlag;

		public void setFlag()
        {
			PlayerManager.Instance.setStoryFlag(storyFlag, true);
        }

		public void unSetFlag()
        {
			PlayerManager.Instance.setStoryFlag(storyFlag, false);
		}
	}
}
