using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public float initialTime = 60f;
    public float targetScoreMultiplier = 0.2f;
    public float timeMultiplier = 0.1f;
    public int targetScore;
    public List<ApplicantData> applicants;
    public PlayerStats playerStats;
    public Timer gameTimer;
    public RopeGameManager ropeGameManager;
    public Animator applicantAnimation;
    public GameObject gameOverScreen;
    public GameObject gameWinScreen;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentStageText;

    void Start()
    {
        gameTimer.Resume();
        int applicant = Random.Range(0, applicants.Count);
        applicantAnimation.runtimeAnimatorController = applicants[applicant].applicantAnimation;
        targetScore = (int)(applicants[applicant].scoreTarget * (1 + (playerStats.currentStage - 1) * targetScoreMultiplier));
        gameTimer.remainingTime = initialTime * (1 + (playerStats.currentStage - 1) * timeMultiplier);
        targetScoreText.text = "Target: " + targetScore.ToString();
        currentScoreText.text = "Score: " + playerStats.currentScore.ToString();
        currentStageText.text = "Stage: " + playerStats.currentStage.ToString();
        gameOverScreen.SetActive(false);
        gameWinScreen.SetActive(false);
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
            gameOverScreen.SetActive(true);
        }
        else if (playerStats.currentScore >= targetScore)
        {
            GameWin();
            gameWinScreen.SetActive(true);
        }
    }

    public void GameOver()
    {
        gameTimer.Pause();
        gameOverScreen.SetActive(true);
        Debug.Log("Game Over");

    }

    public void GameWin()
    {
        gameTimer.Pause();
        gameWinScreen.SetActive(true);
        Debug.Log("You Win!");
    }
    public void AdvanceStage()
    {
        playerStats.AdvanceStage();
        gameTimer.remainingTime = initialTime * (1 + (playerStats.currentStage - 1) * timeMultiplier);
        playerStats.currentScore = 0;
        int applicant = Random.Range(0, applicants.Count);
        applicantAnimation.runtimeAnimatorController = applicants[applicant].applicantAnimation;
        targetScore = (int)(applicants[applicant].scoreTarget * (1 + (playerStats.currentStage - 1) * targetScoreMultiplier));
        targetScoreText.text = "Target: " + targetScore.ToString();
        currentScoreText.text = "Score: " + playerStats.currentScore.ToString();
        currentStageText.text = "Stage: " + playerStats.currentStage.ToString();
        gameWinScreen.SetActive(false);
        gameTimer.Resume();
        ropeGameManager.ResetGamePieces();
    }

    public void ResetStats()
    {
        playerStats.ResetStats();
        gameTimer.remainingTime = initialTime * (1 + (playerStats.currentStage - 1) * timeMultiplier);
        playerStats.currentScore = 0;
        int applicant = Random.Range(0, applicants.Count);
        applicantAnimation.runtimeAnimatorController = applicants[applicant].applicantAnimation;
        targetScore = (int)(applicants[applicant].scoreTarget * (1 + (playerStats.currentStage - 1) * targetScoreMultiplier));
        targetScoreText.text = "Target: " + targetScore.ToString();
        currentScoreText.text = "Score: " + playerStats.currentScore.ToString();
        currentStageText.text = "Stage: " + playerStats.currentStage.ToString();
        gameOverScreen.SetActive(false);
        gameTimer.Resume();
        ropeGameManager.ResetGamePieces();
    }
}
