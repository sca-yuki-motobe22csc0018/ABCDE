using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckSelectUI : MonoBehaviour
{
    // -----------------------------
    // デッキ1ボタン
    // -----------------------------
    public void OnSelectDeck1()
    {
        SelectDeck(0);
    }

    // -----------------------------
    // デッキ2ボタン
    // -----------------------------
    public void OnSelectDeck2()
    {
        SelectDeck(1);
    }

    // -----------------------------
    // デッキ3ボタン
    // -----------------------------
    public void OnSelectDeck3()
    {
        SelectDeck(2);
    }

    // -----------------------------
    // 共通処理
    // -----------------------------
    void SelectDeck(int deckIndex)
    {
        DeckSaveManager.Instance.SetSelectedDeck(deckIndex);
        Debug.Log(deckIndex);
    }
}
