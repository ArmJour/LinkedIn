using UnityEngine;
using UnityEngine.Audio;
public enum SoundType
{
    Background_Music,
    Pop_Up_Noise,
    Level_Win
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    public AudioMixerGroup audioMixer;
    public static SoundManager instance;
    public AudioSource audioSource;
    public AudioSource loopingSource;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        loopingSource = gameObject.AddComponent<AudioSource>();
        loopingSource.loop = true; 
    }

    public static void PlaySound(SoundType soundType) 
    {
        instance.audioSource.PlayOneShot(instance.soundList[(int)soundType]);
    }

    public static void PlayLoopingSound(SoundType soundType)
    {
        instance.loopingSource.clip = instance.soundList[(int)soundType];
        instance.loopingSource.Play();
    }

    public static void StopLoopingSound()
    {
        instance.loopingSource.Stop();
    }

    public static void PlayPopUpSound()
    {
        PlaySound(SoundType.Pop_Up_Noise);
    }
}