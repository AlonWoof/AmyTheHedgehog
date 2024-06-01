using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ItemEffect : MonoBehaviour
	{

		public Player mPlayer;
		public PlayerStatus pStats;

	    // Start is called before the first frame update
	    void Awake()
	    {
			mPlayer = PlayerManager.Instance.getPlayer();
			pStats = mPlayer.getStatus();

			transform.SetParent(mPlayer.transform);
		}
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
