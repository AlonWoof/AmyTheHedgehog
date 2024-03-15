using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class StatusEffect : MonoBehaviour
	{
		public Player mPlayer;
		public PlayerStatus pStats;

		public float duration = 30.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

		public void setPlayer(Player p)
        {
			mPlayer = p;
			pStats = p.getStatus();

			//PlayerManager.Instance.getPlayer();
        }
	
	    // Update is called once per frame
	    void Update()
	    {

			if (mPlayer == null)
				return;

			duration -= Time.deltaTime;

			if(duration < 0.0f)
            {
				Destroy(this);
            }
			else
            {
				executeEffect();
			}
	    }

		protected virtual void executeEffect()
        {

        }
	}
}
