using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [Header("Sprite Renderers")]
    [SerializeField] private SpriteRenderer frontSpriteRenderer;
    [SerializeField] private SpriteRenderer backSpriteRenderer;

    [Header("Collider")]
    [SerializeField] private BoxCollider2D _collider;

    private Transform _cachedTransform;
    private Vector3 orignalScale;

    public ushort Id { get; private set; }
    public bool IsMatched { get; private set; }
    public bool IsFlipping { get; private set; }

    public UnityEvent<Card> OnCardClicked = new UnityEvent<Card>();

    private const float FlipDuration = 0.2f; // Faster and more responsive
    private const float FadeDuration = 0.3f;

    private void Awake()
    {
        _cachedTransform = transform;
        
    }
    private void Start()
    {
        orignalScale = transform.localScale;
    }

    private void OnMouseDown()
    {
        if (!IsMatched && !IsFlipping && _collider.enabled)
            OnCardClicked?.Invoke(this);
    }

    public void Initialize(Sprite sprite, ushort id)
    {
        Id = id;
        frontSpriteRenderer.sprite = sprite;
        IsMatched = false;
        FlipBackInstant();
    }

    public void Flip(bool faceUp)
    {
        if (IsMatched || IsFlipping) return;
        IsFlipping = true;

        _cachedTransform.DOScaleX(0f, FlipDuration / 2).SetEase(Ease.InQuad).OnComplete(() =>
        {
            backSpriteRenderer.gameObject.SetActive(!faceUp);

            _cachedTransform.DOScaleX(orignalScale.x, FlipDuration / 2).SetEase(Ease.OutQuad).OnComplete(() => IsFlipping = false);
        });
    }
    //[ContextMenu("Flip card instant")]
    public void FlipBackInstant()
    {
        backSpriteRenderer.gameObject.SetActive(true);
        frontSpriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Reset alpha

        _cachedTransform.localScale = orignalScale;

        IsFlipping = false;
        IsMatched = false;
        _collider.enabled = true;
    }

    public void Match()
    {
        IsMatched = true;
        frontSpriteRenderer.DOFade(0f, FadeDuration).OnComplete(() => _collider.enabled = false);
    }

    public void ResetCard(Sprite sprite, ushort id)
    {
        Id = id;
        frontSpriteRenderer.sprite = sprite;

        frontSpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        //frontSpriteRenderer.gameObject.SetActive(false);
        backSpriteRenderer.gameObject.SetActive(true);

        IsMatched = false;
        IsFlipping = false;

        _cachedTransform.localScale = orignalScale;
        _collider.enabled = true;
    }
}
