using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン遷移を管理するクラス。DontDestroyOnLoadで永続化。
/// </summary>
public class SceneController : MonoBehaviour
{
    void Awake()
    {
        Locator.Register<SceneController>(this);
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// シーン名を指定してロード
    /// </summary>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
