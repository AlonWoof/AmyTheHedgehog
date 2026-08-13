using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public enum DebugInfoPage
    {
		None,
		PlayerInstance,
		AmyStatus,
		CreamStatus,
		InventoryInfo,
		SceneInfo,
		EventInfo,
		AIInfo,
		PageCount
    }
	public class HUD_DebugInfo : MonoBehaviour
	{
		public Text dbg_text;
		public Player player;
		public AIPlayer aiplayer;


		public int countedTriangles = 0;
		public int maxTriangles = 0;

		public CoroutineHandle countTriangleCoroutine;
		public int maxAllowedTriangles = 100000;
		public bool tooManyTris = false;

		public DebugInfoPage currentPage;
		

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }



		// Update is called once per frame
		void Update()
	    {

			if (!GameManager.Instance.debugMode)
				return;

			if(Input.GetKeyDown(KeyCode.F3))
            {
				currentPage++;

				if (currentPage == DebugInfoPage.PageCount)
					currentPage = DebugInfoPage.None;
            }
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

			if (currentPage == DebugInfoPage.None)
			{
				dbg_text.text = "";
				return;
			}

			PlayerStatus pstats = player.getStatus();

			string dbgstr = "DEBUG INFO " + (int)currentPage + "/" + ((int)DebugInfoPage.PageCount-1) + "\n\n";

			switch(currentPage)
            {
				case DebugInfoPage.PlayerInstance:
					dbgstr += getPlayerInstanceInfo();
					break;

				case DebugInfoPage.AmyStatus:
					dbgstr += getPlayerStatus(PlayableCharacter.Amy);
					break;

				case DebugInfoPage.CreamStatus:
					dbgstr += getPlayerStatus(PlayableCharacter.Cream);
					break;

				case DebugInfoPage.InventoryInfo:
					dbgstr += getInventoryInfo();
					break;

				case DebugInfoPage.EventInfo:
					dbgstr += getEventInfo();
					break;

				case DebugInfoPage.SceneInfo:
					dbgstr += getSceneInfo();
					break;

				case DebugInfoPage.AIInfo:
					dbgstr += getAIInfo();
					break;
			}

			dbg_text.text = dbgstr;
		}

		string getPlayerStatus(PlayableCharacter mChara)
        {
			PlayerStatus pstats = PlayerManager.Instance.getCharacterStatus(mChara);

			string dbgstr = "";

			switch(mChara)
            {
				case PlayableCharacter.Amy:
					dbgstr += "<color=#F6A3BBFF>AMY ";
					break;
				case PlayableCharacter.Cream:
					dbgstr += "<color=#F8E0B8FF>CREAM ";
					break;
			}

			dbgstr += "STATUS</color>\n";

			if (PlayerManager.Instance.currentCharacter != mChara)
				dbgstr += "\n(Currently Resting)\n\n";
			else
				dbgstr += "\n\n\n";

			dbgstr += "Health: " + pstats.currentHealth + " / " + pstats.maxHealth + "\n";
			dbgstr += "Stamina: " + pstats.currentStamina + " / " + pstats.maxStamina + "\n";
			//dbgstr += "Mood: " + pstats.currentMood + " / " + pstats.maxMood + "\n\n";

			dbgstr += "Status Effects: ";

			if (pstats.checkStatusEffect(PlayerStatusFX.Relaxed))
				dbgstr += "Relaxed ";

			if (pstats.checkStatusEffect(PlayerStatusFX.Scared))
				dbgstr += "Scared ";

			if (pstats.checkStatusEffect(PlayerStatusFX.Tired))
				dbgstr += "Tired ";

			if (pstats.checkStatusEffect(PlayerStatusFX.Dirty))
				dbgstr += "Dirty ";

			if (pstats.checkStatusEffect(PlayerStatusFX.Horny))
				dbgstr += "Horny ";

			if (pstats.checkStatusEffect(PlayerStatusFX.Sick))
				dbgstr += "Sick ";

			if (pstats.checkStatusEffect(PlayerStatusFX.RecentOrgasm))
				dbgstr += "RecentOrgasm ";

			dbgstr += "\n\nVibes: ";

			if(pstats.checkVibe(VibeType.Safe))
				dbgstr += "Safe ";

			if (pstats.checkVibe(VibeType.Pretty))
				dbgstr += "Pretty ";

			if (pstats.checkVibe(VibeType.Fun))
				dbgstr += "Fun ";

			if (pstats.checkVibe(VibeType.Dark))
				dbgstr += "Dark ";

			if (pstats.checkVibe(VibeType.Scary))
				dbgstr += "Scary ";

			if (pstats.checkVibe(VibeType.Dirty))
				dbgstr += "Dirty ";

			dbgstr += "\nScared Time Left: " + pstats.scaredTimeLeft;
			dbgstr += "\nSick Time Left: " + pstats.sickTimeLeft;
			dbgstr += "\nRecent Orgasm Time Left: " + pstats.recentOrgasmTimeLeft;
			dbgstr += "\nGood Food Time Left: " + pstats.goodFoodTimeLeft;
			dbgstr += "\nDirtyness: " + pstats.dirtiness;

			dbgstr += "\nTime Spent Resting: " + pstats.timeSpentResting;
			//dbgstr += "\n Good Food Time Left: " + pstats.goodFoodTimeLeft;

			return dbgstr;
		}



		string getPlayerInstanceInfo()
        {

			player = PlayerManager.Instance.getPlayer();
			PlayerStatus pstats = player.getStatus();

			string dbgstr = "";

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

			dbgstr += "Idle Timer: " + player.modeBasic.idleCounter + "\n\n";


			dbgstr += "isOnGround: " + player.isOnGround + "\n";
			dbgstr += "isSliding: " + player.isSliding + "\n";
			dbgstr += "isAttacking: " + player.isAttacking + "\n";
			dbgstr += "isHammerJumping: " + player.isAttacking + "\n";
			dbgstr += "mutekiTimer: " + player.mutekiTimer + "\n";
			dbgstr += "attackTimer: " + player.attackTimer + "\n";
			dbgstr += "framesAirborne: " + player.framesAirborne + "\n";
			dbgstr += "framesGrounded: " + player.framesGrounded + "\n";
			dbgstr += "hammerJumpCharge: " + player.hammerJumpCharge + "\n";
			dbgstr += "canAirAttack: " + player.canAirAttack + "\n";
			dbgstr += "waterDepth: " + player.getWaterDepth() + "\n";

			dbgstr += "\n NPCs that can see me: " + player.areaDetector.visibleNPCCount;

			return dbgstr;
		}

		string getInventoryInfo()
        {
			player = PlayerManager.Instance.getPlayer();
			PlayerStatus pstats = player.getStatus();

			string dbgstr = "INVENTORY: \n\n";

			foreach(ItemData i in pstats.items)
            {
				dbgstr += "   " + i.name + "\n";

            }

			return dbgstr;
		}

		string getEventInfo()
		{
			string dbgstr = "";

			dbgstr += "EVENT INFO: \n";

			dbgstr += "\n";

			for(int i = 0; i < 5; i++)
            {
				dbgstr += "\ndailyNumber" + i + ": " + PlayerManager.Instance.todayEvents.dayNumbers[i];
            }

			dbgstr += "\n\nuniverseNumber: " + PlayerManager.Instance.universeNumber;

			if (PlayerManager.Instance.currentCharacter != PlayableCharacter.Amy)
				dbgstr += "\n\nAmy Location: " + PlayerManager.getPlayerNPCLocationString(PlayerManager.Instance.AmyStatus.npcLocation);

			if (PlayerManager.Instance.currentCharacter != PlayableCharacter.Cream)
				dbgstr += "\n\nCream Location: " + PlayerManager.getPlayerNPCLocationString(PlayerManager.Instance.CreamStatus.npcLocation);

			return dbgstr;
		}

		string getSceneInfo()
        {

			if (!countTriangleCoroutine.IsRunning)
				countTriangleCoroutine = Timing.RunCoroutine(countTriangles(), Segment.RealtimeUpdate, gameObject);

			string dbgstr = "";

			dbgstr += "SCENE INFO: \n";

			dbgstr += "\nTriangle Budget: ";

			if (tooManyTris)
				dbgstr += "<color=#FF0000FF>";

			dbgstr += countedTriangles;

			if (tooManyTris)
			{
				dbgstr += "</color>";

				//artificial lag to hammer the point home
				countTriangles();
			}


			dbgstr += " / " + maxAllowedTriangles;

			dbgstr += "\nMax Triangle: " + maxTriangles;

			return dbgstr;
		}


		IEnumerator<float> countTriangles()
        {
			int tris = 0;

			float dst = GameManager.Instance.mainCamera.farClipPlane;

			int iter = 0;

			foreach (MeshFilter r in FindObjectsOfType<MeshFilter>())
			{
				//if(Vector3.Distance(GameManager.Instance.mainCamera.transform.position, r.transform.position) < dst)

				if (r.gameObject.GetComponent<Renderer>())
				{
					if (r.gameObject.GetComponent<Renderer>().isVisible)
						tris += (r.sharedMesh.triangles.Length / 3);
				}

				iter++;

				if (iter > 30)
				{
					iter = 0;
					yield return 0f;
				}
			}

			countedTriangles = tris;

			if (countedTriangles > maxAllowedTriangles)
			{
				//Application.targetFrameRate = 31;
				tooManyTris = true;
			}
			else
			{
				//Application.targetFrameRate = 61;
				tooManyTris = false;
			}

			if (countedTriangles > maxTriangles)
				maxTriangles = countedTriangles;
		}


		string getAIInfo()
		{
			string dbgstr = "";

			dbgstr += "AI INFO: \n";

			if (!aiplayer)
				aiplayer = FindObjectOfType<AIPlayer>();

			if (!aiplayer)
			{
				if ((Time.frameCount & 32) > 0)
					return dbgstr + "\n\n NO AI PLAYER DETECTED";
				else
					return dbgstr;
			}

			dbgstr += "\nVirtualAnalogX: " + aiplayer.virtualAnalogX;
			dbgstr += "\nVirtualAnalogY: " + aiplayer.virtualAnalogY;

			dbgstr += "\nVirtualJumpDown: " + aiplayer.virtualJumpDown;
			dbgstr += "\nVirtualJumpHeld: " + aiplayer.virtualJumpHeld;

			dbgstr += "\nJumpTimer: " + aiplayer.jumpTimer;

			return dbgstr;
		}
	}
}
