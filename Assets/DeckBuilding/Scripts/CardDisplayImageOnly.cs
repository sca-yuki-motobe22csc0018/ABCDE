using UnityEngine;
using UnityEngine.UI;

public class CardDisplayImageOnly : MonoBehaviour
{
    public Image cardImage;

    public void SetCard(CardInfo info)
    {
        cardImage.sprite = info.sprite;
    }
}
