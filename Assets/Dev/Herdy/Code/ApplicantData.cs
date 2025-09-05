using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine.Video;
[CreateAssetMenu(fileName = "ApplicantData", menuName = "Scriptable Objects/Applicant Data")]
public class ApplicantData : ScriptableObject
{
    public string ApplicantType;
    public RuntimeAnimatorController applicantAnimation;
    public VideoClip applicantVideo;
    public int scoreTarget;
    public float timeLimit;
}
