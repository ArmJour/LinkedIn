using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Scriptable Objects/Player Stats")]
public class PlayerStats : ScriptableObject
{
    public int currentStage;
    public int stageAchieved;
    public int currentScore;
    public int totalScore;

    public void ResetStats()
    {
        currentStage = 1;
        stageAchieved = 1;
        currentScore = 0;
        totalScore = 0;
    }

    public void AddScore(int score)
    {
        currentScore += score;
        totalScore += score;
    }   
    public void AdvanceStage()
    {
        currentStage++;
        if (currentStage > stageAchieved)
        {
            stageAchieved = currentStage;
        }
    }
}
