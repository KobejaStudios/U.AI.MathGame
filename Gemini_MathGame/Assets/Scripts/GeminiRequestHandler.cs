using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

public class GeminiRequestHandler : MonoBehaviour
{
    private readonly string _endpoint = 
        $"https://generativelanguage.googleapis.com/v1/models/gemini-pro:generateContent?key={Secret.GEMINI_KEY}";

    private async Task<string> AsyncGeminiRequest(string prompt)
    {
        try
        {
            var jsonInput = $"{{\"contents\":[{{\"parts\":[{{\"text\":\"{prompt}\"}}]}}]}}";
            using var client = new HttpClient();
            var content = new StringContent(jsonInput, Encoding.UTF8, "application/json");

            using var request = await client.PostAsync(_endpoint, content);
            var rawResponse = await request.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<GeminiResponse>(rawResponse);
        
            return responseData.Candidates[0].Content.Parts[0].Text;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e.Message}");
            throw;
        }
    }

    public async void SendRequest()
    {
        var prompt = "what is 255 + 552";
        Debug.Log($"Request Started. Prompt: {prompt}");
        var answer = await AsyncGeminiRequest(prompt);
        Debug.Log($"Request Finished. Answer: {answer}");
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
