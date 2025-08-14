using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    void Awake()
    {

        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    public bool DoesSaveFileExist()
    {
        return File.Exists(saveFilePath);
    }

    public void SaveGame(GameSaveData dataToSave)
    {

        string json = JsonUtility.ToJson(dataToSave, true);

        File.WriteAllText(saveFilePath, json);

        Debug.Log("Game Saved to: " + saveFilePath);
    }

    public GameSaveData LoadGame()
    {
        if (DoesSaveFileExist())
        {
            string json = File.ReadAllText(saveFilePath);

            GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(json);

            Debug.Log("Game Loaded!");
            return loadedData;
        }
        else
        {
            Debug.Log("No save file found.");
            return null;
        }
    }
}