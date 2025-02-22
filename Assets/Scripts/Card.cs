using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image frontImage;
    [SerializeField] private Button cardButton;

    public ushort Id { get; private set; }
    public bool IsMatched { get; private set; }
    private System.Action<Card> onCardClicked;

    public void Initialize(Sprite sprite, ushort id, System.Action<Card> onClickCallback)
    {
        Id = id;
        frontImage.sprite = sprite;
        onCardClicked = onClickCallback;
        IsMatched = false;

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(() => { if (!IsMatched) onCardClicked?.Invoke(this); });

        FlipBackInstant();
    }

    public void Flip() => frontImage.gameObject.SetActive(true);

    public void FlipBack() => frontImage.gameObject.SetActive(false);

    public void FlipBackInstant() => frontImage.gameObject.SetActive(false);

    public void Match()
    {
        IsMatched = true;
        frontImage.color = new Color(1, 1, 1, 0.6f);
        cardButton.interactable = false;
    }
}
