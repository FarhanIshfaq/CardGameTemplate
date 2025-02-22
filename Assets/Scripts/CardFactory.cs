using UnityEngine;

public interface ICardFactory
{
    Card CreateCard(Sprite sprite, ushort id, System.Action<Card> onClickCallback);
}

public class CardFactory : ICardFactory
{
    private GameObject _cardPrefab;
    private Transform _parentTransform;

    public CardFactory(GameObject cardPrefab, Transform parentTransform)
    {
        _cardPrefab = cardPrefab;
        _parentTransform = parentTransform;
    }

    public Card CreateCard(Sprite sprite, ushort id, System.Action<Card> onClickCallback)
    {
        GameObject cardObject = Object.Instantiate(_cardPrefab, _parentTransform);
        Card card = cardObject.GetComponent<Card>();
        card.Initialize(sprite, id, onClickCallback);
        return card;
    }
}
