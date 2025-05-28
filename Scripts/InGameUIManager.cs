using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject exitPanel; // Assign in Inspector
    public GameObject levelCompletePanel; // Assign in Inspector

    private bool isPaused = false;

    private void Start()
    {
        if (exitPanel != null) exitPanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleExitPanel();
        }
    }

    public void OnExitButtonPressed()
    {
        ToggleExitPanel();
    }

    private void ToggleExitPanel()
    {
        if (exitPanel == null) return;

        isPaused = !isPaused;
        exitPanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void OnResumeButton()
    {
        ToggleExitPanel();
    }

    public void OnRestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnExitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // This can be called from LevelManager
    public void ShowLevelCompletePanel()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    // Optional button for next level (hook from panel)
    public void OnNextLevel(string nextLevelName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextLevelName);
    }
}
