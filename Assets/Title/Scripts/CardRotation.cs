using UnityEngine;
using UnityEngine.UI; // Image用
using DG.Tweening;
using System.Collections.Generic;

public class CardRotation : MonoBehaviour
{
    public Image frontImage;         // 表面のImage
    public Image backImage;          // 裏面のImage
    public Sprite[] cardSprites;     // 表面のイラストたち

    private int lastIndex = -1;//直前に選ばれたイラストのインデックスを記憶する変数

    private void Start()
    {
        // 最初は表面表示、裏面非表示
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);

        ChangeRandomSprite();

        LoopRotation();
    }

    private void LoopRotation()
    {
        DOVirtual.DelayedCall(2f, () =>
        {
            RotateCard();
        });
    }

    private void RotateCard()
    {
        float duration = 0.5f;
        bool spriteChanged = false;

        transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                float yRotation = transform.eulerAngles.y;

                // 裏面表示：90度から270度の間だけ裏面を表示
                if (yRotation >= 90 && yRotation <= 270)
                {
                    frontImage.gameObject.SetActive(false);
                    backImage.gameObject.SetActive(true);
                }
                else
                {
                    frontImage.gameObject.SetActive(true);
                    backImage.gameObject.SetActive(false);
                }

                // イラスト切り替えタイミング（90度を過ぎた直後）
                if (!spriteChanged && yRotation > 90)
                {
                    ChangeRandomSprite();
                    spriteChanged = true;
                }
            })
            .OnComplete(() =>
            {
                LoopRotation();
            });
    }

    private void ChangeRandomSprite()
    {
        if (cardSprites == null || cardSprites.Length == 0) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, cardSprites.Length);
        } while (newIndex == lastIndex && cardSprites.Length > 1);

        lastIndex = newIndex;

        frontImage.sprite = cardSprites[newIndex];
    }
}
