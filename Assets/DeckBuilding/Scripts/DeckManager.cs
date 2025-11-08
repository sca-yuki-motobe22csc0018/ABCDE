using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 現在のデッキ（通常枠）と固定カードを管理する
/// 公開メソッドのみを通してアクセスする想定
/// </summary>
public class DeckManager : MonoBehaviour
{
    public const int MAX_DECK = 30;
    public const int MAX_DUP = 2;

    private List<CardData> normalDeck = new List<CardData>();
    private List<CardData> fixedDeck = new List<CardData>();

    void Awake()
    {
        Locator.Register<DeckManager>(this);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SetupFixedCards();
    }

    /// <summary>
    /// 固定カードを設定する（CSV 内の特定 ID を使って固定同一カードを追加）
    /// ここでは 'EX_CARD' と 'ATK02_CARD' のように CSV の id を想定しています。
    /// 必要なら id を実際の CSV の値に合わせて変更してください。
    /// </summary>
    public void SetupFixedCards()
    {
        fixedDeck.Clear();
        var lib = Locator.Get<CardLibrary>();
        if (lib == null) return;

        // 例: CSV 内で EX の固定カードとして使う id を探す方法
        // 実運用では CSV の特定 id 名に合わせてここを調整してください。
        // ここでは、effectType1 が "EX" のカードを1つ取り、それを6回複製して固定にする例にしておきます。
        var exCard = lib.allCards.FirstOrDefault(c => c.type == "EX" || c.effectType1 == "EX" || c.id == "9001");
        var atk02Card = lib.allCards.FirstOrDefault(c => c.type == "Attack02" || c.id == "9002");

        if (exCard != null)
        {
            for (int i = 0; i < 6; i++) fixedDeck.Add(exCard);
        }

        if (atk02Card != null)
        {
            for (int i = 0; i < 4; i++) fixedDeck.Add(atk02Card);
        }

        Debug.Log($"[DeckManager] Fixed cards: {fixedDeck.Count} items.");
    }

    public bool AddCard(CardData card)
    {
        if (card == null) return false;
        if (normalDeck.Count >= MAX_DECK) return false;
        if (normalDeck.Count(c => c.id == card.id) >= MAX_DUP) return false;
        normalDeck.Add(card);
        return true;
    }

    public bool RemoveCard(CardData card)
    {
        if (card == null) return false;
        var found = normalDeck.FirstOrDefault(c => c.id == card.id);
        if (found != null)
        {
            normalDeck.Remove(found);
            return true;
        }
        return false;
    }

    public void ResetDeck()
    {
        normalDeck.Clear();
    }

    // 公開取得メソッド
    public List<CardData> GetNormalDeck() => new List<CardData>(normalDeck);
    public List<CardData> GetFixedDeck() => new List<CardData>(fixedDeck);
    public bool IsFull() => normalDeck.Count == MAX_DECK;
}
