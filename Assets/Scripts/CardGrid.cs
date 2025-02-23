using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardGrid : MonoBehaviour
{
    [Header("Card Grid Settings")]
    public Vector2 areaSize = new Vector2(15f, 7f);// screen size to cover cards
    public float spacingX = -6.61f; // Horizontal spacing between objects
    public float spacingY = 0.2f; // Vertical spacing between objects
    public bool scaleToFit = true; // If true, objects will be scaled to fit within grid cells
    //public float spacingX = 1.5f;//2.4
    //public float spacingY = 2f;//3.3
    [Header("Cards")]
    [SerializeField] private GameObject cardPrefab;
    private readonly List<Card> _cards = new List<Card>();
    private ICardFactory _cardFactory;
    public List<Card> GenerateGrid(int rows, int columns, Sprite[] cardSprites, System.Action<Card> onCardClicked)
    {
        ClearGrid();
        if (_cardFactory == null)
            _cardFactory = new CardFactory(cardPrefab, transform);

        List<(Sprite, ushort)> spritePairs = GenerateCardSpritePairs(rows * columns, cardSprites);


        //float startX = -(columns - 1) * spacingX / 2f;
        //float startY = (rows - 1) * spacingY / 2f;

        // Calculate available cell size considering separate X and Y spacing
        float cellWidth = (areaSize.x - (columns - 1) * spacingX) / columns;
        float cellHeight = (areaSize.y - (rows - 1) * spacingY) / rows;

        int index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                var (sprite, id) = spritePairs[index++];
                Card card = _cardFactory.CreateCard(sprite, id);
                card.OnCardClicked.RemoveAllListeners();
                card.OnCardClicked.AddListener(onCardClicked.Invoke);
                card.FlipBackInstant();
                //card.gameObject.SetActive(true); // Ensure the card is active
                //GridLayout.AddItem(card.gameObject);
                _cards.Add(card);

                // Calculate position with separate spacing for X and Y
                float xPos = col * (cellWidth + spacingX) - (areaSize.x / 2) + cellWidth / 2;
                float yPos = row * (cellHeight + spacingY) - (areaSize.y / 2) + cellHeight / 2;
                Vector3 position = new Vector3(xPos, yPos, 0f);
                //card.transform.position = new Vector3(startX + col * spacingX, startY - row * spacingY, 0);

                // Move the existing object to the calculated position
                Transform obj = card.transform;
                obj.position = position;

                // Reset scale before applying new one
                obj.localScale = Vector3.one;

                // Scale the object to fit within its cell
                if (scaleToFit && obj.GetComponent<Renderer>())
                {
                    Vector3 originalSize = obj.GetComponent<Renderer>().bounds.size;
                    float scaleX = cellWidth / originalSize.x;
                    float scaleY = cellHeight / originalSize.y;
                    float scale = Mathf.Min(scaleX, scaleY); // Maintain aspect ratio
                    obj.localScale *= scale;
                }
            }
        }

        return _cards;
    }

    private void ClearGrid()
    {
        foreach (Card card in _cards)
            _cardFactory.ReturnCard(card);

        _cards.Clear();
    }

    private List<(Sprite, ushort)> GenerateCardSpritePairs(int totalCards, Sprite[] cardSprites)
    {
        int pairCount = totalCards / 2;
        List<(Sprite, ushort)> spritePairs = new List<(Sprite, ushort)>();

        ushort currentID = 0;
        for (int i = 0; i < pairCount; i++)
        {
            spritePairs.Add((cardSprites[i], currentID));
            spritePairs.Add((cardSprites[i], currentID));
            currentID++;
        }

        Shuffle(spritePairs);
        return spritePairs;
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            int k = Random.Range(0, n--);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
