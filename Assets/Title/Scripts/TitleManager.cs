using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Text clickStart; //clickstartText
    [SerializeField] Text title;      //titleText

    public Image frontImage;          // 表面のImage
    public Image backImage;           // 裏面のImage
    public Sprite[] cardSprites;      // 表面のイラストたち
    public GameObject card;           // 回転するカード

    private int lastIndex = -1;
    private float clickStartAlpha = 0.2f;
    private float alphaTime = 1.0f;
    private int loop = -1;
    private float titleTime = 1.0f;
    private float fadeAlpha = 1.0f;

    private bool clickCheck = false;  // 左クリック受付フラグ
    private bool isRotating = false;  // 回転中フラグ
    private bool isStopped = false;   // ★クリック後に完全停止したか
    private Tween rotationTween;      // ★現在の回転Tweenを記録
    private Tween loopTween;          // ★LoopRotationの遅延呼び出しTweenを記録

    void Start()
    {
        frontImage.enabled = true;
        backImage.enabled = false;

        ChangeRandomSprite();
        Fade();
        LoopRotation();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameEnd();
        }

        // タイトルフェード完了後にクリック受付
        if (clickCheck && Input.GetKeyDown(KeyCode.Mouse0) && !isStopped)
        {
            StopAndFlipToBack(); // ★クリックで停止して裏に回転
        }
    }

    void TitleMove()
    {
        clickCheck = true;
        Sequence seq = DOTween.Sequence();
        seq.Append(clickStart.DOFade(clickStartAlpha, alphaTime).SetEase(Ease.InSine));
        seq.SetLoops(loop, LoopType.Yoyo);
    }

    void Fade()
    {
        Sequence fade = DOTween.Sequence();
        fade.Append(title.DOFade(fadeAlpha, titleTime).SetEase(Ease.InSine));
        fade.Join(clickStart.DOFade(fadeAlpha, titleTime).SetEase(Ease.InSine));
        fade.Join(frontImage.DOFade(fadeAlpha, titleTime).SetEase(Ease.InSine));
        fade.OnComplete(() => TitleMove());
    }

    private void LoopRotation()
    {
        // ★ ループ自体が停止されていない場合のみ次を呼ぶ
        if (isStopped) return;

        loopTween = DOVirtual.DelayedCall(4f, () =>
        {
            RotateCard();
        });
    }

    private void RotateCard()
    {
        if (isStopped) return; // ★停止後なら動かさない

        float duration = 0.5f;
        bool spriteChanged = false;
        isRotating = true;

        rotationTween = card.transform.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                float yRotation = card.transform.eulerAngles.y;

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

                if (!spriteChanged && yRotation > 90)
                {
                    ChangeRandomSprite();
                    spriteChanged = true;
                }
            })
            .OnComplete(() =>
            {
                isRotating = false;
                LoopRotation(); // 再ループ
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

    // ★クリック時に呼ぶ：回転停止＋裏向きに回転アニメーション
    private void StopAndFlipToBack()
    {
        isStopped = true; // 今後ループ呼ばないようにする

        // 回転Tween・遅延Tweenをすべて停止
        if (rotationTween != null && rotationTween.IsActive()) rotationTween.Kill();
        if (loopTween != null && loopTween.IsActive()) loopTween.Kill();

        // 現在の回転角度を取得
        float currentY = card.transform.eulerAngles.y % 360f;
        float targetY;

        // ★どちらの向きでも必ず裏向き(=180度)に回転するようにする
        if (currentY < 180)
            targetY = 180f;
        else
            targetY = 540f; // 一回転分足して自然に裏に向ける

        // 回転アニメーション
        card.transform
            .DORotate(new Vector3(0, targetY, 0), 0.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() =>
            {
                float yRot = card.transform.eulerAngles.y % 360f;
                if (yRot >= 90 && yRot <= 270)
                {
                    frontImage.enabled = false;
                    backImage.enabled = true;
                }
                else
                {
                    frontImage.enabled = true;
                    backImage.enabled = false;
                }
            })
            .OnComplete(() =>
            {
                // 最終的に裏面で固定
                card.transform.rotation = Quaternion.Euler(0, 180, 0);
                frontImage.enabled = false;
                backImage.enabled = true;
                isRotating = false;
            });
    }

    public void changeScene()
    {
        SceneManager.LoadScene("Home");
    }

    public void gameEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
