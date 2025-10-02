using DG.Tweening;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Text clickStart;//clickstartText
    [SerializeField] Text title;//titleText
    //�ǉ�
    public Image frontImage;         // �\�ʂ�Image
    public Image backImage;          // ���ʂ�Image
    public Sprite[] cardSprites;     // �\�ʂ̃C���X�g����
    
    public GameObject card;//��]����J�[�h

    private int lastIndex = -1;//���O�ɑI�΂ꂽ�C���X�g�̃C���f�b�N�X���L������ϐ�


    //�����܂�

    //public Sprite[] titleImages; // �\�ʂ̃C���X�g�����o�^
    //public Image targetImage;    // �\��������Image�R���|�[�l���g

    private float clickStartAlpha = 0.2f;//ClickStart�p�@�A���t�@�l
    private float alphaTime = 1.0f;//ClickStart�p�@�����ɂ����鎞��

    private int loop = -1;//���[�v

    private float titleTime = 1.0f;//fade�ɂ����鎞��
    private float fadeAlpha = 1.0f;//fade�p�A���t�@�l

    private bool clickCheck = false;//���N���b�N�ɂ��V�[���ړ��딚�j�~�p

    void Start()
    {
        // �ŏ��͕\�ʕ\���A���ʔ�\��
        frontImage.enabled = true;
        backImage.enabled = false;

        ChangeRandomSprite();
        Fade();
        LoopRotation();
    }

    // Update is called once per frame
    void Update()
    {
        //�Q�[���I���Ăяo��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameEnd();
        }
        //�^�C�g��������܂ŃN���b�N��t�j�~�p
        if (clickCheck)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                changeScene();
            }
        }
    }

    /// <summary>
    /// clickstart�̓_��
    /// </summary>
    void TitleMove()
    {
        clickCheck = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(clickStart.DOFade(clickStartAlpha, alphaTime).SetEase(Ease.InSine));
        seq.SetLoops(loop, LoopType.Yoyo);
    }

    /// <summary>
    /// �^�C�g����ʂ̃A���t�@�l�̕ω�
    /// </summary>
    void Fade()
    {
        Sequence fade = DOTween.Sequence();
        fade.Append(title.DOFade(fadeAlpha, titleTime).SetEase(Ease.InSine));
        fade.Join(clickStart.DOFade(fadeAlpha, titleTime).SetEase(Ease.InSine));
        fade.Join(frontImage.DOFade(fadeAlpha, titleTime).SetEase(Ease.InSine));
        fade.OnComplete(() => TitleMove());

    }

    /// <summary>
    /// �f�B���C�@�Q�b���RotateCard���Ă�
    /// </summary>
    private void LoopRotation()
    {
        DOVirtual.DelayedCall(4f, () =>
        {
            RotateCard();
        });
    }
    /// <summary>
    /// �J�[�h����]������@ChangeRandomSprite��r���ŌĂ�
    /// </summary>
    private void RotateCard()
    {
        float duration = 0.5f;
        bool spriteChanged = false;

        card.transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                float yRotation = card.transform.eulerAngles.y;

                // ���ʕ\���F90�x����270�x�̊Ԃ������ʂ�\��
                if (yRotation >= 90 && yRotation <= 270)
                {
                    frontImage.enabled = false;
                    backImage.enabled = true;
                }
                else
                {
                    frontImage.enabled = true;
                    backImage.enabled = false;
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
    /// <summary>
    /// �X�v���C�g�������_���ɕύX����@�A�����ē����̂��I�΂�Ȃ��悤�ɂ��Ă���
    /// </summary>
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

    /// <summary>
    /// �V�[���ړ��@�z�[�����
    /// </summary>
    public void changeScene()
    {
        SceneManager.LoadScene("Home");
    }

    /// <summary>
    /// �Q�[���I��
    /// </summary>
    public void gameEnd()
    {
#if UNITY_EDITOR
        // Unity�G�f�B�^�[�ł̓���
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ���ۂ̃Q�[���I������
        Application.Quit();
#endif
    }
}
