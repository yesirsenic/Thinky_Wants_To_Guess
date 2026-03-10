using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [System.Serializable]
    public class SceneBGM
    {
        public string sceneName;
        public AudioClip bgm;
    }

    [Header("BGM List")]
    public List<SceneBGM> sceneBGMs = new List<SceneBGM>();

    private Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();

    private AudioSource audioSource;
    private AudioClip currentClip;

    void Awake()
    {
        // Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        foreach (var item in sceneBGMs)
        {
            if (!bgmDict.ContainsKey(item.sceneName))
                bgmDict.Add(item.sceneName, item.bgm);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGM(scene.name);
    }


    void Start()
    {
        if (VolumeManager.Instance != null)
            SetVolume(VolumeManager.Instance.BGMVolume);
    }

    void PlayBGM(string sceneName)
    {
        if (!bgmDict.ContainsKey(sceneName))
            return;

        AudioClip newClip = bgmDict[sceneName];

        if (currentClip == newClip)
            return;

        audioSource.clip = newClip;
        audioSource.loop = true;
        audioSource.Play();

        currentClip = newClip;
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }
}