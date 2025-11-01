using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// デッキ編成画面のUI管理：
/// 検索・スクロール・クリック追加・保存など
/// </summary>
public class DeckEditorUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject cardPrefab;
    public Transform cardListParent; // ScrollRectのContentに指定
    public TMP_Dropdown categoryDropdown;
    public TMP_InputField nameInput, minCostInput, maxCostInput;
    public Button searchButton, saveButton, resetButton, closeButton;
    public TMP_Dropdown slotDropdown; // 保存スロット選択
    public TextMeshProUGUI infoText;

    void Start()
    {
        searchButton.onClick.AddListener(OnSearch);
        saveButton.onClick.AddListener(OnSave);
        resetButton.onClick.AddListener(OnReset);
        closeButton.onClick.AddListener(OnClose);
        RefreshList();
    }

    /// <summary>
    /// カード一覧を再表示（ScrollRect対応）
    /// </summary>
    void RefreshList()
    {
        foreach (Transform t in cardListParent)
            Destroy(t.gameObject);

        var lib = Locator.Get<CardLibrary>();

        int? min = int.TryParse(minCostInput.text, out var mi) ? mi : (int?)null;
        int? max = int.TryParse(maxCostInput.text, out var ma) ? ma : (int?)null;
        string cat = categoryDropdown.value == 0 ? null : categoryDropdown.options[categoryDropdown.value].text;
        string name = string.IsNullOrEmpty(nameInput.text) ? null : nameInput.text;

        var cards = lib.Query(cat, min, max, name);
        foreach (var c in cards)
        {
            var obj = Instantiate(cardPrefab, cardListParent);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = $"{c.name} (cost:{c.cost})";
            var handler = obj.AddComponent<CardDragHandler>();
            handler.Setup(c, this);
        }

        infoText.text = $"Deck: {Locator.Get<DeckManager>().GetDeck().Count}/30";
    }

    public void OnCardClick(CardData card)
    {
        var dm = Locator.Get<DeckManager>();
        bool added = dm.AddCard(card);
        if (!added)
            dm.RemoveCard(card);
        RefreshList();
    }

    void OnSearch() => RefreshList();

    void OnSave()
    {
        var dm = Locator.Get<DeckManager>();
        if (!dm.IsFull())
        {
            infoText.text = "30枚揃っていません。";
            return;
        }
        int slot = slotDropdown.value + 1;
        Locator.Get<SaveManager>().SaveDeck(slot);
        infoText.text = $"スロット{slot}に保存しました。";
    }

    void OnReset()
    {
        Locator.Get<DeckManager>().ResetDeck();
        infoText.text = "デッキをリセットしました。";
        RefreshList();
    }

    void OnClose()
    {
        var dm = Locator.Get<DeckManager>();
        if (!dm.IsFull())
        {
            infoText.text = "保存されていません。閉じますか？";
            // 実際にはYes/Noダイアログを出すのが望ましい
        }
        Locator.Get<SceneController>().LoadScene("MainScene");
    }
}
