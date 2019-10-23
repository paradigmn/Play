using System;
using System.Collections.Generic;

public static class DictionaryUtils
{
    // Creates a dictionary of the given key-value list.
    // Thereby, the value for a key must directly follow its key in the argument list.
    // Example: DictionaryUtils.OfValues("firstKey", "firstValue", "secondKey", "secondValue");
    public static Dictionary<T, T> OfValues<T>(params T[] keyValuePairs)
    {
        if (!(keyValuePairs.Length % 2 == 0))
        {
            throw new DictionaryCreationException("Number of arguments must be even (alternating key and value).");
        }
        Dictionary<T, T> result = new Dictionary<T, T>();
        for (int i = 0; i < keyValuePairs.Length; i += 2)
        {
            result.Add(keyValuePairs[i], keyValuePairs[i + 1]);
        }
        return result;
    }

    public class DictionaryCreationException : Exception
    {
        public DictionaryCreationException(String message) : base(message)
        {
        }
    }
}
