using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private PlayerHealth playerHealth;

    private bool isGameOver;
    private bool isPaused;

    private void Awake()
    {
        if (playerHealth == null) playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (playerHealth != null) playerHealth.GameOver += ShowLose;
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.GameOver -= ShowLose;
    }

    private void Start()
    {
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        isGameOver = false;
        isPaused = false;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (isGameOver) return;
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    public void ShowWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (winPanel) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void ShowLose()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (losePanel) losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pausePanel) pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        isPaused = false;
        if (pausePanel) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenSettings()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(true);
        if (musicSlider) musicSlider.SetValueWithoutNotify(AudioManager.MusicVolume);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(AudioManager.SFXVolume);
    }

    public void CloseSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(true);
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.SetMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.SetSFXVolume(volume);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
