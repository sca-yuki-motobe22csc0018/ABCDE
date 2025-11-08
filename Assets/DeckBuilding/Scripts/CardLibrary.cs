using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// Resources/cards.csv を読み込み CardData のリストを作る
/// Query(...) で絞り込み可能
/// </summary>
public class CardLibrary : MonoBehaviour
{
    public List<CardData> allCards = new List<CardData>();

    void Awake()
    {
        // 登録 & 永続化
        Locator.Register<CardLibrary>(this);
        DontDestroyOnLoad(gameObject);

        LoadFromCSV("cards"); // Resources/cards.csv を期待
    }

    void LoadFromCSV(string resourceName)
    {
        TextAsset ta = Resources.Load<TextAsset>(resourceName);
        if (ta == null)
        {
            Debug.LogError($"[CardLibrary] Resources/{resourceName}.csv not found.");
            return;
        }

        allCards.Clear();

        using (StringReader sr = new StringReader(ta.text))
        {
            bool isFirstLine = true;
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                if (isFirstLine) { isFirstLine = false; continue; } // ヘッダーを飛ばす
                if (string.IsNullOrWhiteSpace(line)) continue;

                // 単純なカンマ分割（CSV にカンマを含む可能性がある場合はより厳密なパーサを使うべき）
                string[] cols = line.Split(',');

                if (cols.Length < 8) continue; // A-H が最低限必要

                CardData cd = new CardData();
                cd.id = cols[0].Trim();
                cd.ruby = cols[1].Trim();
                cd.type = cols[2].Trim();
                cd.rarity = cols[3].Trim();
                if (!int.TryParse(cols[4].Trim(), out int cost)) cost = 0;
                cd.cost = cost;
                cd.text = cols[5].Trim().Replace("\\n", "\n");
                cd.image = cols[6].Trim();
                cd.effectType1 = cols[7].Trim();

                allCards.Add(cd);
            }
        }

        Debug.Log($"[CardLibrary] Loaded {allCards.Count} cards from CSV.");
    }

    /// <summary>カテゴリ(Type)、コストレンジ、名前部分一致で絞る</summary>
    public List<CardData> Query(string type = null, int? minCost = null, int? maxCost = null, string nameOrId = null)
    {
        IEnumerable<CardData> q = allCards;

        if (!string.IsNullOrEmpty(type)) q = q.Where(c => c.type == type);
        if (minCost.HasValue) q = q.Where(c => c.cost >= minCost.Value);
        if (maxCost.HasValue) q = q.Where(c => c.cost <= maxCost.Value);
        if (!string.IsNullOrEmpty(nameOrId))
            q = q.Where(c => (c.ruby != null && c.ruby.Contains(nameOrId)) || (c.id != null && c.id.Contains(nameOrId)));

        return q.ToList();
    }

    /// <summary>Resources/Cards/<image> から Sprite を取得</summary>
    public Sprite GetCardSprite(CardData card)
    {
        if (card == null || string.IsNullOrEmpty(card.image)) return null;
        return Resources.Load<Sprite>($"Cards/{card.image}");
    }

    public CardData GetById(string id)
    {
        return allCards.FirstOrDefault(c => c.id == id);
    }
}
