using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PauseMenu : MonoBehaviour
	{

		public GameObject pauseText;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {

			if (GameManager.Instance.gamePaused && !pauseText.activeInHierarchy)
				pauseText.SetActive(true);

			if (!GameManager.Instance.gamePaused && pauseText.activeInHierarchy)
				pauseText.SetActive(false);
		}
	}
}
