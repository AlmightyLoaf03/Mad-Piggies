using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private int enemiesRemaining = 0;

    [Header("End Panel UI")]
    public GameObject gameCompletePanel;
    public TMPro.TMP_Text finalScoreText;
    public GameObject[] greyStars;  // Assign 3 grey star GameObjects in Inspector
    public GameObject[] filledStars; // Assign 3 filled star GameObjects in Inspector

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterEnemy()
    {
        enemiesRemaining++;
    }

    public void EnemyDefeated()
    {
        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            OnLevelComplete();
        }
    }

    public void CheckForGameOver()
    {
        Debug.Log("Checking game over condition...");
        if (enemiesRemaining > 0)
        {
            Debug.Log("Game over! Enemies left: " + enemiesRemaining);
            ShowGameOverPanel();
        }
    }

    void OnLevelComplete()
    {
        Debug.Log("Level Complete!");

        // Get remaining birds count from Launcher
        int remainingBirds = 0;
        Launcher launcher = FindObjectOfType<Launcher>();
        if (launcher != null)
        {
            remainingBirds = launcher.RemainingBirds;
        }

        // Award points (example: 250 points per remaining bird)
        int pointsForRemainingBirds = remainingBirds * 250;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(pointsForRemainingBirds);
        }

        ShowGameCompletePanel();
    }

    void ShowGameCompletePanel()
    {
        Time.timeScale = 0f;

        if (gameCompletePanel != null)
        {
            gameCompletePanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + ScoreManager.Instance.GetFinalScore();
        }

        ShowStarsBasedOnScore(ScoreManager.Instance.GetFinalScore());
    }

    public void ShowGameOverPanel()
    {
        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    void ShowStarsBasedOnScore(int score)
    {
        int starCount = 0;

        if (score >= 1500) starCount = 3;
        else if (score >= 1000) starCount = 2;
        else if (score >= 500) starCount = 1;

        // Show all grey stars by default
        for (int i = 0; i < greyStars.Length; i++)
        {
            greyStars[i].SetActive(true);
        }

        // Activate filled stars based on starCount, hide the rest
        for (int i = 0; i < filledStars.Length; i++)
        {
            filledStars[i].SetActive(i < starCount);
        }
    }
}
