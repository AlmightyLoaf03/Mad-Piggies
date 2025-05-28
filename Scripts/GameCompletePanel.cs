using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCompletePanel : MonoBehaviour
{
    [Header("UI References")]
    public GameObject endPanel;
    public Image[] stars; // Assign 3 stars in order: left to right
    public Sprite filledStarSprite;
    public Sprite emptyStarSprite;
    public TMP_Text finalScoreText;

    [Header("Score Thresholds")]
    public int oneStarScore = 500;
    public int twoStarScore = 1000;
    public int threeStarScore = 1500;

    void Start()
    {
        if (endPanel != null)
            endPanel.SetActive(false);
    }

    public void ShowGameCompletePanel()
    {
        if (endPanel == null) return;

        int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.GetFinalScore() : 0;
        endPanel.SetActive(true);

        if (finalScoreText != null)
        {
            finalScoreText.text = "Score: " + finalScore;
        }

        int starsEarned = CalculateStarRating(finalScore);
        DisplayStars(starsEarned);
    }

    int CalculateStarRating(int score)
    {
        if (score >= threeStarScore)
            return 3;
        if (score >= twoStarScore)
            return 2;
        if (score >= oneStarScore)
            return 1;
        return 0;
    }

    void DisplayStars(int count)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < count)
                stars[i].sprite = filledStarSprite;
            else
                stars[i].sprite = emptyStarSprite;
        }
    }
}
