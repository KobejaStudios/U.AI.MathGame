using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PromptBuilderService
{
    public static string BuildPromptSimple(int target, int pairs = 20)
    {
        // var value1 = $"Generate a JSON object named '{target}' with a value that's an array of {pairs * 2} numbers between 0 and {target}. I want to be able to  {PercentBuilder(pairs)} distinct number pairs within the array sum up to {target} and the remaining numbers are random within the range of 0 - {target}" +
        //            $" I only want the JSON object";
        // var value2 = $"Generate a JSON object named '{target}' with a value that's an array of {pairs * 2} numbers between 0 and {target}. Within this array of numbers I want {PercentBuilder(pairs)} distinct number pairs that sum up to {target} and the remaining numbers are random within the range of 0 - {target}" +
        //            $" I only want the JSON object";
        
        var value3 = $"Generate a JSON object named '{target}' with a value that's an array of {pairs} random integers between 0 and {target}. Then merge {pairs / 2} random number pairs that sum up to {target}. " +
                     $"I only want you to return the one JSON object which will have an array of {pairs * 2} numbers meeting the conditions that I have defined";
        Debug.Log($"Prompt: {value3}");
        return value3;

    }
    
    public static string BuildPromptComplex(int target, int pairs = 21)
    {
        return
            $"Generate a JSON object named '{target}' with a value that's an array of {pairs * 2} random numbers between 0 and {target}.  Include some pairs within the array that sum up to {target}. The number of pairs should be between {BuildCorrectPairsTarget(pairs)} and {BuildCorrectPairsTarget(pairs)}." +
            $"\n Additionally, generate a secondary JSON object named \"config\" with the following structure:\n\n```json\n{{\n  \"config\": {{\n    \"numValues\": 36,\n    \"numPairs\": 12, // This will be the number between {BuildCorrectPairsTarget(pairs)} and {BuildCorrectPairsTarget(pairs)} that you used for determining the number of correct pairs\n    \"correctPairs\": [\n      // List of pairs that sum up to 117\n    ]\n  }}\n}}";
    }

    private static int BuildCorrectPairsTarget(int pairs)
    {
        var input = UnityEngine.Random.Range(0.5f, 0.8f);
        var rand = UnityEngine.Random.Range(input, Math.Min(input * 1.5f, 0.9f));
        var value = Mathf.FloorToInt(rand * pairs);
        Debug.Log($"correct pairs target: {value}, total pairs: {pairs}");
        return value;
    }
}
