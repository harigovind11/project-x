using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    private string saveFilePath;

    void Awake()
    {
        
        saveFilePath = Path.Combine(Application.persistentDataPath, "player_progress.json");
    }

    public void SaveProgress(PlayerProgress dataToSave)
    {
        string json = JsonUtility.ToJson(dataToSave, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Player progress saved to: " + saveFilePath);
    }

    public PlayerProgress LoadProgress()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            PlayerProgress loadedData = JsonUtility.FromJson<PlayerProgress>(json);
            Debug.Log("Player progress loaded.");
            return loadedData;
        }
        else
        {
            // If no save file exists, create a new one.
            Debug.Log("No progress file found. Creating a new one.");
            return new PlayerProgress();
        }
    }
}