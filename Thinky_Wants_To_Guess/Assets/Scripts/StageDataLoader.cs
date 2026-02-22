using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataLoader : MonoBehaviour
{
    public List<StageData> stageDataList = new List<StageData>();

    public StageButton[] stageButtons;

    void Awake()
    {
        LoadCSV();
    }

    private void Start()
    {
        SetupStage();
    }

    void LoadCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("stage_data");
        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 0은 헤더니까 제외
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            StageData data = new StageData();
            data.Stage_Num = int.Parse(values[0]);
            data.IsClear = bool.Parse(values[1]);

            stageDataList.Add(data);
        }
    }

    void SetupStage()
    {
        

        for (int i = 0; i < stageButtons.Length; i++)
        {
            stageButtons[i].Setup(stageDataList[i]);
        }
    }
}
