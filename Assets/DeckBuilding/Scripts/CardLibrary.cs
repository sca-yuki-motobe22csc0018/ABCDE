using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// カード情報をCSVからロードし、検索・ソートできるクラス
/// </summary>
public class CardLibrary : MonoBehaviour
{
    public List<CardData> allCards = new List<CardData>();

    void Awake()
    {
        Locator.Register<CardLibrary>(this);
        DontDestroyOnLoad(gameObject);
        LoadFromCSV("cards");
    }

    /// <summary>
    /// Resources/cards.csv を読み込み、CardDataリストに変換する
    /// </summary>
    void LoadFromCSV(string fileName)
    {
        TextAsset ta = Resources.Load<TextAsset>(fileName);
        if (ta == null)
        {
            Debug.LogError("CSV not found: " + fileName);
            return;
        }

        var lines = ta.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            var row = lines[i].Split(',');
            if (row.Length < 6) continue;

            CardData cd = new CardData
            {
                id = row[0].Trim(),
                name = row[1].Trim(),
                category = row[2].Trim(),
                cost = int.Parse(row[3]),
                abilityText = row[4].Trim('"'),
                imageName = row[5].Trim()
            };
            allCards.Add(cd);
        }
    }

    /// <summary>
    /// 条件指定でカードを検索（カテゴリ、コスト範囲、名前など）
    /// </summary>
    public List<CardData> Query(string category = null, int? minCost = null, int? maxCost = null, string name = null)
    {
        IEnumerable<CardData> q = allCards;

        if (!string.IsNullOrEmpty(category))
            q = q.Where(c => c.category == category);

        if (minCost.HasValue)
            q = q.Where(c => c.cost >= minCost.Value);

        if (maxCost.HasValue)
            q = q.Where(c => c.cost <= maxCost.Value);

        if (!string.IsNullOrEmpty(name))
            q = q.Where(c => c.name.ToLower().Contains(name.ToLower()));

        return q.ToList();
    }

    /// <summary>
    /// IDでカード取得
    /// </summary>
    public CardData GetById(string id)
    {
        return allCards.FirstOrDefault(c => c.id == id);
    }
}
