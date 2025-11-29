using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static CardDatabase Instance { get; private set; }

    public List<CardInfo> cards = new List<CardInfo>();
    public string csvFileName = "Card_Data.csv";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadCsv();
    }

    public void LoadCsv()
    {
        cards.Clear();

        string path = Path.Combine(Application.streamingAssetsPath, csvFileName);
        if (!File.Exists(path))
        {
            Debug.LogError("CSV not found: " + path);
            return;
        }

        var lines = File.ReadAllLines(path)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

        bool first = true;
        foreach (var line in lines)
        {
            if (first && line.Contains("Number"))
            {
                first = false;
                continue;
            }

            first = false;
            var c = line.Split(',');

            if (c.Length < 8) continue;

            CardInfo card = new CardInfo();
            card.number = c[0];
            card.name = c[1];
            card.ruby = c[2];
            card.type = c[3];
            card.rarity = c[4];
            int.TryParse(c[5], out card.cost);
            card.text = c[6];
            card.imageName = c[7];

            card.sprite = Resources.Load<Sprite>("Cards/" + card.imageName);

            cards.Add(card);
        }

        Debug.Log($"Loaded {cards.Count} cards.");
    }

    public CardInfo GetCard(string number)
        => cards.FirstOrDefault(c => c.number == number);
}
