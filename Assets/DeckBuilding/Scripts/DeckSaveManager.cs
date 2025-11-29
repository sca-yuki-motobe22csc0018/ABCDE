using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DeckSaveManager : MonoBehaviour
{
    public static DeckSaveManager Instance;

    string fileName = "decks.json";

    [Serializable]
    public class DeckSaveFile
    {
        public List<DeckData> decks = new();
    }

    public DeckSaveFile saveFile = new();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    public void Save()
    {
        string json = JsonUtility.ToJson(saveFile, true);
        File.WriteAllText(FilePath, json);
    }

    public void Load()
    {
        if (!File.Exists(FilePath))
        {
            saveFile = new DeckSaveFile();
            for (int i = 0; i < 3; i++)
                saveFile.decks.Add(new DeckData());
            return;
        }

        string json = File.ReadAllText(FilePath);
        saveFile = JsonUtility.FromJson<DeckSaveFile>(json);
    }

    public DeckData GetDeck(int index)
    {
        if (index < 0 || index >= saveFile.decks.Count) return null;
        return saveFile.decks[index];
    }

    public void SetDeck(int index, DeckData data)
    {
        if (index < 0 || index >= saveFile.decks.Count) return;
        saveFile.decks[index] = data;
        Save();
    }
}
