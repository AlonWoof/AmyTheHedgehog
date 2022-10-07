using System.Collections.Generic;
using UnityEngine;


namespace Artngame.INfiniDy
{
    public static class SaveDataManager
    {
        public static void SaveJsonData(IEnumerable<ISaveable> a_Saveables)
        {
            SaveGrassData sd = new SaveGrassData();
            foreach (var saveable in a_Saveables)
            {
                saveable.PopulateSaveData(sd);
            }

            if (FileManager.WriteToFile("SaveGrassData01.dat", sd.ToJson()))
            {
                Debug.Log("Save successful");
            }
        }

        public static void LoadJsonData(IEnumerable<ISaveable> a_Saveables)
        {
            if (FileManager.LoadFromFile("SaveGrassData01.dat", out var json))
            {
                SaveGrassData sd = new SaveGrassData();
                sd.LoadFromJson(json);

                foreach (var saveable in a_Saveables)
                {
                    saveable.LoadFromSaveData(sd);
                }

                Debug.Log("Load complete");
            }
        }
    }
}