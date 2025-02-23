using UnityEngine;
using System.Collections.Generic;

public interface ICardFactory
{
    Card CreateCard(Sprite sprite, ushort id);
    void ReturnCard(Card card);
}

public class CardFactory : ICardFactory
{
    private readonly GameObject _cardPrefab;
    private readonly Transform _parentTransform;

    private readonly Queue<Card> _cardPool = new Queue<Card>();

    public CardFactory(GameObject cardPrefab, Transform parentTransform)
    {
        _cardPrefab = cardPrefab;
        _parentTransform = parentTransform;
    }

    public Card CreateCard(Sprite sprite, ushort id)
    {
        Card card;
        if (_cardPool.Count > 0)
        {
            card = _cardPool.Dequeue();
            card.transform.SetParent(_parentTransform);
            card.gameObject.SetActive(true);
            card.ResetCard(sprite, id); // Reset the card completely
        }
        else
        {
            card = Object.Instantiate(_cardPrefab, _parentTransform).GetComponent<Card>();
            card.Initialize(sprite, id);
        }

        return card;
    }

    public void ReturnCard(Card card)
    {
        card.gameObject.SetActive(false);
        _cardPool.Enqueue(card);
    }
}
