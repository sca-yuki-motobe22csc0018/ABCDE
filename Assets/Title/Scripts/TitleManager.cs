using DG.Tweening;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Text clickStart;//clickstartText
    [SerializeField] Text title;//titleText
    public Sprite[] titleImages; // �摜�𕡐��o�^
    public Image targetImage;    // �\��������Image�R���|�[�l���g
    //private int centerX = 960;//canvas�p���S���WX
    //private int centerY = 540;//canvas�p���S���WY

    private bool clickCheck = false;//���N���b�N�ɂ��V�[���ړ��딚�j�~�p
    private float startAlpha = 1.0f;//�N���b�N�X�^�[�g�̎n�܂鎞�̃A���t�@�l
    private float alpha = 0.2f;//ClickStart�p
    private float alphaTime = 1.0f;//ClickStart�p�@�����ɂ����鎞��
    private int loop = -1;//ClickStart�p�@���[�v

    private float titleAlpha = 1.0f;//�^�C�g���̃A���t�@�l
    private float titleTime = 1.0f;//fade�ɂ����鎞��

    private float titleImageAlpha = 1.0f;//�^�C�g��image�̃A���t�@�l

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (titleImages.Length > 0 && targetImage != null)
        {
            for (int i = 0; i < titleImages.Length; i++) 
            {
                int index = Random.Range(0, titleImages.Length);
                targetImage.sprite = titleImages[index];
            }
        }
        Fade();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameEnd();
        }

        if (clickCheck)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                changeScene();
            }
        }
    }

    void TitleMove()
    {
        clickCheck = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(clickStart.DOFade(alpha, alphaTime).SetEase(Ease.InSine));
        seq.SetLoops(loop, LoopType.Yoyo);
    }

    void Fade()
    {
        Sequence fade = DOTween.Sequence();
        fade.Append(title.DOFade(titleAlpha, titleTime).SetEase(Ease.InSine));
        fade.Join(clickStart.DOFade(startAlpha, titleTime).SetEase(Ease.InSine));
        fade.Join(targetImage.DOFade(titleImageAlpha, titleTime).SetEase(Ease.InSine));
        fade.OnComplete(() => TitleMove());

    }

    public void changeScene()
    {
        SceneManager.LoadScene("Home");
    }

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
