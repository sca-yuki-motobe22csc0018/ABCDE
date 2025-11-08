using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン切り替えを簡単に呼べるヘルパー
/// </summary>
public class SceneController : MonoBehaviour
{
    void Awake()
    {
        Locator.Register<SceneController>(this);
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
