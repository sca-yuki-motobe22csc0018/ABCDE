using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Image Card;
    private int centerX = 960;
    private int centerY = 540;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TitleMove();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TitleMove()
    {
        Card.transform.DOMove(new Vector3(centerX, centerY, 0),1).SetEase(Ease.Linear);
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
