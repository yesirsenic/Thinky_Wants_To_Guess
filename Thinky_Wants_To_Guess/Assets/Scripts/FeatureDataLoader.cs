using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class FeatureData
{
    public string word;
    public string feature;
    public int weight;
}

public class FeatureDataLoader : MonoBehaviour
{
    public Dictionary<string, List<FeatureData>> wordDictionary
        = new Dictionary<string, List<FeatureData>>();

    public List<FeatureData> features = new List<FeatureData>();

    void Awake()
    {
        LoadCSV();
    }

    private void Start()
    {
        features = GetFeaturesByWord(GameManager.Instance.suggestWord);
    }

    void LoadCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("suggest_word_data");
        string[] lines = csvFile.text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string line = lines[i].Trim();

            // ½°Ç¥ ¶Ç´Â ÅÇ µÑ ´Ù ´ëÀÀ
            string[] values = line.Split('\t');

            if (values.Length < 5)
                values = line.Split(',');

            if (values.Length < 5)
            {
                Debug.LogWarning("CSV ÆÄ½Ì ½ÇÆÐ: " + line);
                continue;
            }

            string word = values[2];
            string feature = values[3];
            int weight = int.Parse(values[4]);

            FeatureData data = new FeatureData
            {
                word = word,
                feature = feature,
                weight = weight
            };

            if (!wordDictionary.ContainsKey(word))
                wordDictionary[word] = new List<FeatureData>();

            wordDictionary[word].Add(data);
        }
    }

    public List<FeatureData> GetFeaturesByWord(string word)
    {
        if (wordDictionary.ContainsKey(word))
            return wordDictionary[word];

        return null;
    }
}
