using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[System.Serializable]
	public class StoryFlag
	{
		public int sceneHash;
		public bool isFinished;
	}

	[System.Serializable]
	public class SaveGame
	{

		public const int SAVE_VERSION = 1;

		public PlayableCharacter currentCharacter;
		public int ringBank;
		public PlayerStatus AmyStatus;
		public PlayerStatus CreamStatus;

		public ProgressData progress;
		public List<StoryFlag> cutsceneFlags;
		public long saveTime;

		public SaveGame()
		{
			cutsceneFlags = new List<StoryFlag>();
		}

		public static bool isSaveValid(int saveIndex)
		{
			string dataPath = Application.persistentDataPath + "/AmySavey" + saveIndex + ".dat";
			Debug.Log(dataPath);

			if (!File.Exists(dataPath))
			{
				return false;
			}


			FileStream file = File.OpenRead(dataPath);
			BinaryReader reader = new BinaryReader(file);

			int file_version = reader.ReadInt32();

			reader.Close();

			if (file_version > 0 && file_version <= SAVE_VERSION)
				return true;

			File.Delete(dataPath);
			return false;
		}


		public static void loadGame(int saveIndex)
        {
			string dataPath = Application.persistentDataPath + "/AmySavey" + saveIndex + ".dat";
			Debug.Log(dataPath);

			if (!File.Exists(dataPath))
			{
				return;
			}

			if (!isSaveValid(saveIndex))
				return;

			FileStream file = File.OpenRead(dataPath);
			BinaryReader reader = new BinaryReader(file);

			//Go past the save version
			int curSaveVersion = reader.ReadInt32();

			PlayerManager plman = PlayerManager.Instance;
			GameManager gman = GameManager.Instance;

			System.DateTime lastSaveTime = System.DateTime.FromFileTime(reader.ReadInt64());
			plman.ringBank = reader.ReadInt32();
			plman.currentCharacter = (PlayableCharacter)reader.ReadInt32();

			//Progress flags
			plman.hasHammer = reader.ReadBoolean();
			plman.hasCloth = reader.ReadBoolean();
			plman.hasSlingshot = reader.ReadBoolean();

			readPlayerStatus(plman.AmyStatus, ref reader);
			readPlayerStatus(plman.CreamStatus, ref reader);

			reader.Close();

			float time = (float)(Application.targetFrameRate * (System.DateTime.Now - lastSaveTime).TotalSeconds);

			plman.processSleeping(plman.AmyStatus, time);
			plman.processSleeping(plman.CreamStatus, time);
		}

		public static void writeSaveGame(int saveIndex)
		{
			string dataPath = Application.persistentDataPath + "/AmySavey" + saveIndex + ".dat";
			Debug.Log(dataPath);

			FileStream file = File.Create(dataPath);
			BinaryWriter writer = new BinaryWriter(file);

			PlayerManager plman = PlayerManager.Instance;
			GameManager gman = GameManager.Instance;

			writer.Write(SAVE_VERSION);
			writer.Write(System.DateTime.Now.ToFileTime());
			writer.Write(plman.ringBank);
			writer.Write((int)plman.currentCharacter);

			//Progress flags
			writer.Write(plman.hasHammer);
			writer.Write(plman.hasCloth);
			writer.Write(plman.hasSlingshot);

			writePlayerStatus(plman.AmyStatus, ref writer);
			writePlayerStatus(plman.CreamStatus, ref writer);

			//Paddding at the end.
			for (int i = 0; i < 64; i++)
				writer.Write(0);

			writer.Close();
		}


		public static void writePlayerStatus(PlayerStatus pStats,ref BinaryWriter writer)
        {
			writer.Write(pStats.currentHealth);
			writer.Write(pStats.currentStamina);
			writer.Write(pStats.statusFX);
			writer.Write(pStats.sickTimeLeft);
			writer.Write(pStats.timeSpentResting);
        }

		public static void readPlayerStatus(PlayerStatus pStats, ref BinaryReader reader)
        {
			pStats.currentHealth = reader.ReadSingle();
			pStats.currentStamina = reader.ReadSingle();
			pStats.statusFX = reader.ReadInt32();
			pStats.sickTimeLeft = reader.ReadSingle();
			pStats.timeSpentResting = reader.ReadSingle();
		}

		public float calcTimeInFrames()
        {
			System.DateTime lastDate = System.DateTime.FromFileTime(saveTime);

			Debug.Log("It has been "+ (System.DateTime.Now - lastDate).TotalSeconds + " seconds since last save");
			return (float)((System.DateTime.Now - lastDate).TotalSeconds * 60.0f);
		}
	}
}
