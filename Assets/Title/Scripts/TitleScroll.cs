using DG.Tweening;
using UnityEngine;

public class TitleScroll : MonoBehaviour
{
    private float scrollDistance = 2160f;  // UI座標系での高さ（ピクセル）
    private float scrollDuration = 5f;
    private float delay = 2.0f;
    private RectTransform rectTransform;
    private Vector2 startPos;
    private int loop = -1;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;

        rectTransform.DOAnchorPosY(startPos.y - scrollDistance, scrollDuration)
            .SetLoops(loop, LoopType.Restart)
            .SetEase(Ease.Linear)
            .SetDelay(delay);
    }
}
