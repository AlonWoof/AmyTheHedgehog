using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class MusicChanger : MonoBehaviour
	{
		public BGMData targetBGM;
		public float crossFadeTime = 0.0f;
		public void changeBGM()
        {
			MusicManager.Instance.changeSongs(targetBGM, crossFadeTime);
        }

		public void killMusic()
        {
			MusicManager.Instance.killBGM(true);
        }
	}
}
