using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class RailStart : MonoBehaviour
	{
		public Rail mRail;
		public Player mPlayer;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			if (!mPlayer)
				mPlayer = PlayerManager.Instance.getPlayer();
	    }

        private void OnTriggerStay(Collider other)
        {
            if(other.GetComponent<Player>() && mPlayer.currentMode == PlayerModes.NORMAL)
            {

				if (!mPlayer.isOnGround && Mathf.Abs(mPlayer.speed.y) > 0.2f && mPlayer.currentMode == PlayerModes.NORMAL)
				{
					mPlayer = PlayerManager.Instance.getPlayer();
					mPlayer.changeCurrentMode(PlayerModes.RAIL);

					mPlayer.modeRail.MountRail(mRail);
				}
			}
        }
    }
}
