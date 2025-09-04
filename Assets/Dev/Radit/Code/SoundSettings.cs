using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private AudioMixer masterMixer;

    private void Start()
    {
        // Load saved volume or set to default
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        masterVolumeSlider.value = savedVolume;
        SetMasterVolume(savedVolume);
        // Add listener to slider
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    public void SetMasterVolume(float _value)
    {
        if(_value < 1)
        {
            _value = .001f;
        }

        RefreshSlider(_value);
        PlayerPrefs.SetFloat("SavedMasterVolume", _value);
        masterMixer.SetFloat("MasterVolume", Mathf.Log10(_value  /100) * 20);

    }

    public void SetVolumeFromSlider()
    {
        SetMasterVolume(masterVolumeSlider.value);
    }


    public void RefreshSlider(float _value)
    {
        masterVolumeSlider.value = _value;
    }
}
