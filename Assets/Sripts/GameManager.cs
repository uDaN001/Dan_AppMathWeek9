using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject gameOverScreen; 
    public Button restartButton;       
    public TextMeshProUGUI scoreUIText;           

    [Header("Game Stats")]
    public int score = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        UpdateScoreUI();
    }

    public void AddPoints(int points)
    {
        if (isGameOver) return;

        score += points;
        UpdateScoreUI();
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.enabled = false;
        }

        ItemSpawner spawner = FindFirstObjectByType<ItemSpawner>();
        if (spawner != null)
        {
            spawner.enabled = false;
        }

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateScoreUI()
    {
        if (scoreUIText != null)
        {
            scoreUIText.text = "Score: " + score.ToString();
        }
    }
}