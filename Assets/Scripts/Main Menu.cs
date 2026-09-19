using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("UI Panls")]

    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;



    [Header("Audio Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;



    [Header("Start Game")]
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private string cutsceneScene = "cutScene";
    [SerializeField] private float fadeOutDuration = 0.7f;

    private bool isGameOver = false;
    private bool isGameStarted = false;
    private bool pausedForSettings = false;
    private bool isStarting = false;


    void Start()
    {
        ShowMainMenu();
    }

    //1- Main Menu Panel
    public void StartGame()
    {
        if (isStarting) return;   // ignore extra clicks while the screen is fading
        isStarting = true;
        StartCoroutine(StartGameRoutine());
    }

    // Fade the menu to black, then go to the intro cutscene (which loads the game when it finishes).
    private IEnumerator StartGameRoutine()
    {
        if (screenFader != null) yield return screenFader.FadeTo(1f, fadeOutDuration);

        isGameStarted = true;
        isGameOver = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(cutsceneScene);
    }
    public void ShowMainMenu()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(true);

        if (settingsPanel) settingsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (instructionsPanel) instructionsPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);

        isGameStarted = false;
        isGameOver = false;
        Time.timeScale = 0f;
    }
    //2- Settings Panel
    public void OpenSettings()
    {
        pausedForSettings = pausePanel != null && pausePanel.activeSelf;

        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);

        if (settingsPanel) settingsPanel.SetActive(true);

        if (musicSlider) musicSlider.SetValueWithoutNotify(AudioManager.MusicVolume);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(AudioManager.SFXVolume);
    }
    public void CloseSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
        if (pausedForSettings)
        {
            if (pausePanel) pausePanel.SetActive(true);
        }
        else
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(true);
        }
    }
    //3- Instructions Panel
    public void OpenInstructions()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (instructionsPanel) instructionsPanel.SetActive(true);

    }
    //4- Credits Panel
    public void OpenCredits()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(true);
    }
    public void BackToMainMenu()
    {
        if (creditsPanel) creditsPanel.SetActive(false);
        if (instructionsPanel) instructionsPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);

        if (mainMenuPanel) mainMenuPanel.SetActive(true);
    }
    //5- pause panel
    public void OpenPausePanel()
    {
        if (!isGameStarted || isGameOver) return;
        if (pausePanel) pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ClosePausePanel()
    {
        if (pausePanel) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    //6- win panel
    public void ShowWinPanel()
    {
        isGameOver = true;
        if (pausePanel) pausePanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    //7- lose panel
    public void ShowLosePanel()
    {
        isGameOver = true;
        if (losePanel) losePanel.SetActive(true);
        if (pausePanel) pausePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && isGameStarted && !isGameOver)
        {
            if (settingsPanel && settingsPanel.activeSelf && pausedForSettings)
            {
                CloseSettings();
            }
            else if (pausePanel && pausePanel.activeSelf)
            {
                ClosePausePanel();
            }
            else
            {
                OpenPausePanel();
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // Quit Game we might add it to the main menu
    public void QuitGame()
    {
        Application.Quit();
    }
    public void SetMusicVolume(float volume)
    {
        AudioManager.SetMusicVolume(volume);
    }
    public void SetSFXVolume(float volume)
    {
        AudioManager.SetSFXVolume(volume);
    }

}
