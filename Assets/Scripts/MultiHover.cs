using UnityEngine;
using DG.Tweening;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MultiHover : MonoBehaviour
{
    public string[] targetObjectNames; // 対象のオブジェクト名リスト（複数）
    private bool isHoveringAny = false;
    public GameObject Hand;

    void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        bool nowHovering = false;

        if (hit.collider != null)
        {
            string hitName = hit.collider.gameObject.name;
            foreach (string targetName in targetObjectNames)
            {
                if (hitName == targetName)
                {
                    nowHovering = true;
                    break;
                }
            }
        }

        // 状態が変わったときだけ処理を呼ぶ
        if (nowHovering && !isHoveringAny)
        {
            MouseEnter();
        }
        else if (!nowHovering && isHoveringAny)
        {
            MouseExit();
        }

        isHoveringAny = nowHovering;
    }

    private void MouseEnter()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(Hand.transform.DOScale(new Vector3(1.5f, 1.5f, 1), 0.1f).SetEase(Ease.InQuint));
        //sequence.Append(Hand.transform.DOMoveY(this.transform.position.y + 0.75f, 0.1f));
    }
    void MouseExit()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(Hand.transform.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.InQuint));
        //sequence.Append(Hand.transform.DOMoveY(this.transform.position.y, 0.1f));
    }
}
