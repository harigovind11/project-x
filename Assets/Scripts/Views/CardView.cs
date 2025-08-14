using UnityEngine;
using System.Collections;

public class CardView : MonoBehaviour
{
    public CardData CardData { get; private set; }
    public bool IsMatched { get; private set; }

    [Header("Card View Properties")]
    [SerializeField] private Sprite _cardFace;
    [SerializeField] private Sprite _cardBack;
    [SerializeField] private SpriteRenderer _icon;
    [SerializeField] private float _flipDuration = 0.5f;

    [Header("Effects")]
    [SerializeField] private GameObject _matchEffectPrefab;
    [SerializeField] private float _matchAnimationDuration = 0.3f;

    //Private Animation State 
    private bool _isFlipping;
    private bool _faceUp;
    private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(CardData cardData)
    {
        this.CardData = cardData;
        this._icon.sprite = cardData.cardSprite;
        IsMatched = false;
        _isFlipping = false;
        _faceUp = false;
        _spriteRenderer.sprite = _cardBack;
        _icon.gameObject.SetActive(false);
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);
    }

    //Input
    private void OnMouseDown()
    {
        if (!_isFlipping && !IsMatched)
        {
            EventManager.RaiseCardFlipped(this);
        }
    }

    // Actions
    public void Flip()
    {
        if (!_isFlipping)
        {
            StartCoroutine(FlipCard());
        }
    }

    public void SetAsMatched()
    {
        if (!IsMatched)
        {
            IsMatched = true;

            if (_matchEffectPrefab != null)
            {
                Instantiate(_matchEffectPrefab, transform.position, Quaternion.identity);
            }

            StartCoroutine(ScaleOutAndDeactivate());
        }
    }

    // Animation Coroutines
    private IEnumerator FlipCard()
    {
        _isFlipping = true;

        Vector3 originalScale = transform.localScale;
        yield return StartCoroutine(FlipHalf(_flipDuration / 2, originalScale.x, 0f));
        _spriteRenderer.sprite = _faceUp ? _cardBack : _cardFace;
        _icon.gameObject.SetActive(!_faceUp);
        yield return StartCoroutine(FlipHalf(_flipDuration / 2, 0f, originalScale.x));

        _faceUp = !_faceUp;
        _isFlipping = false;
    }

    private IEnumerator FlipHalf(float duration, float startScale, float endScale)
    {
        Vector3 initialScale = transform.localScale;
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            var scale = Mathf.Lerp(startScale, endScale, time / duration);
            transform.localScale = new Vector3(scale, initialScale.y, initialScale.z);
            yield return null;
        }

        transform.localScale = new Vector3(endScale, initialScale.y, initialScale.z);
    }

    private IEnumerator ScaleOutAndDeactivate()
    {
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        for (float time = 0; time < _matchAnimationDuration; time += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, time / _matchAnimationDuration);
            yield return null;
        }

        transform.localScale = targetScale;
        gameObject.SetActive(false);
    }
}