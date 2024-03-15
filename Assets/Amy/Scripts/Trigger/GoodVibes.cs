using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class GoodVibes : MonoBehaviour
	{

		public float moodHealingPerSecond = 0.1f;
		public float range = 16.0f;


		List<Player> mPlayerList;

	    // Start is called before the first frame update
	    void Start()
	    {
			mPlayerList = new List<Player>();

		}
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }


    }
}
