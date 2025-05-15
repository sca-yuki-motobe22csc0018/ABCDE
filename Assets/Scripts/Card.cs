using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    public GameObject card;
    public GameObject thisHand;
    public  SpriteRenderer rend;
    public  int thisLayer;
    int upLayer=120;
    private GameObject currentHitObject = null;
    public string targetObjectName; // 対象のオブジェクト名
    bool onMouse;
    bool drag;
    //private Vector3 offset;

    void Start()
    {
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
        if (Input.GetMouseButtonDown(0)&&onMouse)
        {
            //offset = card.transform.position - (Vector3)mousePos;
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
            card.transform.position = (Vector3)mousePos;// + offset;
        }
    }


    private void MouseEnter()
    {
        rend.sortingOrder = upLayer;
        onMouse = true;
        if (!drag)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOScale(new Vector3(0.3f, 0.3f, 1), 0.1f).SetEase(Ease.InQuint));
            sequence.Join(card.transform.DOMoveY(this.transform.position.y + 0.5f, 0.1f));
        }
    }
    void MouseExit()
    {
        rend.sortingOrder = thisLayer;
        onMouse = false;
        if (!drag)
        {
            var sequence = DOTween.Sequence();
            sequence.Append(card.transform.DOScale(new Vector3(0.25f, 0.25f, 1), 0.1f).SetEase(Ease.InQuint));
            sequence.Join(card.transform.DOMoveY(this.transform.position.y, 0.1f));
        }
    }
}