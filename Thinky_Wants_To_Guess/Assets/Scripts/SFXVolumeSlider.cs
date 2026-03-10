using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeSlider : MonoBehaviour
{
    Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        if (VolumeManager.Instance != null)
            slider.value = VolumeManager.Instance.SFXVolume;

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        if (VolumeManager.Instance != null)
            VolumeManager.Instance.SetSFXVolume(value);
    }
}