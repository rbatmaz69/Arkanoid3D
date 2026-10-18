using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("Quellen")]
    [SerializeField] private GameManager gameManager;

    [Header("Menüs")]
    [SerializeField] private GameObject dimmer;
    [SerializeField] private GameObject panelPause;
    [SerializeField] private GameObject panelLevel;
    [SerializeField] private GameObject panelGameOver;

    [Header("Texte")]
    [SerializeField] private TMP_Text levelPunkte;
    [SerializeField] private TMP_Text gameOverPunkte;

    private bool paused;

    private void OnEnable()
    {
        if (gameManager != null) gameManager.StateChanged += OnStateChanged;
    }

    private void OnDisable()
    {
        if (gameManager != null) gameManager.StateChanged -= OnStateChanged;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        HideAll();
    }

    private void Update()
    {
        if (gameManager.State != GameState.Playing) return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (paused) Resume(); else Pause();
        }
    }

    private void HideAll()
    {
        dimmer.SetActive(false);
        panelPause.SetActive(false);
        panelLevel.SetActive(false);
        panelGameOver.SetActive(false);
    }

    public void Pause()
    {
        paused = true;
        Time.timeScale = 0f;
        dimmer.SetActive(true);
        panelPause.SetActive(true);
    }

    public void Resume()
    {
        paused = false;
        Time.timeScale = 1f;
        HideAll();
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
        {
            Time.timeScale = 0f;
            dimmer.SetActive(true);
            panelGameOver.SetActive(true);
            gameOverPunkte.text = "ENDPUNKTE " + gameManager.Score.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de-DE"))
            + "\nHIGHSCORE " + gameManager.HighScore.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
        }
        else if (state == GameState.LevelComplete)
        {
            dimmer.SetActive(true);
            panelLevel.SetActive(true);
            levelPunkte.text = "PUNKTE " + gameManager.Score.ToString("N0", System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
        }
    }

    public void NextLevel()
    {
        HideAll();
    }
}