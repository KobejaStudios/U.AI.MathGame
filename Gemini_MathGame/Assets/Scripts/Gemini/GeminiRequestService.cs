using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public interface IGeminiRequestService
{
    UniTask<string> AsyncGeminiRequest(string prompt);
}

public class GeminiRequestService : IGeminiRequestService
{
    private readonly string _endpoint = 
        $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent?key={Secret.GEMINI_KEY}";

    public async UniTask<string> AsyncGeminiRequest(string prompt)
    {
        return await UniTask.RunOnThreadPool(async () =>
        {
            Debug.Log("Sending Gemini Request");
            try
            {
                var jsonInput = $"{{\"contents\":[{{\"parts\":[{{\"text\":\"{prompt}\"}}]}}]}}";
                using var client = new HttpClient();
                var content = new StringContent(jsonInput, Encoding.UTF8, "application/json");

                using var request = await client.PostAsync(_endpoint, content);
                var rawResponse = await request.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<GeminiResponse>(rawResponse);
                var result = responseData.Candidates[0].Content.Parts[0].Text;
                Debug.Log($"Received gemini request.\nRaw data: {rawResponse}\nReturning result: {result}");
            
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e.Message}");
                throw;
            }
        });
    }
}

public class SafetyRating
{
    public string Category { get; set; }
    public string Probability { get; set; }
}

public class ContentPart
{
    public string Text { get; set; }
}

public class Content
{
    public List<ContentPart> Parts { get; set; }
    public string Role { get; set; }
}

public class Candidate
{
    public Content Content { get; set; }
    public string FinishReason { get; set; }
    public int Index { get; set; }
    public List<SafetyRating> SafetyRatings { get; set; }
}

public class UsageMetadata
{
    public int PromptTokenCount { get; set; }
    public int CandidatesTokenCount { get; set; }
    public int TotalTokenCount { get; set; }
}

public class GeminiResponse
{
    public List<Candidate> Candidates { get; set; }
    public UsageMetadata UsageMetadata { get; set; }
}
