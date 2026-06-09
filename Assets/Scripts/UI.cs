using UnityEngine;
using TMPro;

public class UI : MonoBehaviour
{
    public Game game;

    public GameObject menuPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;

    public TextMeshProUGUI berryText;
    public TextMeshProUGUI resultText;

    private void Start() 
    {
        ShowMenu();
    }

    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    public void ShowHUD()
    {
        menuPanel.SetActive(false);
        hudPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        menuPanel.SetActive(false);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
    
    public void EasyMode()
    {
        game.SetDifficulty(8, 8, 10, 3);
        StartGame();
    }

    public void MediumMode()
    {
        game.SetDifficulty(16, 16, 32, 2);
        StartGame();
    }

    public void HardMode()
    {
        game.SetDifficulty(24, 24, 60, 1);
        StartGame();
    }

    public void Tutorial()
    {
        // show introduction story
    }

    private void StartGame()
    {
        ShowHUD();
        game.NewGame();
    }

    public void UpdateBerries(int current, int max)
    {
        berryText.text = $"Berries: {current} / {max}";
    }

    public void ShowWin()
    {
        ShowGameOver();
        resultText.text = "You win!";
    }

    public void ShowLose() 
    {
        ShowGameOver();
        resultText.text = "You lose!";
    }

    public void Restart()
    {
        game.NewGame();
        ShowHUD();
    }

    public void Return()
    {
        ShowMenu();
    }
}
