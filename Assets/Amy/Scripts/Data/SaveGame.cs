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

		public StoryFlag(int hash, bool value)
        {
			sceneHash = hash;
			isFinished = value;
        }

		public int sceneHash;
		public bool isFinished;

		public static int SADX_NUDE = Animator.StringToHash("SADX_NUDE");
	}

	[System.Serializable]
	public class SaveGame
	{

		public const int SAVE_VERSION = 1;



		public SaveGame()
		{

		}

		public static bool isSaveValid(int saveIndex)
		{
			string dataPath = Application.persistentDataPath + "/AmySavey" + saveIndex + ".dat";
			Debug.Log(dataPath);

			if (!File.Exists(dataPath))
			{
				return false;
			}

			if (saveIndex < 0)
				return false;


			FileStream file = File.OpenRead(dataPath);
			BinaryReader reader = new BinaryReader(file);

			int file_version = reader.ReadInt32();

			reader.Close();

			if (file_version > 0 && file_version <= SAVE_VERSION)
				return true;

			File.Delete(dataPath);
			return false;
		}

		public static void deleteSaveFile(int saveIndex)
		{
			string dataPath = Application.persistentDataPath + "/AmySavey" + saveIndex + ".dat";
			Debug.Log(dataPath);

			if (!File.Exists(dataPath))
			{
				return;
			}

			if (saveIndex < 0)
				return;

			File.Delete(dataPath);

		}

		public static SaveFileMetadata getFileMetaData(int saveIndex)
        {

			if (saveIndex < 0)
				return null;

			string dataPath = Application.persistentDataPath + "/AmySavey" + saveIndex + ".dat";
			Debug.Log(dataPath);

			if (!File.Exists(dataPath))
			{
				return null;
			}

			if (!isSaveValid(saveIndex))
				return null;

			SaveFileMetadata metaData = new SaveFileMetadata();

			FileStream file = File.OpenRead(dataPath);
			BinaryReader reader = new BinaryReader(file);


			metaData.fileIndex = saveIndex;

			reader.ReadInt32();
			metaData.lastSaveTime = System.DateTime.FromFileTime(reader.ReadInt64());
			metaData.ringBank = reader.ReadInt32();
			metaData.currentCharacter = (PlayableCharacter)reader.ReadInt32();

			metaData.totalHours = reader.ReadInt32();
			metaData.totalMinutes = reader.ReadInt32();
			metaData.totalSeconds = Mathf.RoundToInt(reader.ReadSingle());
			metaData.totalDays = reader.ReadInt32();

			reader.Close();

			return metaData;
		}

		public static void loadGame(int saveIndex)
        {

			if (saveIndex < 0)
				return;

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

			plman.totalHours = reader.ReadInt32();
			plman.totalMinutes = reader.ReadInt32();
			plman.totalSeconds = reader.ReadSingle();
			plman.days = reader.ReadInt32();
			plman.daysTilMenstruation = reader.ReadInt32();
			plman.universeNumber = reader.ReadInt32();

			//Progress flags
			plman.hasHammer = reader.ReadBoolean();
			plman.hasCloth = reader.ReadBoolean();
			plman.hasSlingshot = reader.ReadBoolean();

			readPlayerStatus(plman.AmyStatus, ref reader);
			readPlayerStatus(plman.CreamStatus, ref reader);

			plman.AmySuikaAffection = reader.ReadInt32();
			plman.CreamSuikaAffection = reader.ReadInt32();

			int numFlags = reader.ReadInt32();

			if (plman.storyFlags == null)
				plman.storyFlags = new List<StoryFlag>();

			plman.storyFlags.Clear();

			for(int i = 0; i < numFlags; i++)
            {

				plman.storyFlags.Add(new StoryFlag(reader.ReadInt32(), reader.ReadBoolean()));
			}

			reader.Close();

			float time = (float)(Application.targetFrameRate * (System.DateTime.Now - lastSaveTime).TotalSeconds);

			plman.processSleeping(plman.AmyStatus, time);
			plman.processSleeping(plman.CreamStatus, time);

			if(readSADXNudeModData())
            {
				plman.setStoryFlag(StoryFlag.SADX_NUDE, true);
            }
		}

		public static void writeSaveGame(int saveIndex)
		{
			if (saveIndex < 0)
				return;

			GameObject.Instantiate(GameManager.Instance.systemData.RES_NowSaving);

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
			writer.Write(plman.totalHours);
			writer.Write(plman.totalMinutes);
			writer.Write(plman.totalSeconds);
			writer.Write(plman.days);
			writer.Write(plman.daysTilMenstruation);
			writer.Write(plman.universeNumber);

			//Progress flags
			writer.Write(plman.hasHammer);
			writer.Write(plman.hasCloth);
			writer.Write(plman.hasSlingshot);

			writePlayerStatus(plman.AmyStatus, ref writer);
			writePlayerStatus(plman.CreamStatus, ref writer);

			writer.Write(plman.AmySuikaAffection);
			writer.Write(plman.CreamSuikaAffection);

			writer.Write(plman.storyFlags.Count);

			foreach(StoryFlag f in plman.storyFlags)
            {
				writer.Write(f.sceneHash);
				writer.Write(f.isFinished);
            }

			//Paddding at the end.
			for (int i = 0; i < 64; i++)
				writer.Write(0);

			writer.Close();
		}

		public static void writeConfigFile()
		{
			GameObject.Instantiate(GameManager.Instance.systemData.RES_NowSaving);

			string dataPath = Application.persistentDataPath + "/AmyConfig.dat";
			Debug.Log(dataPath);

			GameConfig cfg = GameManager.Instance.config;

			FileStream file = File.Create(dataPath);
			BinaryWriter writer = new BinaryWriter(file);
			writer.Write(SAVE_VERSION);
			writer.Write(cfg.desiredFOV);
			writer.Write(cfg.lookSensitivity);
			writer.Write(cfg.pitchInvert);
			writer.Write(cfg.yawInvert);

			//Paddding at the end.
			for (int i = 0; i < 64; i++)
				writer.Write(0);

			writer.Close();
		}

		public static void readConfigFile()
        {
			string dataPath = Application.persistentDataPath + "/AmyConfig.dat";
			Debug.Log(dataPath);

			if (!File.Exists(dataPath))
			{
				return;
			}

			FileStream file = File.OpenRead(dataPath);
			BinaryReader reader = new BinaryReader(file);

			//Go past the save version
			int curSaveVersion = reader.ReadInt32();

			GameManager.Instance.config.desiredFOV = reader.ReadSingle();
			GameManager.Instance.config.lookSensitivity = reader.ReadSingle();
			GameManager.Instance.config.pitchInvert = reader.ReadBoolean();
			GameManager.Instance.config.yawInvert = reader.ReadBoolean();

			reader.Close();
		}

		public static bool readSADXNudeModData()
        {
			string dataPath = Application.persistentDataPath + "/../Sonic Adventure/NUDE.dat";
			Debug.Log(dataPath);

			if (!File.Exists(dataPath))
			{
				return false;
			}

			FileStream file = File.OpenRead(dataPath);
			BinaryReader reader = new BinaryReader(file);

			//Go past the save version
			int magic = reader.ReadInt32();

			if (magic != 0x52504C41)
				return false;

			reader.Close();

			return true;
		}

		public static void writePlayerStatus(PlayerStatus pStats,ref BinaryWriter writer)
        {
			writer.Write(pStats.currentHealth);
			writer.Write(pStats.currentStamina);
			writer.Write(pStats.statusFX);
			writer.Write(pStats.sickTimeLeft);
			writer.Write(pStats.recentOrgasmTimeLeft);
			writer.Write(pStats.goodFoodTimeLeft);
			writer.Write(pStats.timeSpentResting);

			if (pStats.items == null)
				pStats.items = new List<ItemData>();

			writer.Write(pStats.items.Count);

			foreach(ItemData i in pStats.items)
            {
				writer.Write(i.getHash()); 
            }
			
        }

		public static void readPlayerStatus(PlayerStatus pStats, ref BinaryReader reader)
        {
			pStats.currentHealth = reader.ReadSingle();
			pStats.currentStamina = reader.ReadSingle();
			pStats.statusFX = reader.ReadInt32();
			pStats.sickTimeLeft = reader.ReadSingle();
			pStats.recentOrgasmTimeLeft = reader.ReadSingle();
			pStats.goodFoodTimeLeft = reader.ReadSingle();
			pStats.timeSpentResting = reader.ReadSingle();

			if (pStats.items == null)
				pStats.items = new List<ItemData>();

			pStats.items.Clear();

			int numItems = reader.ReadInt32();

			if (numItems > 0)
			{
				for (int i = 0; i < numItems; i++)
				{
					ItemData d = ItemData.getItemData(reader.ReadInt32());

					if(d != null)
						pStats.items.Add(d);
				}
			}

		}

	}
}
