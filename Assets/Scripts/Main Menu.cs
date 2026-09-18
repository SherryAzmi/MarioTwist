using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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



    private bool isGameOver = false;
    private bool isGameStarted = false;
    private bool pausedForSettings = false;


    void Start()
    {
        ShowMainMenu();
    }

    //1- Main Menu Panel
    public void StartGame()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (instructionsPanel) instructionsPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);

        isGameStarted = true;
        isGameOver = false;
        Time.timeScale = 1f;
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
        pausedForSettings = pausePanel.activeSelf;

        mainMenuPanel.SetActive(false);
        pausePanel.SetActive(false);

        settingsPanel.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (pausedForSettings)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            mainMenuPanel.SetActive(true);
        }
    }
    //3- Instructions Panel
    public void OpenInstructions()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);

    }
    //4- Credits Panel
    public void OpenCredits()
    {
        mainMenuPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }
    public void BackToMainMenu()
    {
        creditsPanel.SetActive(false);
        instructionsPanel.SetActive(false);
        settingsPanel.SetActive(false);

        mainMenuPanel.SetActive(true);
    }
    //5- pause panel
    public void OpenPausePanel()
    {
        if (!isGameStarted || isGameOver) return;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ClosePausePanel()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    //6- win panel
    public void ShowWinPanel()
    {
        isGameOver = true;
        pausePanel.SetActive(false);
        losePanel.SetActive(false);
        winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    //7- lose panel
    public void ShowLosePanel()
    {
        isGameOver = true;
        losePanel.SetActive(true);
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isGameStarted && !isGameOver)
        {
            if (settingsPanel.activeSelf && pausedForSettings)
            {
                CloseSettings();
            }
            else if (pausePanel.activeSelf)
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
}
