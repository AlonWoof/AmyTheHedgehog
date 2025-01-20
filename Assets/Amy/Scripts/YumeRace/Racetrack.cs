using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	public enum RaceDifficulty
    {
		Easy,
		Medium,
		Hard
    }

	public enum Racer
    {
		Amy,
		Yume
    }


	public class Racetrack : MonoBehaviour
	{

		public List<RaceNode> racetrackNodes;

		public Transform playerStart;
		public Transform yumeStart;

		public Mesh amyMesh;
		public Mesh yumeMesh;

		public RaceDifficulty difficulty;

		public Player playerInstance;
		public AIPlayer yumeInstance;
		public AIPlayerRace yumeAI;

		public int playerCurrentNode = 0;
		public int playerLaps = 0;
		public int yumeCurrentNode = 0;
		public int yumeLaps = 0;

		public bool test_lapTimerActive = false;
		public float test_yumeLapTime = 0.0f;

		public float raceCountdown = 5.0f;

		private void OnValidate()
		{
			if (!playerStart)
            {
				GameObject g = new GameObject("PLAYER START");
				playerStart = g.transform;
            }

			if (!yumeStart)
			{
				GameObject g = new GameObject("YUME START");
				yumeStart = g.transform;
			}

			playerStart.transform.SetParent(gameObject.transform);
			yumeStart.transform.SetParent(gameObject.transform);
		}

        // Start is called before the first frame update
        void Start()
	    {
			Invoke("startRace", 1.0f);
	    }

		void test_StartLapTimer()
        {
			test_lapTimerActive = true;
			test_yumeLapTime = 0.0f;
		}

		void test_StopTimer()
        {
			Debug.Log("YUME LAP TIME: " + test_yumeLapTime);
			test_lapTimerActive = false;
			test_yumeLapTime = 0.0f;
        }

		void startRace()
		{
			if (PlayerManager.Instance.mPlayerInstance)
			{
				playerInstance = PlayerManager.Instance.mPlayerInstance;
				PlayerManager.Instance.mPlayerInstance.transform.position = playerStart.position;
				PlayerManager.Instance.mPlayerInstance.setAngleInstantly(playerStart.forward);
				PlayerManager.Instance.mPlayerInstance.tpc.centerBehindPlayer();
			}

			switch(difficulty)
            {
				case RaceDifficulty.Easy:
					GameManager.getSystemData().YumeParams.ingameModel = GameManager.getSystemData().YumeRace_EasyModel;
					GameManager.getSystemData().YumeParams = GameManager.getSystemData().YumeParams_Easy;
					break;

				case RaceDifficulty.Medium:
					GameManager.getSystemData().YumeParams.ingameModel = GameManager.getSystemData().YumeRace_MediumModel;
					GameManager.getSystemData().YumeParams = GameManager.getSystemData().YumeParams_Medium;
					break;

				case RaceDifficulty.Hard:
					GameManager.getSystemData().YumeParams.ingameModel = GameManager.getSystemData().YumeRace_HardModel;
					GameManager.getSystemData().YumeParams = GameManager.getSystemData().YumeParams_Hard;
					break;
			}

			yumeInstance = Player.Spawn(yumeStart.position, yumeStart.transform.forward, PlayableCharacter.Yume, true) as AIPlayer;
			yumeAI = yumeInstance.gameObject.AddComponent<AIPlayerRace>();
			yumeAI.difficulty = difficulty;


		}


		void OnGUI()
		{

			string dbgStr = "Race Info:\n";
			dbgStr += "\n Player Node: " + playerCurrentNode;
			dbgStr += "\n Yume Node: " + yumeCurrentNode;
			dbgStr += "\n Player Laps: " + playerLaps;
			dbgStr += "\n Yume Laps: " + yumeLaps;

			Helper.drawDebugText(new Vector2(100,100), dbgStr);
		}


		// Update is called once per frame
		void Update()
	    {
			if(raceCountdown > 0.0f)
            {
				raceCountdown -= Time.deltaTime;
				playerInstance.changeCurrentMode(PlayerModes.CUTSCENE);
				playerInstance.clearAccel();
				yumeAI.mPlayer.clearAccel();

				if (raceCountdown < 0.0f)
                {
					yumeAI.changePhase(YumeRacePhase.Normal);
					playerInstance.changeCurrentMode(PlayerModes.NORMAL);
					test_StartLapTimer();
				}

				return;
			}

			if (checkIfReachedWaypoint(playerInstance.transform, playerCurrentNode))
				playerCurrentNode++;

			if (checkIfReachedWaypoint(yumeInstance.transform, yumeCurrentNode))
				yumeCurrentNode++;

			if (playerCurrentNode > racetrackNodes.Count - 1)
			{
				playerCurrentNode = 0;
				playerLaps++;
			}

			if (yumeCurrentNode > racetrackNodes.Count - 1)
			{
				yumeCurrentNode = 0;
				yumeLaps++;
				test_StopTimer();
				test_StartLapTimer();
			}

			//if (yumeCurrentNode > playerCurrentNode)
			//	Debug.Log("Yume is Winning");
			//else
			//	Debug.Log("Amy is Winning");

			if (test_lapTimerActive)
				test_yumeLapTime += Time.deltaTime;

			if (Input.GetKeyDown(KeyCode.Keypad7))
			{
				ThirdPersonCamera tpc = playerInstance.tpc;

				if (playerInstance.tpc.playerTransform == playerInstance.transform)
				{
					tpc.playerTransform = yumeInstance.transform;
					tpc.mPlayer = yumeInstance;
				}
				else
                {
					tpc.playerTransform = playerInstance.transform;
					tpc.mPlayer = playerInstance;
				}
			}
		}

        private void OnTriggerEnter(Collider other)
        {

			if (other.gameObject == playerInstance.gameObject)
			{
				if(playerCurrentNode == racetrackNodes.Count)
                {
					playerLaps++;
					playerCurrentNode = 0;
                }
			}

			if (other.gameObject == yumeInstance.gameObject)
            {
				if (yumeCurrentNode == racetrackNodes.Count)
				{
					yumeLaps++;
					yumeCurrentNode = 0;
				}
			}
        }


        bool checkIfReachedWaypoint(Transform racer, int waypointNum)
        {
			if (waypointNum > racetrackNodes.Count - 1)
				return false;

			if (Vector3.Distance(racer.transform.position, racetrackNodes[waypointNum].transform.position) < 6.0f)
				return true;

			return false;
		}

        private void OnDrawGizmos()
        {

			foreach (RaceNode w in GetComponentsInChildren<RaceNode>())
			{
				if (!racetrackNodes.Contains(w))
					racetrackNodes.Add(w);
			}

			if (racetrackNodes.Count < 2)
				return;

			for(int i = 0; i < racetrackNodes.Count; i++)
            {
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireSphere(racetrackNodes[i].transform.position, racetrackNodes[i].aiRange);

				if(i - 1 > -1)
                {
					Gizmos.DrawLine(racetrackNodes[i - 1].transform.position, racetrackNodes[i].transform.position);
                }

				if((i + 1) == racetrackNodes.Count)
                {
					Gizmos.DrawLine(racetrackNodes[i].transform.position, racetrackNodes[0].transform.position);
				}

				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(racetrackNodes[i].transform.position, racetrackNodes[i].playerRange);
			}

			Color drawColor = SystemColors.AmyColor;
			drawColor.a = 0.5f;
			Gizmos.color = drawColor;
			Gizmos.DrawWireMesh(amyMesh, playerStart.position, playerStart.rotation * Quaternion.Euler(-90, 0, 0), Vector3.one * 100.0f);

			drawColor = SystemColors.YumeColor;
			drawColor.a = 0.5f;
			Gizmos.color = drawColor;

			Gizmos.DrawWireMesh(yumeMesh, yumeStart.position, yumeStart.rotation * Quaternion.Euler(-90, 0, 0), Vector3.one * 100.0f);

		}
    }
}
