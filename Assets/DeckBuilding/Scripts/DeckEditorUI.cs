using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckEditorUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform cardListParent;
    public Transform deckParent;

    public GameObject listItemPrefab;   // 右側カード一覧用（画像1枚のPrefab）
    public GameObject deckItemPrefab;   // 中央デッキ表示用（画像1枚のPrefab）

    public CardDetailUI cardDetailUI;

    [Header("Deck Status")]
    public TMP_Text deckCountText;

    [Header("Search UI")]
    public TMP_InputField nameSearchField;
    public TMP_Dropdown deckSelectDropdown;

    List<string> currentDeck = new();
    DeckData loadingDeck;
    int selectedDeckIndex = 0;

    void Start()
    {
        LoadDeckFromSave();
        RefreshCardList();
        RefreshDeckDisplay();
    }

    //-------------------------------------------------------
    // デッキ読み込み
    //-------------------------------------------------------
    void LoadDeckFromSave()
    {
        selectedDeckIndex = deckSelectDropdown.value;
        loadingDeck = DeckSaveManager.Instance.GetDeck(selectedDeckIndex);

        currentDeck = new List<string>(loadingDeck.cardNumbers);
    }

    //-------------------------------------------------------
    // 右側：カード一覧
    //-------------------------------------------------------
    void RefreshCardList()
    {
        foreach (Transform t in cardListParent) Destroy(t.gameObject);

        foreach (var card in CardDatabase.Instance.cards)
        {
            // 名前検索
            if (!string.IsNullOrEmpty(nameSearchField.text))
            {
                if (!card.name.Contains(nameSearchField.text)) continue;
            }

            // ★Prefabを生成（画像のみ）
            var obj = Instantiate(listItemPrefab, cardListParent);

            // ★画像をセット
            obj.GetComponent<CardDisplayImageOnly>().SetCard(card);

            // ★クリックでデッキに追加
            Button btn = obj.GetComponent<Button>();
            btn.onClick.AddListener(() => AddCardToDeck(card));

            // ★詳細表示（左側）
            btn.onClick.AddListener(() => ShowDetail(card));
        }
    }

    public void OnSearchButton()
    {
        RefreshCardList();
    }


    //-------------------------------------------------------
    // 左側：カード詳細
    //-------------------------------------------------------
    public void ShowDetail(CardInfo card)
    {
        cardDetailUI.Show(card);
    }


    //-------------------------------------------------------
    // 中央：デッキ表示
    //-------------------------------------------------------
    void RefreshDeckDisplay()
    {
        foreach (Transform t in deckParent) Destroy(t.gameObject);

        foreach (var num in currentDeck)
        {
            var info = CardDatabase.Instance.GetCard(num);
            if (info == null) continue;

            // ★デッキ用Prefab（画像だけ）生成
            var obj = Instantiate(deckItemPrefab, deckParent);

            // ★画像セット
            obj.GetComponent<CardDisplayImageOnly>().SetCard(info);

            // ★クリックでデッキから削除
            Button btn = obj.GetComponent<Button>();
            btn.onClick.AddListener(() => RemoveCardFromDeck(info));
        }

        deckCountText.text = currentDeck.Count + "/30";
    }


    //-------------------------------------------------------
    // カード追加・削除
    //-------------------------------------------------------
    public void AddCardToDeck(CardInfo card)
    {
        // 30枚上限
        if (currentDeck.Count >= 30) return;

        // 同名2枚制限
        int count = currentDeck.FindAll(x => x == card.number).Count;
        if (count >= 2) return;

        // EX1/EX2 特別ルール（必要ならここに追加）

        currentDeck.Add(card.number);
        RefreshDeckDisplay();
    }

    public void RemoveCardFromDeck(CardInfo card)
    {
        currentDeck.Remove(card.number);
        RefreshDeckDisplay();
    }


    //-------------------------------------------------------
    // デッキ保存
    //-------------------------------------------------------
    public void OnSaveButton()
    {
        if (currentDeck.Count != 30)
        {
            deckCountText.text = "デッキ枚数が30ではありません";
            return;
        }

        DeckData data = new DeckData();
        data.cardNumbers = new List<string>(currentDeck);
        DeckSaveManager.Instance.SetDeck(selectedDeckIndex, data);

        deckCountText.text = "保存しました";
    }

    //-------------------------------------------------------
    // リセット
    //-------------------------------------------------------
    public void OnResetButton()
    {
        currentDeck.Clear();
        RefreshDeckDisplay();
    }

    //-------------------------------------------------------
    // 戻る
    //-------------------------------------------------------
    public void OnCloseButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }
}
