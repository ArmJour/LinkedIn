using UnityEngine;
using TMPro;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public float initialTime = 60f;
    public float targetScoreMultiplier = 0.2f;
    public float timeMultiplier = 0.1f;
    public List<ApplicantData> applicants;
    public PlayerStats playerStats;
    public Timer gameTimer;
    public GameObject gameOverScreen;
    public GameObject gameWinScreen;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentStageText;

    void Start()
    {
        gameTimer.Resume();
        targetScoreText.text = "Target: " + applicants[Random.Range(0, applicants.Length)].scoreTarget * targetScoreMultiplier.ToString();
        currentScoreText.text = "Score: " + playerStats.currentScore.ToString();
        currentStageText.text = "Stage: " + playerStats.currentStage.ToString();
        gameOverScreen.SetActive(false);
        gameWinScreen.SetActive(false);
        gameTimer.remainingTime = initialTime * playerStats.currentStage;

    }

    void Update()
    {
        currentScoreText.text = "Score: " + playerStats.currentScore.ToString();
        currentStageText.text = "Stage: " + playerStats.currentStage.ToString();
        CheckGameStatus();
    }

    public void CheckGameStatus()
    {
        if (gameTimer.remainingTime <= 0)
        {
            GameOver();

        }
        else if (playerStats.currentScore >= (applicants[playerStats.currentStage - 1].scoreTarget * targetScoreMultiplier))
        {
            AdvanceStage();
            gameWinScreen.SetActive(true);
        }
    }

    public void GameOver()
    {
        gameTimer.Pause();
        gameOverScreen.SetActive(true);

    }
    
    public void AdvanceStage()
    {
        playerStats.AdvanceStage();
        gameTimer.remainingTime = initialTime * (1 + (playerStats.currentStage - 1) * timeMultiplier);
        playerStats.currentScore = 0;
        targetScoreText.text = "Target: " + (applicants[playerStats.currentStage - 1].scoreTarget * targetScoreMultiplier).ToString();
        currentScoreText.text = "Score: " + playerStats.currentScore.ToString();
        currentStageText.text = "Stage: " + playerStats.currentStage.ToString();
    }
    
}
