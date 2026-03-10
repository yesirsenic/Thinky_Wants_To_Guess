using UnityEngine;
using UnityEngine.UI;

public class BGMVolumeSlider : MonoBehaviour
{
    Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();

        if (VolumeManager.Instance != null)
            slider.value = VolumeManager.Instance.BGMVolume;

        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float value)
    {
        VolumeManager.Instance.SetBGMVolume(value);
    }
}