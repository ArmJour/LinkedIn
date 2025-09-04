using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private AudioMixer masterMixer;

    private const string VolumePrefKey = "MasterVolume";

    private void Start()
    {
        if (masterVolumeSlider == null)
            Debug.LogError("SoundSettings: masterVolumeSlider is not assigned!");
        if (masterMixer == null)
            Debug.LogError("SoundSettings: masterMixer is not assigned!");

        // Ensure slider min/max are correct
        masterVolumeSlider.minValue = 0.0001f;
        masterVolumeSlider.maxValue = 1f;

        // Load saved volume or set to default
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 0.75f);
        masterVolumeSlider.value = savedVolume;
        ApplyVolume(savedVolume);

        masterVolumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        ApplyVolume(value);
        PlayerPrefs.SetFloat(VolumePrefKey, value);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(float value)
    {
        value = Mathf.Clamp(value, 0.0001f, 1f);
        float dB = Mathf.Log10(value) * 20f;
        bool result = masterMixer.SetFloat("MasterVolume", dB);
        Debug.Log($"SoundSettings: Setting volume to {value} ({dB} dB), Mixer set: {result}");
        if (!result)
            Debug.LogError("SoundSettings: Failed to set 'MasterVolume' parameter in AudioMixer. Check parameter name.");
    }
}
