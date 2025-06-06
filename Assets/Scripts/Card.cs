using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public GameObject card;
    public GameObject thisHand;
    SpriteRenderer rend;
    public int thisLayer;
    int upLayer=120;
    private GameObject currentHitObject = null;
    bool onMouse;
    bool drag;

    void Start()
    {
        rend= card.GetComponent<SpriteRenderer>();
        thisLayer = rend.sortingOrder;
        onMouse=false;
        drag = false;
    }


    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject == card)
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
        if (Input.GetMouseButtonDown(0)&&onMouse)
        {
            drag = true;
            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOScale(new Vector3(0.4f, 0.4f, 1), 0.1f).SetEase(Ease.InQuint));
        }
        if (Input.GetMouseButtonUp(0))
        {
            card.transform.position=this.transform.position;
            drag = false;
            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOScale(new Vector3(0.25f, 0.25f, 1), 0.1f).SetEase(Ease.InQuint));
        }
        if (drag)
        {
            card.transform.position = (Vector3)mousePos;
        }
    }


    private void MouseEnter()
    {
        if (!drag)
        {
            rend.sortingOrder = upLayer;
            onMouse = true;
            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOScale(new Vector3(0.3f, 0.3f, 1), 0.1f).SetEase(Ease.InQuint));
            sequence.Join(card.transform.DOMoveY(this.transform.position.y + 0.5f, 0.1f));
        }
    }
    void MouseExit()
    {
        if (!drag)
        {
            rend.sortingOrder = thisLayer;
            onMouse = false;
            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOScale(new Vector3(0.25f, 0.25f, 1), 0.1f).SetEase(Ease.InQuint));
            sequence.Join(card.transform.DOMoveY(this.transform.position.y, 0.1f));
        }
    }
}