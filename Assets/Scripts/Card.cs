using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public GameObject card;
    public  SpriteRenderer rend;
    public  int thisLayer;
    int upLayer=120;
    private GameObject currentHitObject = null;
    public string targetObjectName = "Card3"; // 対象のオブジェクト名
    void Start()
    {
        thisLayer = rend.sortingOrder;
    }


    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.name == targetObjectName)
            {
                if (currentHitObject != hitObject)
                {
                    MouseEnter();
                    currentHitObject = hitObject;
                }
            }
            else
            {
                if (currentHitObject != null)
                {
                    MouseExit();
                    currentHitObject = null;
                }
            }
        }
        else
        {
            if (currentHitObject != null)
            {
                MouseExit();
                currentHitObject = null;
            }
        }
    }


    private void MouseEnter()
    {
        rend.sortingOrder = upLayer;

        var sequence = DOTween.Sequence();
        sequence.Append(card.transform.DOScale(new Vector3(1.2f, 1.2f, 1), 0.1f).SetEase(Ease.InQuint));
        sequence.Append(card.transform.DOMoveY(this.transform.position.y + 0.5f, 0.1f));
    }
    void MouseExit()
    {
        rend.sortingOrder = thisLayer;
        var sequence = DOTween.Sequence();
        sequence.Append(card.transform.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.InQuint));
        sequence.Append(card.transform.DOMoveY(this.transform.position.y, 0.1f));
    }
}
