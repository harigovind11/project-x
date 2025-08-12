using UnityEngine;
using System.Collections;

public class CardView : MonoBehaviour
{
    public CardData CardData { get; private set; }
    public bool IsCollected { get; private set; }

    [Header("Card View Properties")]
    [SerializeField] private Sprite _cardFace;
    [SerializeField] private Sprite _cardBack;
    [SerializeField] private SpriteRenderer _icon;
    [SerializeField] private float _flipDuration = 0.5f; // Your flip animation duration

    //Private Animation State 
    private bool _isFlipping;
    private bool _faceUp;
    private SpriteRenderer _spriteRenderer;


    void Awake()
    {

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    //Input
    private void OnMouseDown()
    {

        if (!_isFlipping && !IsCollected)
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
        if (!IsCollected)
        {
            IsCollected = true;
            // particle effects and scale animation here
            gameObject.SetActive(false);
        }
    }

    // Animation Coroutine

    private IEnumerator FlipCard()
    {
        _isFlipping = true;

        yield return StartCoroutine(FlipHalf(_flipDuration / 2, 1f, 0f));
        _spriteRenderer.sprite = _faceUp ? _cardBack : _cardFace;
        _icon.gameObject.SetActive(!_faceUp);
        yield return StartCoroutine(FlipHalf(_flipDuration / 2, 0f, 1f));

        _faceUp = !_faceUp;
        _isFlipping = false;
    }

    private IEnumerator FlipHalf(float duration, float startScale, float endScale)
    {
        Vector3 initialScale = transform.localScale;
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            var scaleX = Mathf.Lerp(startScale, endScale, time / duration);
            transform.localScale = new Vector3(scaleX * initialScale.x, initialScale.y, initialScale.z);
            yield return null;
        }
        transform.localScale = new Vector3(endScale * initialScale.x, initialScale.y, initialScale.z);
    }

}