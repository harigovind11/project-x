using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{

    [Header("UI References")]
    [SerializeField] private GameObject mainContentPanel;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Button audioButton;
    [SerializeField] private Sprite audioOnSprite;
    [SerializeField] private Sprite audioOffSprite;

    private bool isMuted = false;

    void Start()
    {

        if (mainContentPanel != null) mainContentPanel.SetActive(true);
        if (infoPanel != null) infoPanel.SetActive(false);

        isMuted = AudioListener.volume == 0;
        UpdateButtonSprite();
    }


    // Public Methods for Buttons

    public void OnPlayButton()
    {
        GameStateManager.Instance.ChangeState(GameState.LevelSelect);
    }
    
 

    public void OnInfoButton()
    {

        if (infoPanel != null)
        {    if (mainContentPanel != null) mainContentPanel.SetActive(false);
            infoPanel.SetActive(!infoPanel.activeSelf);
        }
    }
    public void CloseInfoPanel()
    {
        if (infoPanel != null) infoPanel.SetActive(false);
        if (mainContentPanel != null) mainContentPanel.SetActive(true);
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