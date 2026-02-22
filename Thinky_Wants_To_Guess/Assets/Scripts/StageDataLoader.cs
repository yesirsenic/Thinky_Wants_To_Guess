using System.Collections.Generic;
using UnityEngine;

public class StageDataLoader : MonoBehaviour
{
    public List<StageData> stageDataList = new List<StageData>();

    public LevelButton[] levelButtons;

    void Awake()
    {
        LoadCSV();
    }

    private void Start()
    {
        SetupStage(GameManager.Instance.stageNum);
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
            data.Level_Num = int.Parse(values[1]);
            data.Suggest_Word = values[2];
            data.IsClear = bool.Parse(values[3]);

            stageDataList.Add(data);
        }
    }

    List<StageData> GetStageLevels(int stageNum)
    {
        List<StageData> result = new List<StageData>();

        foreach (var data in stageDataList)
        {
            if (data.Stage_Num == stageNum)
            {
                result.Add(data);
            }
        }

        return result;
    }

     void SetupStage(int stageNum)
    {
        List<StageData> levels = GetStageLevels(stageNum);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].Setup(levels[i]);
        }
    }
}