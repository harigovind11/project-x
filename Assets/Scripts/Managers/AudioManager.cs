using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource; 
    [SerializeField] private AudioSource sfxSource;  
    
    [Header("Music Tracks")]
    
    [Tooltip("The music that plays in the main menu.")]
    [SerializeField] private AudioClip menuMusic;

    [Tooltip("The music that plays during gameplay.")]
    [SerializeField] private AudioClip gameplayMusic;


    [Header("Sound Effects (SFX)")]
    
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

  

    void Start()
    {
        PlayMusic(menuMusic);
    }

    void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChanged;
        
        EventManager.OnPlayerFlipSound += HandleCardFlipped;
        EventManager.OnMatchFound += HandleMatchFound;
        EventManager.OnMatchFailed += HandleMismatchFound;
        EventManager.OnGameWon += HandleGameOver;
        EventManager.OnGameLost += HandleGameOver;
        GameStateManager.OnGameStateChanged += HandleGameStateChanged;
    }
    
    void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChanged;
        
        EventManager.OnPlayerFlipSound -= HandleCardFlipped;
        EventManager.OnMatchFound -= HandleMatchFound;
        EventManager.OnMatchFailed -= HandleMismatchFound;
        EventManager.OnGameWon -= HandleGameWon;
        EventManager.OnGameLost -= HandleGameOver;
    }

    // Event handlers to play sounds

    void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.MainMenu)
        {
   
           PlayMusic(menuMusic);
        }
        else if (newState == GameState.Gameplay)
        {
   
            PlayMusic(gameplayMusic);
        }
    }
    private void HandleCardFlipped()
    {
        PlaySfx(cardFlipSound);
    }

    private void HandleMatchFound(CardData _)
    {
        PlaySfx(matchSuccessfulSound);
    }

    private void HandleMismatchFound()
    {
        PlaySfx(mismatchSound);
    }

    private void HandleGameWon()
    {
        PlaySfx(levelCompleteSound);
    }

    private void HandleGameOver()
    {
        PlaySfx(gameOverSound);
    }

 
    private void PlaySfx(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    private void PlayMusic(AudioClip musicClip)
    {
        if (musicSource != null && musicClip != null)
        {
            if (musicSource.clip == musicClip && musicSource.isPlaying)
            {
                return;
            }

            musicSource.clip = musicClip;
            musicSource.loop = true; 
            musicSource.Play();
        }
    }

}