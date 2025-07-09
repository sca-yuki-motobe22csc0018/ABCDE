using UnityEngine;

public class HandPos : MonoBehaviour
{
    public int numberOfCards;
    public GameObject[] posObj;

    public float spacing = 1.5f; // カードの間隔

    void Update()
    {
        int count = Mathf.Clamp(numberOfCards, 0, posObj.Length);

        // ① すべて非表示に
        for (int i = 0; i < posObj.Length; i++)
        {
            posObj[i].SetActive(false);
        }

        // ② 表示するカードの位置を調整して表示
        for (int i = 0; i < count; i++)
        {
            float startX = -((count - 1) * spacing / 2); // 中心基準で配置
            Vector3 newPos = new Vector3(startX + i * spacing, 0, 0); // YとZは固定（必要に応じて調整）
            posObj[i].transform.localPosition = newPos;
            posObj[i].SetActive(true);
        }
    }
}
