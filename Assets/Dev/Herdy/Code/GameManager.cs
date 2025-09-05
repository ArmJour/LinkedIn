using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public float targetScoreMultiplier = 0.2f;
    public float timeMultiplier = 0.1f;
    public int targetScore;
    public List<ApplicantData> applicants;
    public PlayerStats playerStats;
    public Timer gameTimer;
    public RopeGameManager ropeGameManager;
    public Animator applicantAnimation;
    public VideoPlayer videoPlayer;
    public GameObject gameOverScreen;
    public GameObject gameWinScreen;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentStageText;
    private bool isGameFinished = false;


    void Start()
    {
        int applicant = Random.Range(0, applicants.Count);
        PlayVideo(applicant);
        applicantAnimation.runtimeAnimatorController = applicants[applicant].applicantAnimation;
        targetScore = (int)(applicants[applicant].scoreTarget * (1 + (playerStats.currentStage - 1) * targetScoreMultiplier));
        gameTimer.remainingTime = applicants[applicant].timeLimit * (1 + (playerStats.currentStage - 1) * timeMultiplier);
        targetScoreText.text = "Target: " + targetScore.ToString();
        currentScoreText.text = "Score: " + playerStats.currentScore.ToString();
        currentStageText.text = "Stage: " + playerStats.currentStage.ToString();
        gameOverScreen.SetActive(false);
        gameWinScreen.SetActive(false);
    }

    void PlayVideo(int applicant)
    {
        videoPlayer.gameObject.SetActive(true);        
        gameTimer.Pause();
        videoPlayer.clip = applicants[applicant].applicantVideo;
        videoPlayer.Play(); 
        videoPlayer.loopPointReached += OnVideoEnd;
    }
    void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayer.gameObject.SetActive(false);
        gameTimer.Resume();
    }

    void Update()
    {
        currentScoreText.text = "Score: " + playerStats.currentScore.ToString();
        currentStageText.text = "Stage: " + playerStats.currentStage.ToString();
        CheckGameStatus();
    }

    public void CheckGameStatus()
    {
        if (isGameFinished) return;

        if (gameTimer.remainingTime <= 0)
        {
            isGameFinished = true;
            GameOver();
        }
        else if (playerStats.currentScore >= targetScore)
        {
            isGameFinished = true;
            GameWin();
        }
    }

    public void GameOver()
    {
        gameTimer.Pause();
        gameOverScreen.SetActive(true);
        SoundManager.PlaySound(SoundType.Level_Lose);

        TextMeshProUGUI[] textComponents = gameOverScreen.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var textComponent in textComponents)
        {
            if (textComponent.text == "Current Stage:")
            {
                textComponent.text += " " + playerStats.currentStage.ToString();
            }
            else if (textComponent.text == "Highest Stage:")
            {
                textComponent.text += " " + playerStats.stageAchieved.ToString();
            }
        }

        Debug.Log("Game Over");
    }

    public void GameWin()
    {
        gameTimer.Pause();
        gameWinScreen.SetActive(true);
        SoundManager.PlaySound(SoundType.Level_Win);

        TextMeshProUGUI[] textComponents = gameWinScreen.GetComponentsInChildren<TextMeshProUGUI>();

        foreach (var textComponent in textComponents)
        {
            if (textComponent.text == "Current Stage:")
            {
                textComponent.text += " " + playerStats.currentStage.ToString();
            }
            else if (textComponent.text == "Highest Stage:")
            {
                textComponent.text += " " + playerStats.stageAchieved.ToString();
            }
        }
        Debug.Log("You Win!");
    }
    public void AdvanceStage()
    {
        isGameFinished = false;
        playerStats.AdvanceStage();
        playerStats.currentScore = 0;
        int applicant = Random.Range(0, applicants.Count);
        PlayVideo(applicant);
        gameTimer.remainingTime = applicants[applicant].timeLimit * (1 + (playerStats.currentStage - 1) * timeMultiplier);
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
        isGameFinished = false;
        playerStats.ResetStats();
        playerStats.currentScore = 0;
        int applicant = Random.Range(0, applicants.Count);
        PlayVideo(applicant);
        applicantAnimation.runtimeAnimatorController = applicants[applicant].applicantAnimation;
        gameTimer.remainingTime = applicants[applicant].timeLimit * (1 + (playerStats.currentStage - 1) * timeMultiplier);
        targetScore = (int)(applicants[applicant].scoreTarget * (1 + (playerStats.currentStage - 1) * targetScoreMultiplier));
        targetScoreText.text = "Target: " + targetScore.ToString();
        currentScoreText.text = "Score: " + playerStats.currentScore.ToString();
        currentStageText.text = "Stage: " + playerStats.currentStage.ToString();
        gameOverScreen.SetActive(false);
        gameTimer.Resume();
        ropeGameManager.ResetGamePieces();
    }
}
