using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckEditorUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform cardListParent;
    public Transform deckParent;

    public GameObject listItemPrefab;
    public GameObject deckItemPrefab;

    public CardDetailUI cardDetailUI;

    [Header("Deck Status")]
    public TMP_Text deckCountText;

    [Header("Search UI")]
    public TMP_InputField nameSearchField;
    public TMP_Dropdown deckSelectDropdown;

    List<string> currentDeck = new();

    void Start()
    {
        LoadDeckFromSave();
        RefreshCardList();
        RefreshDeckDisplay();

        // Dropdown変更時にデッキ切り替え
        deckSelectDropdown.onValueChanged.AddListener(_ => OnDeckChanged());
    }

    void Update()
    {
        //デバック用
        //// Dキー：全デッキ削除
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    DeckSaveManager.Instance.ClearAllDecks();
        //    LoadDeckFromSave();
        //    RefreshDeckDisplay();
        //    deckCountText.text = "全デッキを削除しました（Debug）";
        //}

        //// Fキー：選択中デッキ削除
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    int deckIndex = deckSelectDropdown.value;
        //    DeckSaveManager.Instance.ClearDeck(deckIndex);
        //    LoadDeckFromSave();
        //    RefreshDeckDisplay();
        //    deckCountText.text = $"デッキ{deckIndex + 1}を削除しました（Debug）";
        //}
    }


    //-------------------------------------------------------
    // デッキ切り替え
    //-------------------------------------------------------
    void OnDeckChanged()
    {
        LoadDeckFromSave();
        RefreshDeckDisplay();
    }

    //-------------------------------------------------------
    // デッキ読み込み
    //-------------------------------------------------------
    void LoadDeckFromSave()
    {
        int deckIndex = deckSelectDropdown.value;
        var deck = DeckSaveManager.Instance.GetDeck(deckIndex);

        if (deck == null || deck.cardNumbers == null)
            currentDeck = new List<string>();
        else
            currentDeck = new List<string>(deck.cardNumbers);
    }

    //-------------------------------------------------------
    // 右側：カード一覧
    //-------------------------------------------------------
    void RefreshCardList()
    {
        foreach (Transform t in cardListParent) Destroy(t.gameObject);

        foreach (var card in CardDatabase.Instance.cards)
        {
            if (!string.IsNullOrEmpty(nameSearchField.text) &&
                !card.name.Contains(nameSearchField.text))
                continue;

            var obj = Instantiate(listItemPrefab, cardListParent);

            obj.GetComponent<CardDisplayImageOnly>().SetCard(card, this);

            Button btn = obj.GetComponent<Button>();
            btn.onClick.AddListener(() => AddCardToDeck(card));
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

            var obj = Instantiate(deckItemPrefab, deckParent);
            obj.GetComponent<CardDisplayImageOnly>().SetCard(info, this);

            Button btn = obj.GetComponent<Button>();
            btn.onClick.AddListener(() => RemoveCardFromDeck(info));
        }

        deckCountText.text = $"現在のデッキ枚数 {currentDeck.Count}/30";
    }

    //-------------------------------------------------------
    // 追加 / 削除
    //-------------------------------------------------------
    public void AddCardToDeck(CardInfo card)
    {
        if (currentDeck.Count >= 30) return;

        int count = currentDeck.FindAll(x => x == card.number).Count;
        if (count >= 2) return;

        currentDeck.Add(card.number);
        RefreshDeckDisplay();
    }

    public void RemoveCardFromDeck(CardInfo card)
    {
        currentDeck.Remove(card.number);
        RefreshDeckDisplay();
    }

    //-------------------------------------------------------
    // 保存
    //-------------------------------------------------------
    public void OnSaveButton()
    {
        int deckIndex = deckSelectDropdown.value;

        if (currentDeck.Count != 30)
        {
            deckCountText.text = "デッキ枚数が30ではありません";
            return;
        }

        DeckData data = new DeckData
        {
            cardNumbers = new List<string>(currentDeck)
        };

        DeckSaveManager.Instance.SetDeck(deckIndex, data);

        deckCountText.text = $"デッキ{deckIndex + 1}をSAVEしました";
    }



//-------------------------------------------------------
// リセット
//-------------------------------------------------------
public void OnResetButton()
    {
        currentDeck.Clear();
        int deckIndex = deckSelectDropdown.value;
        DeckSaveManager.Instance.ClearDeck(deckIndex);
        LoadDeckFromSave();
        RefreshDeckDisplay();
        deckCountText.text = $"デッキ{deckIndex + 1}をResetしました";
    }

    //-------------------------------------------------------
    // 戻る
    //-------------------------------------------------------
    public void OnCloseButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
    }
}
