using UnityEngine;

public class HandPos : MonoBehaviour
{
    public int numberOfCards;
    public GameObject[] posObj;

    public float spacing = 1.5f; // �J�[�h�̊Ԋu

    void Update()
    {
        int count = Mathf.Clamp(numberOfCards, 0, posObj.Length);

        // �@ ���ׂĔ�\����
        for (int i = 0; i < posObj.Length; i++)
        {
            posObj[i].SetActive(false);
        }

        // �A �\������J�[�h�̈ʒu�𒲐����ĕ\��
        for (int i = 0; i < count; i++)
        {
            float startX = -((count - 1) * spacing / 2); // ���S��Ŕz�u
            Vector3 newPos = new Vector3(startX + i * spacing, 0, 0); // Y��Z�͌Œ�i�K�v�ɉ����Ē����j
            posObj[i].transform.localPosition = newPos;
            posObj[i].SetActive(true);
        }
    }
}
