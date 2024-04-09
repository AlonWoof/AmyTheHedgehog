using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TimedSceneLoad : MonoBehaviour
	{

		public string sceneToLoad;
		public float timeTilLoad = 10.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			if (timeTilLoad > 0.0f)
				timeTilLoad -= Time.unscaledDeltaTime;
			else
            {
				GameManager.Instance.loadScene(sceneToLoad);
				enabled = false;
				//Application.Quit();
            }
			
	    }
	}
}
