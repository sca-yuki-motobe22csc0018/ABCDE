using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class CardData
{
    public int id;
    public string name;
    public int cost;
    public string description;
    public string image; // Resources���̉摜�t�@�C����
}

public class CsvLoader : MonoBehaviour
{
    void Start()
    {
        // StreamingAssets�t�H���_��CSV�t�@�C���p�X���擾
        string filePath = Path.Combine(Application.streamingAssetsPath, "Card_ata.csv");

        // �t�@�C����ǂݍ���
        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] values = line.Split(',');
                Debug.Log("ID: " + values[0] + ", Name: " + values[1] + ", Type: " + values[2] + ", Rarity: " + values[3] + ", Cost: " + values[4] + ", Text: " + values[5]);
            }
        }
        else
        {
            Debug.LogError("CSV�t�@�C����������܂���ł���: " + filePath);
        }
    }
}
