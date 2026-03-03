using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

#region Response Classes

[Serializable]
public class AIResponse
{
    public int score;
    public string comment;
}

[Serializable]
public class ChatMessage
{
    public string content;
}

[Serializable]
public class Choice
{
    public ChatMessage message;
}

[Serializable]
public class OpenAIResponse
{
    public Choice[] choices;
}

#endregion

public class AIManager : MonoBehaviour
{
    [Header("References")]
    public DrawingCanvas drawingCanvas;

    [Header("API Settings")]
    public string apiKey;
    public int clearScore = 70;

    private bool isProcessing = false;

    // 🔥 버튼에 연결할 함수
    public void Submit()
    {
        string answer = GameManager.Instance.suggestWord;

        if (isProcessing) return;

        if (drawingCanvas == null)
        {
            Debug.LogError("DrawingCanvas not assigned!");
            return;
        }

        Texture2D texture = drawingCanvas.GetTexture();
        Texture2D resized = ResizeTexture(texture, 700, 300);
        StartCoroutine(SendToAI(resized, answer));
    }

    IEnumerator SendToAI(Texture2D texture, string answer)
    {
        isProcessing = true;

        byte[] imageBytes = texture.EncodeToPNG();
        string base64Image = Convert.ToBase64String(imageBytes);

        string prompt =
$@"너는 그림 채점 AI다.

제시어: {answer}

1. 제시어와의 유사도를 0~100 사이 정수로 평가하라.
2. 50자 이내 감상을 작성하라.

절대 코드블럭(```)을 사용하지 말 것.
다른 텍스트를 출력하지 말 것.
반드시 JSON 객체 하나만 출력할 것.

형식:
{{
  ""score"": 0,
  ""comment"": """"
}}";

        string jsonBody =
$@"{{
  ""model"": ""gpt-4o-mini"",
  ""messages"": [
    {{
      ""role"": ""user"",
      ""content"": [
        {{
          ""type"": ""text"",
          ""text"": ""{EscapeJson(prompt)}""
        }},
        {{
          ""type"": ""image_url"",
          ""image_url"": {{
            ""url"": ""data:image/png;base64,{base64Image}""
          }}
        }}
      ]
    }}
  ],
  ""max_tokens"": 200
}}";

        using (UnityWebRequest request = new UnityWebRequest(
            "https://api.openai.com/v1/chat/completions",
            "POST"))
        {
            request.uploadHandler =
                new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody));
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API Error: " + request.error);
                Debug.LogError(request.downloadHandler.text);
                isProcessing = false;
                yield break;
            }

            string response = request.downloadHandler.text;
            Debug.Log("Raw Response: " + response);

            OpenAIResponse openAIRes =
                JsonUtility.FromJson<OpenAIResponse>(response);

            if (openAIRes == null || openAIRes.choices.Length == 0)
            {
                Debug.LogError("Invalid OpenAI response structure");
                isProcessing = false;
                yield break;
            }

            string content =
                openAIRes.choices[0].message.content;

            content = content.Replace("```json", "")
                             .Replace("```", "")
                             .Trim();

            AIResponse result =
                JsonUtility.FromJson<AIResponse>(content);

            if (result == null)
            {
                Debug.LogError("AIResponse parsing failed");
                isProcessing = false;
                yield break;
            }

            Debug.Log("Score: " + result.score);
            Debug.Log("Comment: " + result.comment);

            if (result.score >= clearScore)
                GameClear(result.score, result.comment);
            else
                GameFail(result.score, result.comment);

            isProcessing = false;
        }
    }

    string EscapeJson(string str)
    {
        return str.Replace("\\", "\\\\")
                  .Replace("\"", "\\\"")
                  .Replace("\n", "\\n")
                  .Replace("\r", "");
    }

    Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        RenderTexture rt = RenderTexture.GetTemporary(targetWidth, targetHeight);
        Graphics.Blit(source, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
        result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
        result.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        return result;
    }

    void GameClear(int score, string comment)
    {
        Debug.Log("CLEAR! Score: " + score);
        Debug.Log("Comment: " + comment);
    }

    void GameFail(int score, string comment)
    {
        Debug.Log("FAIL! Score: " + score);
        Debug.Log("Comment: " + comment);
    }
}