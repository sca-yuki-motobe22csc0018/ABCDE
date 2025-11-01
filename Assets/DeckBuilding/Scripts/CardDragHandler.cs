using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// マウスのクリックかドラッグかを判定して
/// DeckEditorUIに追加・削除を通知する
/// </summary>
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private bool isDragging = false;
    private CardData cardData;
    private DeckEditorUI deckUI;

    public void Setup(CardData card, DeckEditorUI ui)
    {
        cardData = card;
        deckUI = ui;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        // 将来的にドロップ位置判定を追加予定
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isDragging)
        {
            // デッキ追加または削除
            deckUI.OnCardClick(cardData);
        }
    }
}
