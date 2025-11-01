using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// PlayerPrefsを使ってデッキを保存・読み込みする
/// 複数スロット対応
/// </summary>
public class SaveManager : MonoBehaviour
{
    void Awake()
    {
        Locator.Register<SaveManager>(this);
        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    public class DeckSaveData
    {
        public List<string> deckIds;
        public List<string> fixedIds;
    }

    /// <summary>
    /// スロット番号（例: 1～3）を指定してデッキ保存
    /// </summary>
    public void SaveDeck(int slot = 1)
    {
        var dm = Locator.Get<DeckManager>();
        DeckSaveData data = new DeckSaveData
        {
            deckIds = dm.GetDeck().Select(c => c.id).ToList(),
            fixedIds = dm.GetFixedCards().Select(c => c.id).ToList()
        };
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString($"DeckSave_{slot}", json);
        PlayerPrefs.Save();
        Debug.Log($"Deck saved to slot {slot}");
    }

    /// <summary>
    /// 指定スロットのデッキ読み込み
    /// </summary>
    public void LoadDeck(int slot = 1)
    {
        if (!PlayerPrefs.HasKey($"DeckSave_{slot}"))
        {
            Debug.Log($"No deck found in slot {slot}");
            return;
        }

        string json = PlayerPrefs.GetString($"DeckSave_{slot}");
        var data = JsonUtility.FromJson<DeckSaveData>(json);
        var lib = Locator.Get<CardLibrary>();

        var deckCards = data.deckIds.Select(id => lib.GetById(id)).Where(c => c != null).ToList();
        var fixedCards = data.fixedIds.Select(id => lib.GetById(id)).Where(c => c != null).ToList();

        Locator.Get<DeckManager>().LoadDeck(deckCards, fixedCards);
        Debug.Log($"Deck loaded from slot {slot}");
    }
}
