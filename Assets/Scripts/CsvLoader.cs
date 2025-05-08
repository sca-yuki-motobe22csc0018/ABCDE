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
    public string image; // Resources内の画像ファイル名
}

public class CsvLoader : MonoBehaviour
{
    void Start()
    {
        // StreamingAssetsフォルダのCSVファイルパスを取得
        string filePath = Path.Combine(Application.streamingAssetsPath, "Card_ata.csv");

        // ファイルを読み込む
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
            Debug.LogError("CSVファイルが見つかりませんでした: " + filePath);
        }
    }
}
