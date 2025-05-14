using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI; // Text型を使うため必要

public class CardGenerator : MonoBehaviour
{
    public GameObject[] Type;
    public Text costText;
    public Text nameText;
    public Text textText;
    public int a;

    [System.Serializable]
    public class CardData
    {
        public int id;
        public string name;
        public int type;
        public string rarity;
        public int cost;
        public string text;
        // public string image; // Resources内の画像ファイル名
    }

    public List<CardData> cardList = new List<CardData>();

    void Start()
    {
        LoadCSV();
        costText.text = cardList[a].cost.ToString();
        nameText.text = cardList[a].name;
        textText.text = cardList[a].text;
        Type[0].SetActive(true);
        Type[cardList[a].type].SetActive(true);
    }

    void LoadCSV()
    {
        string path = Application.streamingAssetsPath + "/Card_Data.csv";
        string csvText = File.ReadAllText(path, Encoding.UTF8);
        List<string[]> rows = ParseCsv(csvText);

        for (int i = 1; i < rows.Count; i++) // 0行目はヘッダー
        {
            string[] values = rows[i];
            if (values.Length < 6) continue;

            CardData data = new CardData();
            data.id = int.Parse(values[0]);
            data.name = values[1];
            data.type = int.Parse(values[2]);
            data.rarity = values[3];
            data.cost = int.Parse(values[4]);
            data.text = values[5];
            // data.image = values[6];

            cardList.Add(data);
        }
    }

    // セル内改行・カンマ・クォート対応 CSV パーサー
    List<string[]> ParseCsv(string csvText)
    {
        var rows = new List<string[]>();
        var currentRow = new List<string>();
        var currentField = new StringBuilder();

        bool inQuotes = false;
        for (int i = 0; i < csvText.Length; i++)
        {
            char c = csvText[i];
            char next = i + 1 < csvText.Length ? csvText[i + 1] : '\0';

            if (inQuotes)
            {
                if (c == '"' && next == '"')
                {
                    currentField.Append('"');
                    i++; // skip next
                }
                else if (c == '"')
                {
                    inQuotes = false;
                }
                else
                {
                    currentField.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == ',')
                {
                    currentRow.Add(currentField.ToString());
                    currentField.Clear();
                }
                else if (c == '\r' && next == '\n')
                {
                    currentRow.Add(currentField.ToString());
                    rows.Add(currentRow.ToArray());
                    currentRow = new List<string>();
                    currentField.Clear();
                    i++; // skip \n
                }
                else if (c == '\n' || c == '\r')
                {
                    currentRow.Add(currentField.ToString());
                    rows.Add(currentRow.ToArray());
                    currentRow = new List<string>();
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }
        }

        if (currentField.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(currentField.ToString());
            rows.Add(currentRow.ToArray());
        }

        return rows;
    }
}