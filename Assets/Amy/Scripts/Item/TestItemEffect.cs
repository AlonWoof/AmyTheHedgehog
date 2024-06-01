using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TestItemEffect : MonoBehaviour
	{
	    // Start is called before the first frame update
	    void Start()
	    {
			Player mPlayer = PlayerManager.Instance.getPlayer();

			//wow
			mPlayer.changeCurrentMode(PlayerModes.RUBBING);
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
