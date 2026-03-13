using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    

    public static GameManager Instance;

    public int stageNum { get; set; }

    public int stageLevelNum { get; set; }

    public string suggestWord { get; set; }

    public string suggestKey { get; set; }


    public List<StageData> stageDataList { get; set; } = new List<StageData>();

    public List<StageLevelData> stageLevelDataList { get; set; } = new List<StageLevelData>();

    public int maxLevelStage = 15;

    public bool is_Demo { get; set; }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        __Init__();

        is_Demo = false;
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }

    private void __Init__()
    {

        if (PlayerPrefs.GetInt("MaxStage") == 0)
        {
            PlayerPrefs.SetInt("MaxStage", 1);
        }

        if(PlayerPrefs.GetInt("MaxLevel") == 0)
        {
            PlayerPrefs.SetInt("MaxLevel", 1);
        }


    }
}
