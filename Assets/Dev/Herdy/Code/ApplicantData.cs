using UnityEngine;

[CreateAssetMenu(fileName = "ApplicantData", menuName = "Scriptable Objects/Applicant Data")]
public class ApplicantData : ScriptableObject
{
    public string ApplicantType;
    public Sprite ApplicantSprite;
    public int scoreTarget;
}
