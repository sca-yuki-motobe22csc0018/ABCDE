using UnityEngine;
using UnityEngine.UI; // Image�p
using DG.Tweening;
using System.Collections.Generic;

public class CardRotation : MonoBehaviour
{
    public Image frontImage;         // �\�ʂ�Image
    public Image backImage;          // ���ʂ�Image
    public Sprite[] cardSprites;     // �\�ʂ̃C���X�g����

    private int lastIndex = -1;//���O�ɑI�΂ꂽ�C���X�g�̃C���f�b�N�X���L������ϐ�

    private void Start()
    {
        // �ŏ��͕\�ʕ\���A���ʔ�\��
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

                // ���ʕ\���F90�x����270�x�̊Ԃ������ʂ�\��
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

                // �C���X�g�؂�ւ��^�C�~���O�i90�x���߂�������j
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
