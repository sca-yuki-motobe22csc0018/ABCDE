using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// PlayerPrefs を使ってデッキを複数スロット保存/読み込みする簡易マネージャ
/// </summary>
public class SaveManager : MonoBehaviour
{
    void Awake()
    {
        Locator.Register<SaveManager>(this);
        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    public class DeckSaveDTO
    {
        public List<string> normalIds;
        public List<string> fixedIds;
    }

    public void SaveDeck(int slot)
    {
        var dm = Locator.Get<DeckManager>();
        if (dm == null) return;

        var dto = new DeckSaveDTO
        {
            normalIds = dm.GetNormalDeck().Select(c => c.id).ToList(),
            fixedIds = dm.GetFixedDeck().Select(c => c.id).ToList()
        };

        string json = JsonUtility.ToJson(dto);
        PlayerPrefs.SetString($"DeckSlot_{slot}", json);
        PlayerPrefs.Save();
        Debug.Log($"[SaveManager] Saved deck slot {slot}");
    }

    public void LoadDeck(int slot)
    {
        var lib = Locator.Get<CardLibrary>();
        var dm = Locator.Get<DeckManager>();
        if (lib == null || dm == null) return;

        if (!PlayerPrefs.HasKey($"DeckSlot_{slot}"))
        {
            Debug.LogWarning($"[SaveManager] Deck slot {slot} not found.");
            return;
        }

        string json = PlayerPrefs.GetString($"DeckSlot_{slot}");
        var dto = JsonUtility.FromJson<DeckSaveDTO>(json);
        if (dto == null)
        {
            Debug.LogWarning($"[SaveManager] Failed to parse slot {slot}.");
            return;
        }

        dm.ResetDeck();
        foreach (var id in dto.normalIds ?? new List<string>())
        {
            var c = lib.GetById(id);
            if (c != null) dm.AddCard(c);
        }

        // 再設定（固定カードは CSV を参照）
        dm.SetupFixedCards();

        Debug.Log($"[SaveManager] Loaded deck slot {slot}");
    }
}
