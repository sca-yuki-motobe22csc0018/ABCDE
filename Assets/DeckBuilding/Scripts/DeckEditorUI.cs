using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// デッキ編成画面の UI 管理クラス
/// - 左側詳細表示の更新 (ShowCardDetail)
/// - カード一覧 / デッキ / 固定カードの描画
/// - 検索・保存・読み込み・リセット・閉じる処理
/// 注: Locator 経由で各マネージャを参照
/// </summary>
public class DeckEditorUI : MonoBehaviour
{
    [Header("Parents (ScrollRect Contentなど)")]
    public Transform cardListParent;   // Tag: CardList を付けること（Raycast 判定用）
    public Transform deckParent;       // Tag: DeckSlot を付けること（Raycast 判定用）
    public Transform fixedAreaParent;

    [Header("Detail Panel (左)")]
    public Image detailImage;
    public TMP_Text detailNameText;
    public TMP_Text detailCostText;
    public TMP_Text detailTypeText;
    public TMP_Text detailText;

    [Header("Controls & Prefab")]
    public GameObject cardPrefab;      // Image + TMP_Text を含む簡易カードUI prefab
    public TMP_Dropdown categoryDropdown;
    public TMP_InputField nameInput;
    public TMP_InputField minCostInput;
    public TMP_InputField maxCostInput;
    public TMP_Dropdown slotDropdown;
    public Button searchButton;
    public Button saveButton;
    public Button loadButton;
    public Button resetButton;
    public Button closeButton;
    public TMP_Text infoText;

    CardLibrary cardLibrary => Locator.Get<CardLibrary>();
    DeckManager deckManager => Locator.Get<DeckManager>();
    SaveManager saveManager => Locator.Get<SaveManager>();
    SceneController sceneController => Locator.Get<SceneController>();

    void Start()
    {
        // ボタン登録（nullチェック）
        if (searchButton != null) searchButton.onClick.AddListener(RefreshCardList);
        if (saveButton != null) saveButton.onClick.AddListener(OnSave);
        if (loadButton != null) loadButton.onClick.AddListener(OnLoad);
        if (resetButton != null) resetButton.onClick.AddListener(OnReset);
        if (closeButton != null) closeButton.onClick.AddListener(OnClose);

        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshFixedArea();
        RefreshDeckView();
        RefreshCardList();
    }

    public void RefreshFixedArea()
    {
        if (fixedAreaParent == null) return;
        foreach (Transform t in fixedAreaParent) Destroy(t.gameObject);

        var fixedCards = deckManager.GetFixedDeck();
        foreach (var c in fixedCards)
        {
            var go = Instantiate(cardPrefab, fixedAreaParent);
            var text = go.GetComponentInChildren<TMP_Text>();
            if (text) text.text = $"{c.ruby}";
            var img = go.GetComponentInChildren<Image>();
            if (img) img.sprite = cardLibrary.GetCardSprite(c);

            // 固定は基本削除できないが見た目統一のため handler を付けておく（inDeck = true）
            var handler = go.AddComponent<CardDragHandler>();
            handler.Setup(c, this, true);
        }
    }

    public void RefreshDeckView()
    {
        if (deckParent == null) return;
        foreach (Transform t in deckParent) Destroy(t.gameObject);

        var current = deckManager.GetNormalDeck();
        foreach (var c in current)
        {
            var go = Instantiate(cardPrefab, deckParent);
            var text = go.GetComponentInChildren<TMP_Text>();
            if (text) text.text = $"{c.ruby} ({c.cost})";
            var img = go.GetComponentInChildren<Image>();
            if (img) img.sprite = cardLibrary.GetCardSprite(c);

            // デッキ側のカードはクリックで削除できる(inDeck=true)
            var handler = go.AddComponent<CardDragHandler>();
            handler.Setup(c, this, true);
        }

        if (infoText != null)
            infoText.text = $"デッキ枚数: {current.Count}/{DeckManager.MAX_DECK}";
    }

    public void RefreshCardList()
    {
        if (cardListParent == null) return;
        foreach (Transform t in cardListParent) Destroy(t.gameObject);

        int? min = int.TryParse(minCostInput?.text, out var mi) ? mi : (int?)null;
        int? max = int.TryParse(maxCostInput?.text, out var ma) ? ma : (int?)null;
        string cat = (categoryDropdown != null && categoryDropdown.value > 0) ? categoryDropdown.options[categoryDropdown.value].text : null;
        string name = string.IsNullOrEmpty(nameInput?.text) ? null : nameInput.text;

        var list = cardLibrary.Query(cat, min, max, name);
        foreach (var c in list)
        {
            var go = Instantiate(cardPrefab, cardListParent);
            var text = go.GetComponentInChildren<TMP_Text>();
            if (text) text.text = $"{c.ruby} ({c.cost})";
            var img = go.GetComponentInChildren<Image>();
            if (img) img.sprite = cardLibrary.GetCardSprite(c);

            // 一覧側のカードは inDeck=false（クリックで追加）
            var handler = go.AddComponent<CardDragHandler>();
            handler.Setup(c, this, false);
        }
    }

    // 左側詳細表示を更新する（CardDragHandler から呼ばれる）
    public void ShowCardDetail(CardData card)
    {
        if (card == null) return;
        if (detailNameText != null) detailNameText.text = card.ruby;
        if (detailCostText != null) detailCostText.text = $"コスト: {card.cost}";
        if (detailTypeText != null) detailTypeText.text = $"分類: {card.type}";
        if (detailText != null) detailText.text = card.text;
        if (detailImage != null) detailImage.sprite = cardLibrary.GetCardSprite(card);
    }

    // CardDragHandler から Add/Remove を呼ぶ（DeckManager の公開メソッドを使用）
    public void AddToDeck(CardData card)
    {
        if (deckManager.AddCard(card))
        {
            RefreshDeckView();
            RefreshCardList();
        }
        else
        {
            if (infoText != null) infoText.text = "追加できません（上限または枚数制限）";
        }
    }

    public void RemoveFromDeck(CardData card)
    {
        if (deckManager.RemoveCard(card))
        {
            RefreshDeckView();
            RefreshCardList();
        }
        else
        {
            if (infoText != null) infoText.text = "削除失敗";
        }
    }

    // ボタンコールバック
    void OnSave()
    {
        int slot = (slotDropdown != null) ? slotDropdown.value + 1 : 1;
        saveManager.SaveDeck(slot);
        if (infoText != null) infoText.text = $"保存しました（スロット{slot}）";
    }

    void OnLoad()
    {
        int slot = (slotDropdown != null) ? slotDropdown.value + 1 : 1;
        saveManager.LoadDeck(slot);
        RefreshAll();
        if (infoText != null) infoText.text = $"読み込みました（スロット{slot}）";
    }

    void OnReset()
    {
        deckManager.ResetDeck();
        RefreshAll();
        if (infoText != null) infoText.text = "デッキをリセットしました。";
    }

    void OnClose()
    {
        // 未保存の確認ダイアログを入れるのが望ましい（ここでは直接遷移）
        sceneController.LoadScene("MainScene");
    }
}
