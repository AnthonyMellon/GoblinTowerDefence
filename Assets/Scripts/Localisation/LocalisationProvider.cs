using System.Collections.Generic;
using UnityEngine;

public static class LocalisationProvider
{
    private static Dictionary<string, string> EnglishLocalisations = new Dictionary<string, string>()
    {
        {"UI_CURRENCY", "${0}"}
    };

    private static Dictionary<string, string> CurrentLocalisations = EnglishLocalisations;

    public static string LocaliseText(string key, string[] args, bool replaceTokens = true)
    {
        string returnString = "";
        if (CurrentLocalisations.TryGetValue(key, out returnString))
        {
            if(replaceTokens)
            {
                if (args == null)
                {
                    Debug.LogWarning($"No args given for localisation <color=red>{key}</color>");
                    return key;
                }

                returnString = ReplaceTokens(returnString, args);
            }

            return returnString;
        }
        else
        {
            Debug.LogWarning($"Localisation key <color=red>{key}</color> not found");
        }

        return key;
    }

    public static string ReplaceTokens(string tokenString, string[] args)
    {
        if (args == null) return tokenString;

        string newString = tokenString;
        for (int i = 0; i < args.Length; i++)
        {
            string replaceString = "{" + i + "}";
            newString = newString.Replace(replaceString, args[i]);
        }
        return newString;
    }
}
