using DG.Tweening;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Text clickStart;//clickstartText
    [SerializeField] Text title;//titleText
    //追加
    public Image frontImage;         // 表面のImage
    public Image backImage;          // 裏面のImage
    public Sprite[] cardSprites;     // 表面のイラストたち
    
    public GameObject card;//回転するカード

    private int lastIndex = -1;//直前に選ばれたイラストのインデックスを記憶する変数


    //ここまで

    //public Sprite[] titleImages; // 表面のイラスト複数登録
    //public Image targetImage;    // 表示させるImageコンポーネント

    private float clickStartAlpha = 0.2f;//ClickStart用　アルファ値
    private float alphaTime = 1.0f;//ClickStart用　動きにかかる時間

    private int loop = -1;//ループ

    private float titleTime = 1.0f;//fadeにかかる時間
    private float fadeAlpha = 1.0f;//fade用アルファ値

    private bool clickCheck = false;//左クリックによるシーン移動誤爆阻止用

    void Start()
    {
        // 最初は表面表示、裏面非表示
        frontImage.enabled = true;
        backImage.enabled = false;

        ChangeRandomSprite();
        Fade();
        LoopRotation();
    }

    // Update is called once per frame
    void Update()
    {
        //ゲーム終了呼び出し
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameEnd();
        }
        //タイトル見えるまでクリック受付阻止用
        if (clickCheck)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                changeScene();
            }
        }
    }

    /// <summary>
    /// clickstartの点滅
    /// </summary>
    void TitleMove()
    {
        clickCheck = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(clickStart.DOFade(clickStartAlpha, alphaTime).SetEase(Ease.InSine));
        seq.SetLoops(loop, LoopType.Yoyo);
    }

    /// <summary>
    /// タイトル画面のアルファ値の変化
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
    /// ディレイ　２秒後にRotateCardを呼ぶ
    /// </summary>
    private void LoopRotation()
    {
        DOVirtual.DelayedCall(4f, () =>
        {
            RotateCard();
        });
    }
    /// <summary>
    /// カードを回転させる　ChangeRandomSpriteを途中で呼ぶ
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

                // 裏面表示：90度から270度の間だけ裏面を表示
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
    /// <summary>
    /// スプライトをランダムに変更する　連続して同じのが選ばれないようにしてある
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
    /// シーン移動　ホーム画面
    /// </summary>
    public void changeScene()
    {
        SceneManager.LoadScene("Home");
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void gameEnd()
    {
#if UNITY_EDITOR
        // Unityエディターでの動作
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 実際のゲーム終了処理
        Application.Quit();
#endif
    }
}
