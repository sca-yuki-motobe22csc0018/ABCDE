using UnityEngine;

public class CardPrefab : MonoBehaviour
{
    public GameObject prefab;
    public int cardID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObject()
    {
        GameObject obj = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        obj.name = cardID.ToString();
    }
}
