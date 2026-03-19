using System.Collections.Generic;
using UnityEngine;

public class StageLevelDataLoader : MonoBehaviour
{
    public List<StageLevelData> stageDataList = new List<StageLevelData>();

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
        GameManager.Instance.stageLevelDataList.Clear();

        TextAsset csvFile = Resources.Load<TextAsset>("stage_level_data");
        string[] lines = csvFile.text.Split('\n');

        int startIndex = 1 + 15 * (GameManager.Instance.stageNum - 1);
        int endIndex = startIndex + 15;

        for (int i = startIndex; i < endIndex && i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] values = lines[i].Split(',');

            StageLevelData data = new StageLevelData();
            data.Stage_Num = int.Parse(values[0]);
            data.Level_Num = int.Parse(values[1]);
            data.Suggest_Word = values[2];
            data.Key_Suggest = values[3];

            stageDataList.Add(data);
        }

        GameManager.Instance.stageLevelDataList.AddRange(stageDataList);
    }

    List<StageLevelData> GetStageLevels(int stageNum)
    {
        List<StageLevelData> result = new List<StageLevelData>();

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
        List<StageLevelData> levels = GetStageLevels(stageNum);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            levelButtons[i].Setup(levels[i]);
        }
    }
}