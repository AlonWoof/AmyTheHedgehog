using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class HUD_DebugInfo : MonoBehaviour
	{
		public Text dbg_text;
		public Player player;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

        private void LateUpdate()
        {
			setText();

		}

		void setText()
		{


			if (!player)
			{
				player = PlayerManager.Instance.getPlayer();
				dbg_text.text = "";
				return;
			}

			PlayerStatus pstats = player.getStatus();

			string dbgstr = "DEBUG INFO\n\n";
			dbgstr += "Health: " + pstats.currentHealth + " / " + pstats.maxHealth + "\n";
			dbgstr += "Stamina: " + pstats.currentStamina + " / " + pstats.maxStamina + "\n";
			dbgstr += "Mood: " + pstats.currentMood + " / " + pstats.maxMood + "\n\n";

			dbgstr += "Mode: " + player.dbg_getModeString() + "\n";
			dbgstr += "Accel \n X: " + player.acceleration.x +
				"\n Y: " + player.acceleration.y +
				"\n Z: " + player.acceleration.z + "\n\n";

			dbgstr += "Dir \n X: " + player.direction.x +
				"\n Y: " + player.direction.y +
				"\n Z: " + player.direction.z + "\n\n";

			dbgstr += "Speed \n X: " + player.speed.x +
				"\n Y: " + player.speed.y +
				"\n Z: " + player.speed.z + "\n\n";


			dbgstr += "isOnGround: " + player.isOnGround + "\n";
			dbgstr += "isSliding: " + player.isSliding + "\n";
			dbgstr += "isAttacking: " + player.isAttacking + "\n";
			dbgstr += "isHammerJumping: " + player.isAttacking + "\n";
			dbgstr += "mutekiTimer: " + player.mutekiTimer + "\n";
			dbgstr += "attackTimer: " + player.attackTimer + "\n";
			dbgstr += "isHammerJumping: " + player.framesAirborne + "\n";

		dbg_text.text = dbgstr;
		}
	}
}
