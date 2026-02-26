using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    [Header("References")]
    public FeatureDataLoader dataLoader;
    public DrawingCanvas drawingCanvas;

    [Header("UI")]
    public Button submitButton;

    [Header("Settings")]
    public int resizeSize = 256;
    public int featureThreshold = 60;   // ⭐ 60 이상이면 true
    [Range(0f, 1f)]
    public float passThreshold = 0.6f;  // 전체 통과 기준

    [Header("OpenAI")]
    public string apiKey;
    private string apiUrl = "https://api.openai.com/v1/responses";

    // =========================================================
    // Submit Entry
    // =========================================================
    public void Submit()
    {
        string currentWord = GameManager.Instance.suggestWord;

        if (!submitButton.interactable)
            return;

        submitButton.interactable = false; // 🔒 버튼 잠금

        List<FeatureData> features =
            dataLoader.GetFeaturesByWord(currentWord);

        if (features == null || features.Count == 0)
        {
            Debug.LogError("Feature 없음");
            submitButton.interactable = true; // 실패 시 복구
            return;
        }

        Texture2D raw = drawingCanvas.GetTexture();
        Texture2D resized = ResizeTexture(raw, resizeSize);

        string prompt = BuildFeaturePrompt(currentWord, features);
        string base64 = ConvertToBase64(resized);

        StartCoroutine(SendToVisionAPI(prompt, base64, features));
    }

    // =========================================================
    // 점수 기반 프롬프트
    // =========================================================
    string BuildFeaturePrompt(string word, List<FeatureData> features)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("You are analyzing a simple drawing.");
        sb.AppendLine("The drawing is rough and cartoon-like.");
        sb.AppendLine("For each feature, give a confidence score from 0 to 100.");
        sb.AppendLine("0 = not visible at all");
        sb.AppendLine("100 = clearly visible");
        sb.AppendLine("Do not be overly strict.");
        sb.AppendLine();
        sb.AppendLine("Target word: " + word);
        sb.AppendLine();
        sb.AppendLine("Features:");

        for (int i = 0; i < features.Count; i++)
        {
            sb.AppendLine($"{i + 1}. {features[i].feature}");
        }

        sb.AppendLine();
        sb.AppendLine("Return ONLY JSON like this:");
        sb.AppendLine("{");

        for (int i = 0; i < features.Count; i++)
        {
            sb.AppendLine($"  \"{i + 1}\": number{(i < features.Count - 1 ? "," : "")}");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    // =========================================================
    // API 호출
    // =========================================================
    IEnumerator SendToVisionAPI(string prompt, string base64Image, List<FeatureData> features)
    {
        string jsonBody =
        $@"
    {{
        ""model"": ""gpt-4o-mini"",
        ""input"": [
            {{
                ""role"": ""user"",
                ""content"": [
                    {{
                        ""type"": ""input_text"",
                        ""text"": ""{EscapeJson(prompt)}""
                    }},
                    {{
                        ""type"": ""input_image"",
                        ""image_url"": ""data:image/jpeg;base64,{base64Image}""
                    }}
                ]
            }}
        ]
    }}";

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("API Error: " + request.error);
            Debug.LogError(request.downloadHandler.text);
        }
        else
        {
            string responseText = request.downloadHandler.text;

            Debug.Log("=== API 응답 원본 ===");
            Debug.Log(responseText);

            string extractedJson = ExtractAssistantJson(responseText);

            if (extractedJson == null)
            {
                Debug.LogError("JSON 추출 실패");
                yield break;
            }

            Debug.Log("=== 추출된 순수 JSON ===");
            Debug.Log(extractedJson);

            Dictionary<string, int> scoreDict =
                ParseScoreResult(extractedJson);

            int totalScore = 0;
            int maxScore = features.Sum(f => f.weight);

            Debug.Log("===== Feature 판정 결과 =====");

            for (int i = 0; i < features.Count; i++)
            {
                string key = (i + 1).ToString();

                int confidence = 0;
                bool isTrue = false;

                if (scoreDict.ContainsKey(key))
                {
                    confidence = scoreDict[key];
                    isTrue = confidence >= featureThreshold;
                }

                if (isTrue)
                    totalScore += features[i].weight;

                Debug.Log(
                    $"Feature {key}: {features[i].feature}\n" +
                    $"   → Confidence: {confidence}\n" +
                    $"   → Result: {(isTrue ? "TRUE" : "FALSE")}"
                );
            }

            Debug.Log($"===== 최종 점수: {totalScore} / {maxScore} =====");

            if (totalScore >= maxScore * passThreshold)
                Debug.Log("🎯 통과!");
            else
                Debug.Log("❌ 실패");
        }
    }

    // =========================================================
    // JSON 추출
    // =========================================================
    string ExtractAssistantJson(string fullResponse)
    {
        try
        {
            var wrapper = JsonUtility.FromJson<ResponseWrapper>(fullResponse);
            string text = wrapper.output[0].content[0].text;

            text = text.Replace("```json", "")
                       .Replace("```", "")
                       .Trim();

            return text;
        }
        catch
        {
            return null;
        }
    }

    // =========================================================
    // 점수 JSON 파싱
    // =========================================================
    Dictionary<string, int> ParseScoreResult(string json)
    {
        Dictionary<string, int> result = new Dictionary<string, int>();

        json = json.Replace("{", "")
                   .Replace("}", "")
                   .Replace("\"", "");

        string[] pairs = json.Split(',');

        foreach (string pair in pairs)
        {
            string[] keyValue = pair.Split(':');

            if (keyValue.Length == 2)
            {
                string key = keyValue[0].Trim();
                int value;

                if (int.TryParse(keyValue[1].Trim(), out value))
                {
                    result[key] = value;
                }
            }
        }

        return result;
    }

    // =========================================================
    // Resize
    // =========================================================
    Texture2D ResizeTexture(Texture2D source, int size)
    {
        RenderTexture rt = RenderTexture.GetTemporary(size, size);
        Graphics.Blit(source, rt);

        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(size, size);
        result.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        result.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }

    // =========================================================
    // Base64
    // =========================================================
    string ConvertToBase64(Texture2D tex)
    {
        byte[] jpg = tex.EncodeToJPG(70);
        return System.Convert.ToBase64String(jpg);
    }

    // =========================================================
    // Escape
    // =========================================================
    string EscapeJson(string str)
    {
        return str.Replace("\\", "\\\\")
                  .Replace("\"", "\\\"")
                  .Replace("\n", "\\n")
                  .Replace("\r", "");
    }
}

// ==========================
// 응답 구조용 클래스
// ==========================
[System.Serializable]
public class ResponseWrapper
{
    public OutputItem[] output;
}

[System.Serializable]
public class OutputItem
{
    public ContentItem[] content;
}

[System.Serializable]
public class ContentItem
{
    public string text;
}