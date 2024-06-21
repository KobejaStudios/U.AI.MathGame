using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

#if UNITASK_AVAILABLE
using Cysharp.Threading.Tasks;
using Task = Cysharp.Threading.Tasks.UniTask;
#pragma warning disable CS0618 // Type or member is obsolete
#else
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
#endif

// ReSharper disable once CheckNamespace
public static class JsonExtensions
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        Converters = new JsonConverter[] { new StringEnumConverter() },
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
    };

    public static string ToJson(this object o)
    {
        return JsonConvert.SerializeObject(o, JsonSerializerSettings);
    }

    public static string ToJsonPretty(this object o)
    {
        return JsonConvert.SerializeObject(o, Formatting.Indented, JsonSerializerSettings);
    }

    public static T FromJson<T>(this string s)
    {
        return JsonConvert.DeserializeObject<T>(s, JsonSerializerSettings);
    }
    
    public static async
#if UNITASK_AVAILABLE
            UniTask<string>
#else
        Task<string>
#endif
        ToJsonAsync(this object o)
    {
        return await Task.Run(() => ToJson(o));
    }

    public static async
#if UNITASK_AVAILABLE
            UniTask<T>
#else
        Task<T>
#endif
        FromJsonAsync<T>(this string s)
    {
        return await Task.Run(() => FromJson<T>(s));
    }
}