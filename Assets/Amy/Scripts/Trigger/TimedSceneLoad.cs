using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TimedSceneLoad : MonoBehaviour
	{

		public string sceneToLoad;
		public float timeTilLoad = 10.0f;
		bool triggered = false;

	    // Start is called before the first frame update
	    void Start()
	    {
			Timing.KillCoroutines();
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			if (timeTilLoad > 0.0f)
				timeTilLoad -= Time.unscaledDeltaTime;
			else if(!triggered)
            {

				PlayerManager.Instance.PlayerDieRespawn(PlayerKilled.DeathType.Corrupted);
				PlayerManager.Instance.exitType = ExitLevelType.NORMAL;
				//enabled = false;
				triggered = true;
				//Application.Quit();
				Time.timeScale = 1.0f;
			}
			
	    }
	}
}
