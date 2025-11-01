using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 現在のデッキ・固定カードの管理
/// </summary>
public class DeckManager : MonoBehaviour
{
    public const int MAX_DECK = 30;      // 通常デッキ最大枚数
    public const int MAX_DUPLICATE = 2;  // 同一カード最大枚数

    private List<CardData> currentDeck = new List<CardData>();
    private List<CardData> fixedCards = new List<CardData>();

    void Awake()
    {
        Locator.Register<DeckManager>(this);
        DontDestroyOnLoad(gameObject);
        InitFixedCards();
    }

    /// <summary>
    /// 固定カード（エクストラ6枚 + Attack02 4枚）を登録
    /// </summary>
    void InitFixedCards()
    {
        var lib = Locator.Get<CardLibrary>();
        if (lib == null) return;

        var ex = lib.Query("Extra").Take(6);
        var atk2 = lib.Query("Attack02").Take(4);
        fixedCards = ex.Concat(atk2).ToList();
    }

    public bool AddCard(CardData card)
    {
        if (currentDeck.Count >= MAX_DECK) return false;
        if (currentDeck.Count(c => c.id == card.id) >= MAX_DUPLICATE) return false;
        currentDeck.Add(card);
        return true;
    }

    public bool RemoveCard(CardData card)
    {
        var target = currentDeck.Find(c => c.id == card.id);
        if (target != null)
        {
            currentDeck.Remove(target);
            return true;
        }
        return false;
    }

    public List<CardData> GetDeck() => currentDeck.ToList();
    public List<CardData> GetFixedCards() => fixedCards.ToList();
    public bool IsFull() => currentDeck.Count == MAX_DECK;
    public void ResetDeck() => currentDeck.Clear();

    public void LoadDeck(List<CardData> deck, List<CardData> fixedDeck)
    {
        currentDeck = new List<CardData>(deck);
        fixedCards = new List<CardData>(fixedDeck);
    }
}
