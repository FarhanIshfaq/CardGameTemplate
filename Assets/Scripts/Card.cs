using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField] private Image frontImage;
    [SerializeField] private Button cardButton;

    private Transform _cachedTransform;

    public ushort Id { get; private set; }
    public bool IsMatched { get; private set; }
    public bool IsFlipping { get; private set; }

    public UnityEvent<Card> OnCardClicked = new UnityEvent<Card>();

    private const float FlipDuration = 0.3f;
    private const float FadeDuration = 0.4f;

    private void Awake() => _cachedTransform = transform;

    public void Initialize(Sprite sprite, ushort id)
    {
        Id = id;
        frontImage.sprite = sprite;
        IsMatched = false;

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(() =>
        {
            if (!IsMatched && !IsFlipping)
                OnCardClicked?.Invoke(this);
        });

        FlipBackInstant();
    }

    public void Flip(bool faceUp)
    {
        if (IsMatched || IsFlipping) return;
        IsFlipping = true;

        _cachedTransform.DOScaleX(0f, FlipDuration / 2).SetEase(Ease.InQuad).OnComplete(() =>
        {
            frontImage.gameObject.SetActive(faceUp);
            _cachedTransform.DOScaleX(1f, FlipDuration / 2).SetEase(Ease.OutQuad).OnComplete(() => IsFlipping = false);
        });
    }

    public void FlipBackInstant()
    {
        frontImage.gameObject.SetActive(false);
        frontImage.DOFade(1f, 0f); // Reset fade effect instantly
        _cachedTransform.localScale = Vector3.one;
        IsFlipping = false;
        IsMatched = false;
        cardButton.interactable = true; // Ensure the card is interactable
    }

    public void Match()
    {
        IsMatched = true;
        frontImage.DOFade(0f, FadeDuration).OnComplete(() => cardButton.interactable = false);
    }

    /// <summary>
    /// Reset the card completely during game reset (visuals, states, and click events).
    /// </summary>
    public void ResetCard(Sprite sprite, ushort id)
    {
        Id = id;
        frontImage.sprite = sprite;
        frontImage.DOFade(1f, 0f); // Reset fade effect
        frontImage.gameObject.SetActive(false);

        IsMatched = false;
        IsFlipping = false;

        _cachedTransform.localScale = Vector3.one;
        cardButton.interactable = true;

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(() =>
        {
            if (!IsMatched && !IsFlipping)
                OnCardClicked?.Invoke(this);
        });
    }
}
