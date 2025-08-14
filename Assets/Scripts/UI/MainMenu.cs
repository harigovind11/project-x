using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Button audioButton;
    [SerializeField] private Sprite audioOnSprite;
    [SerializeField] private Sprite audioOffSprite;

    private bool isMuted = false;

    void Start()
    {
     
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
        
        isMuted = AudioListener.volume == 0;
        UpdateButtonSprite();
    }

    // Public Methods for Buttons

    public void OnPlayButton()
    {
        GameStateManager.Instance.ChangeState(GameState.Gameplay);
    }

    public void OnInfoButton()
    {
       
        if (infoPanel != null)
        {
            infoPanel.SetActive(!infoPanel.activeSelf);
        }
    }

    public void OnAudioToggleButton()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0f : 1f;
        UpdateButtonSprite();
    }

    // Helper Methods

    private void UpdateButtonSprite()
    {
        if (audioButton != null && audioOnSprite != null && audioOffSprite != null)
        {
            
            audioButton.GetComponent<Image>().sprite = isMuted ? audioOffSprite : audioOnSprite;
        }
    }
}