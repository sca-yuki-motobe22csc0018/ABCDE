using DG.Tweening;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Text clickStart;//clickstartText
    [SerializeField] Text title;//titleText
    public Sprite[] titleImages; // 画像を複数登録
    public Image targetImage;    // 表示させるImageコンポーネント
    //private int centerX = 960;//canvas用中心座標X
    //private int centerY = 540;//canvas用中心座標Y

    private bool clickCheck = false;//左クリックによるシーン移動誤爆阻止用
    private float startAlpha = 1.0f;//クリックスタートの始まる時のアルファ値
    private float alpha = 0.2f;//ClickStart用
    private float alphaTime = 1.0f;//ClickStart用　動きにかかる時間
    private int loop = -1;//ClickStart用　ループ

    private float titleAlpha = 1.0f;//タイトルのアルファ値
    private float titleTime = 1.0f;//fadeにかかる時間

    private float titleImageAlpha = 1.0f;//タイトルimageのアルファ値

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
        // Unityエディターでの動作
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 実際のゲーム終了処理
        Application.Quit();
#endif
    }
}
