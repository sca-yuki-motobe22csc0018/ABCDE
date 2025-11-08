using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// クリック／ドラッグ操作で追加・削除・詳細表示を行うハンドラ
/// - Setup(card, editor, inDeck) で初期化
/// - inDeck==true -> デッキ内カード（クリックで削除）
/// </summary>
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    private CardData card;
    private DeckEditorUI editor;
    private bool isInDeck = false;

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Canvas rootCanvas;
    private Transform originalParent;
    private bool isDragging = false;

    /// <summary>
    /// card: 表示するカードデータ
    /// editor: DeckEditorUI の参照（ShowCardDetail / AddToDeck / RemoveFromDeck を呼ぶ）
    /// inDeck: この UI がデッキ側で表示されているか
    /// </summary>
    public void Setup(CardData cardData, DeckEditorUI editorUI, bool inDeck)
    {
        card = cardData;
        editor = editorUI;
        isInDeck = inDeck;
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        rootCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // まず詳細表示
        editor.ShowCardDetail(card);

        // ドラッグ中はクリック処理しない
        if (isDragging) return;

        if (isInDeck)
        {
            // デッキのカードはクリックで削除
            editor.RemoveFromDeck(card);
        }
        else
        {
            // 一覧のカードはクリックで追加
            editor.AddToDeck(card);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        originalParent = transform.parent;
        transform.SetParent(rootCanvas.transform, false);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rect == null || rootCanvas == null) return;
        rect.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 元の親に戻す
        transform.SetParent(originalParent, false);
        rect.anchoredPosition = Vector2.zero;
        canvasGroup.blocksRaycasts = true;

        // ドロップ先の判定（複数ヒットするためリストで確認）
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool droppedToDeck = false;
        bool droppedToList = false;

        foreach (var r in results)
        {
            if (r.gameObject.CompareTag("DeckSlot")) droppedToDeck = true;
            if (r.gameObject.CompareTag("CardList")) droppedToList = true;
        }

        if (!isInDeck && droppedToDeck)
        {
            // 一覧 -> デッキ ドロップ
            editor.AddToDeck(card);
        }
        else if (isInDeck && droppedToList)
        {
            // デッキ -> 一覧 ドロップ = 削除
            editor.RemoveFromDeck(card);
        }

        isDragging = false;
    }
}
