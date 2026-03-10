using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance;

    const string BGM_KEY = "BGM_VOLUME";
    const string SFX_KEY = "SFX_VOLUME";

    public float BGMVolume { get; private set; } = 0.75f;
    public float SFXVolume { get; private set; } = 0.75f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolume();
    }

    void LoadVolume()
    {
        BGMVolume = PlayerPrefs.GetFloat(BGM_KEY, 0.75f);
        SFXVolume = PlayerPrefs.GetFloat(SFX_KEY, 0.75f);

        ApplyVolume();
    }

    public void SetBGMVolume(float volume)
    {
        BGMVolume = volume;

        PlayerPrefs.SetFloat(BGM_KEY, volume);
        PlayerPrefs.Save();

        ApplyVolume();
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;

        PlayerPrefs.SetFloat(SFX_KEY, volume);
        PlayerPrefs.Save();

        ApplyVolume();
    }

    void ApplyVolume()
    {
        if (BGMManager.Instance != null)
            BGMManager.Instance.SetVolume(BGMVolume);

        if (SFXManager.Instance != null)
            SFXManager.Instance.SetVolume(SFXVolume);
    }
}