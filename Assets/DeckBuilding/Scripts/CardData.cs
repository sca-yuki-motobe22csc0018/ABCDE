using UnityEngine;

/// <summary>
/// CSVから読み込むカード1枚分のデータ構造
/// </summary>
[System.Serializable]
public class CardData
{
    public string id;          // 識別ID
    public string name;        // カード名
    public string category;    // 分類（Attack, Defense, etc.）
    public int cost;           // コスト
    public string abilityText; // 能力説明文
    public string imageName;   // Resources/Cards にある画像名
}
