using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HudController : MonoBehaviour
{
    [Header("Quellen")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private LevelGenerator levelGenerator;

    [Header("Punkte")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Leben")]
    [SerializeField] private Image[] lifeIcons;

    [Header("Level")]
    [SerializeField] private TMP_Text levelTitle;
    [SerializeField] private TMP_Text blocksInfo;
    [SerializeField] private Image progressFill;

    private static readonly Color32 Ink = new Color32(30, 34, 53, 255);
    private static readonly Color32 Sand = new Color32(235, 223, 203, 255);

    private int totalBlocks;

    private void OnEnable()
    {
        if (gameManager != null)
        {
            gameManager.ScoreChanged += UpdateScore;
            gameManager.LivesChanged += UpdateLives;
            gameManager.BlocksRemainingChanged += UpdateBlocks;
        }

        if (levelGenerator != null)
        {
            levelGenerator.LevelChanged += UpdateLevel;
        }
    }

    private void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.ScoreChanged -= UpdateScore;
            gameManager.LivesChanged -= UpdateLives;
            gameManager.BlocksRemainingChanged -= UpdateBlocks;
        }

        if (levelGenerator != null)
        {
            levelGenerator.LevelChanged -= UpdateLevel;
        }
    }

    private void Start()
    {
        if (gameManager != null)
        {
            UpdateScore(gameManager.Score);
            UpdateLives(gameManager.Lives);
            UpdateBlocks(gameManager.BlocksRemaining);
        }

        if (levelGenerator != null)
        {
            UpdateLevel(levelGenerator.CurrentLevelNumber);
        }
    }

    private void UpdateScore(int score)
    {
        if (scoreText == null) return;

        scoreText.text = score.ToString("N0", CultureInfo.GetCultureInfo("de-DE"));
    }

    private void UpdateLives(int lives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            bool alive = i < lives;
            lifeIcons[i].color = alive ? Ink : Sand;

            Outline outline = lifeIcons[i].GetComponent<Outline>();
            if (outline != null) outline.enabled = !alive;
        }
    }

    private void UpdateLevel(int levelNumber)
    {
        if (levelTitle == null) return;

        levelTitle.text = "LEVEL " + levelNumber.ToString("00");
    }

    private void UpdateBlocks(int remaining)
    {
        if (remaining > totalBlocks) totalBlocks = remaining;

        if (blocksInfo != null)
        {
            blocksInfo.text = remaining == 1 ? "Noch 1 Block" : "Noch " + remaining + " Blöcke";
        }

        if (progressFill != null)
        {
            progressFill.fillAmount = totalBlocks > 0 ? 1f - (float)remaining / totalBlocks : 0f;
        }

        if (remaining <= 0) totalBlocks = 0;
    }
}