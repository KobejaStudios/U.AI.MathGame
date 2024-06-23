using System;
using System.Collections.Generic;
using System.Reflection;

public static class DataExtensions
{
    public static bool TryGetAs<TValue, TKey>(this IDictionary<TKey, object> dictionary, TKey key, out TValue value)
    {
        value = default(TValue);
        if (!dictionary.TryGetValue(key, out object ambiguous)) return false;
			
        if (ambiguous is TValue asTValue)
        {
            value = asTValue;
            return true;
        }

        try
        {
            if (typeof(TValue).GetTypeInfo().IsEnum && ambiguous is string asString)
            {
                value = (TValue)Enum.Parse(typeof(TValue), asString);
            }
            else
            {
                value = (TValue)Convert.ChangeType(ambiguous, typeof(TValue));
            }
            return true;
        }
        catch
        {
            throw new Exception($"TryGetAs key [{key}] expected [{typeof(TValue).ToString()}] got a [{ambiguous}]");
        }
    }
}
