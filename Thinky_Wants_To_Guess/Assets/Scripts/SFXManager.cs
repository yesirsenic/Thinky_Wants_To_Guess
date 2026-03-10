using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public enum SFXType
    {
        ButtonClick,
        Hmm,
        Aha,
        StageClear,
        StageFail
    }

    [System.Serializable]
    public class SFXData
    {
        public SFXType type;
        public AudioClip clip;
    }

    public List<SFXData> sfxList = new List<SFXData>();

    private Dictionary<SFXType, AudioClip> sfxDict = new Dictionary<SFXType, AudioClip>();

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        foreach (var sfx in sfxList)
        {
            if (!sfxDict.ContainsKey(sfx.type))
                sfxDict.Add(sfx.type, sfx.clip);
        }
    }

    public void Play(SFXType type)
    {
        if (!sfxDict.ContainsKey(type))
            return;

        audioSource.PlayOneShot(sfxDict[type]);
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }
}