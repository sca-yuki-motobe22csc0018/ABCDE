using UnityEngine;

/// <summary>
/// CSV の A〜H 列に対応するカードデータクラス
/// A: Name (id), B: ruby, C: Type, D: Rarity, E: Cost, F: Text, G: Image, H: effectType1
/// </summary>
[System.Serializable]
public class CardData
{
    public string id;          // A列 (Name)
    public string ruby;        // B列 (ruby)
    public string type;        // C列 (Type)
    public string rarity;      // D列 (Rarity)
    public int cost;           // E列 (Cost)
    public string text;        // F列 (Text)
    public string image;       // G列 (Image) -> Resources/Cards/<image>
    public string effectType1; // H列 (effectType1)
}
