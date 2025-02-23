using UnityEngine;

public class BonusCardFactory : ICardFactory
{
    private GameObject _bonusCardPrefab;
    private Transform _parentTransform;

    public BonusCardFactory(GameObject bonusCardPrefab, Transform parentTransform)
    {
        _bonusCardPrefab = bonusCardPrefab;
        _parentTransform = parentTransform;
    }

    public Card CreateCard(Sprite sprite, ushort id)
    {
        GameObject cardObject = Object.Instantiate(_bonusCardPrefab, _parentTransform);
        Card card = cardObject.GetComponent<Card>();
        card.Initialize(sprite, id);
        return card;
    }
    public void ReturnCard(Card card)
    {
        card.gameObject.SetActive(false);
    }
}
