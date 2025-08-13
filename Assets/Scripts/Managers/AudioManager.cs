using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [Tooltip("The sound that plays when any card is flipped.")]
    [SerializeField] private AudioClip cardFlipSound;

    [Tooltip("The sound that plays when a correct match is found.")]
    [SerializeField] private AudioClip matchSuccessfulSound;

    [Tooltip("The sound that plays when the two flipped cards do not match.")]
    [SerializeField] private AudioClip mismatchSound;

    [Tooltip("The sound that plays when the level is completed.")]
    [SerializeField] private AudioClip levelCompleteSound;

    [Tooltip("The sound that plays when the game is won or lost.")]
    [SerializeField] private AudioClip gameOverSound;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        EventManager.OnCardFlipped += HandleCardFlipped;
        EventManager.OnMatchFound += HandleMatchFound;
        EventManager.OnMatchFailed += HandleMismatchFound;
        EventManager.OnGameWon += HandleGameOver;
        EventManager.OnGameLost += HandleGameOver;
    }

    // Always unsubscribe from events when the object is disabled to prevent errors.
    void OnDisable()
    {
        EventManager.OnCardFlipped -= HandleCardFlipped;
        EventManager.OnMatchFound -= HandleMatchFound;
        EventManager.OnMatchFailed -= HandleMismatchFound;
        EventManager.OnGameWon -= HandleGameWon;
        EventManager.OnGameLost -= HandleGameOver;
    }

    // Event handlers to play sounds
    private void HandleCardFlipped(CardView _)
    {
        PlaySound(cardFlipSound);
    }

    private void HandleMatchFound(CardData _)
    {
        PlaySound(matchSuccessfulSound);
    }

    private void HandleMismatchFound()
    {
        PlaySound(mismatchSound);
    }

    private void HandleGameWon()
    {
        PlaySound(levelCompleteSound);
    }

    private void HandleGameOver()
    {
        PlaySound(gameOverSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}