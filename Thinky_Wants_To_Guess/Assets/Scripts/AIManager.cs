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
$@"너는 그림을 보고 무엇인지 추측하는 AI다.

참고 단어: {answer}

규칙:

1. 그림의 특징을 먼저 설명하라.
2. 그 다음 무엇처럼 보이는지 추측하라.

comment는 반드시 아래 구조를 정확히 따라야 한다.

[특징 설명]. 그래서 [사물 이름]처럼 보인다.

예:
둥근 모양에 위쪽에 선이 있어서 사과처럼 보인다.

3. 특징 설명에는 최소 두 가지 특징을 포함하라.
4. comment는 반드시 20자 이상 작성하라.

5. score가 70 이상이면 반드시 참고 단어를 사용하라.
6. score가 70 미만이면 참고 단어를 절대 사용하지 마라.

7. 반드시 마지막에 사물 이름 하나를 말해야 한다.
8. 설명만 하고 끝내면 안 된다.
9. 여러 후보를 말하지 말고 하나만 말하라.
10. comment의 마지막 문장은 반드시 ""[사물 이름]처럼 보인다"" 형태여야 한다.
11. comment는 반드시 한 문장만 작성하라.

출력 규칙:

JSON 하나만 출력
다른 텍스트 금지

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

            MainGameManager.Instance.ResponseExplanation(result.comment);
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
        MainGameManager.Instance.isClear = true;
        Debug.Log("CLEAR! Score: " + score);
        Debug.Log("Comment: " + comment);
    }

    void GameFail(int score, string comment)
    {
        MainGameManager.Instance.isClear = false;
        Debug.Log("FAIL! Score: " + score);
        Debug.Log("Comment: " + comment);
    }
}