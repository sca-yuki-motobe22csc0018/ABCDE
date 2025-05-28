using System.Collections.Generic;
using UnityEngine;

public class LayerChanger : MonoBehaviour
{
    public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    void Start()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
            spriteRenderers.Add(child.GetComponent<SpriteRenderer>());
        }
    }

    void Update()
    {

    }
}
