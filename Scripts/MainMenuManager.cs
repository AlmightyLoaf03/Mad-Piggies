using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;       // Assign your main menu panel here
    public GameObject levelSelectPanel;    // Assign your level select panel here

    // Call this on Start button click
    public void OnStartButton()
    {
        // Load level 1 scene - replace "Angry Bird" with your actual scene name
        SceneManager.LoadScene("Level1");
    }

    // Call this on Level Select button click
    public void OnLevelSelectButton()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    // Call this on Exit button click
    public void OnExitButton()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    // Call this on back button click inside Level Select panel
    public void OnBackFromLevelSelect()
    {
        levelSelectPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    // Call this to load selected level by number (1, 2, or 3)
    public void OnLevelButton(int levelNumber)
    {
        // Example: "Level1", "Level2", "Level3" are scene names for levels
        string sceneName = "Level" + levelNumber;
        SceneManager.LoadScene(sceneName);
    }
}
