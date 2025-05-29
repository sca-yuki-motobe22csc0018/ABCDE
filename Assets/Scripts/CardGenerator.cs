using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CardGenerator : MonoBehaviour
{
    public SpriteRenderer imageSpriteRenderer;
    public SpriteRenderer typeSpriteRenderer;
    public Text costText;
    public Text nameText;
    public Text textText;

    public int baseSortingOrder = 0;
    private int lastSortingOrder = -9999;

    [System.Serializable]
    public class CardData
    {
        public int id;
        public string name;
        public string type;
        public string rarity;
        public int cost;
        public string text;
        public string image;
    }

    public List<CardData> cardList = new List<CardData>();

    void Start()
    {
        int cardID = int.Parse(this.name);
        cardID = (cardID > 0) ? cardID - 1 : 1;

        LoadCSV();

        costText.text = cardList[cardID].cost.ToString();
        nameText.text = cardList[cardID].name;
        textText.text = cardList[cardID].text;

        Sprite imageSprite = Resources.Load<Sprite>("CardImages/" + cardList[cardID].image);
        if (imageSprite != null)
            imageSpriteRenderer.sprite = imageSprite;
        else
            Debug.LogWarning("‰æ‘œ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ: " + cardList[cardID].image);

        Sprite typeSprite = Resources.Load<Sprite>("CardTypes/Card_Type_" + cardList[cardID].type);
        if (typeSprite != null)
            typeSpriteRenderer.sprite = typeSprite;
        else
            Debug.LogWarning("ƒ^ƒCƒv‰æ‘œ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ: " + cardList[cardID].type);

        this.name = cardList[cardID].name;

        lastSortingOrder = baseSortingOrder;
        UpdateChildOrders();
    }

    private void Update()
    {
        if (transform.parent != null)
        {
            SpriteRenderer parentRenderer = transform.parent.GetComponent<SpriteRenderer>();

            if (parentRenderer != null)
            {
                int parentOrder = parentRenderer.sortingOrder;
                lastSortingOrder+=parentOrder;
            }
        }

        if (baseSortingOrder != lastSortingOrder)
        {
            UpdateChildOrders();
            lastSortingOrder = baseSortingOrder;
        }
    }

    void UpdateChildOrders()
    {
        foreach (Transform child in transform)
        {
            int offset = 0;

            SortingOffset so = child.GetComponent<SortingOffset>();
            if (so != null)
            {
                offset = so.orderOffset;
            }

            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = baseSortingOrder + offset;
            }

            Canvas childCanvas = child.GetComponent<Canvas>();
            if (childCanvas != null)
            {
                childCanvas.overrideSorting = true;
                childCanvas.sortingOrder = baseSortingOrder + offset;
            }
        }
    }

    void LoadCSV()
    {
        string path = Application.streamingAssetsPath + "/Card_Data.csv";
        string csvText = File.ReadAllText(path, Encoding.UTF8);
        List<string[]> rows = ParseCsv(csvText);

        for (int i = 1; i < rows.Count; i++)
        {
            string[] values = rows[i];
            if (values.Length < 7) continue;

            CardData data = new CardData();
            data.id = int.Parse(values[0]);
            data.name = values[1];
            data.type = values[2];
            data.rarity = values[3];
            data.cost = int.Parse(values[4]);
            data.text = values[5];
            data.image = values[6];

            cardList.Add(data);
        }
    }

    List<string[]> ParseCsv(string csvText)
    {
        var rows = new List<string[]>();
        var currentRow = new List<string>();
        var currentField = new StringBuilder();

        bool inQuotes = false;
        for (int i = 0; i < csvText.Length; i++)
        {
            char c = csvText[i];
            char next = i + 1 < csvText.Length ? csvText[i + 1] : '\0';

            if (inQuotes)
            {
                if (c == '"' && next == '"') { currentField.Append('"'); i++; }
                else if (c == '"') inQuotes = false;
                else currentField.Append(c);
            }
            else
            {
                if (c == '"') inQuotes = true;
                else if (c == ',') { currentRow.Add(currentField.ToString()); currentField.Clear(); }
                else if (c == '\r' && next == '\n') { currentRow.Add(currentField.ToString()); rows.Add(currentRow.ToArray()); currentRow = new List<string>(); currentField.Clear(); i++; }
                else if (c == '\n' || c == '\r') { currentRow.Add(currentField.ToString()); rows.Add(currentRow.ToArray()); currentRow = new List<string>(); currentField.Clear(); }
                else currentField.Append(c);
            }
        }

        if (currentField.Length > 0 || currentRow.Count > 0)
        {
            currentRow.Add(currentField.ToString());
            rows.Add(currentRow.ToArray());
        }

        return rows;
    }
}