using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardGrid : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GridLayoutGroup gridLayoutGroup;

    private readonly List<Card> cards = new List<Card>();
    private ICardFactory cardFactory;

    public List<Card> GenerateGrid(int rows, int columns, Sprite[] cardSprites, System.Action<Card> onCardClicked)
    {
        ClearGrid(); // Return cards to the pool
        if (cardFactory == null)
            cardFactory = new CardFactory(cardPrefab, transform); // Initialize factory once

        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = columns;

        List<Sprite> spriteList = GenerateCardSpritePairs(rows * columns, cardSprites);

        Dictionary<Sprite, ushort> spriteIdMap = new Dictionary<Sprite, ushort>();
        ushort currentID = 0;

        foreach (Sprite sprite in spriteList)
        {
            if (!spriteIdMap.ContainsKey(sprite))
                spriteIdMap[sprite] = currentID++;

            Card card = cardFactory.CreateCard(sprite, spriteIdMap[sprite]);
            card.OnCardClicked.RemoveAllListeners(); // Ensure no duplicate listeners
            card.OnCardClicked.AddListener(onCardClicked.Invoke);
            cards.Add(card);
        }

        return cards;
    }

    private void ClearGrid()
    {
        foreach (Card card in cards)
        {
            cardFactory.ReturnCard(card); // Return card to pool
        }
        cards.Clear();
    }

    private List<Sprite> GenerateCardSpritePairs(int totalCards, Sprite[] cardSprites)
    {
        int pairCount = totalCards / 2;
        List<Sprite> sprites = cardSprites.Take(pairCount).SelectMany(s => new[] { s, s }).ToList();
        Shuffle(sprites);
        return sprites;
    }

    private void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = rng.Next(n--);
            (list[n], list[k]) = (list[k], list[n]);
        }
    }
}
