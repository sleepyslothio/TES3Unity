using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Demonixis.ToolboxV2.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Translator
{
#if UNITY_EDITOR
    private static List<string> AllTranslations = new List<string>();
#endif

    private static Dictionary<string, string> Translations = new Dictionary<string, string>();
    public static SystemLanguage? CustomLanguage;
    private static bool Initialized = false;
    public static bool LogsEnabled = false;

#if UNITY_EDITOR
    public static Dictionary<string, string> LoadedTranslations => Translations;
#endif

    public static SystemLanguage GetCurrentLanguage()
    {
        if (CustomLanguage.HasValue)
        {
            return CustomLanguage.Value;
        }

        return Application.systemLanguage;
    }

    // Returns the translation for this key.
    public static string Get(string key)
    {
        Intialize();

        var keyLower = key.ToLower();

#if UNITY_EDITOR
        if (!AllTranslations.Contains(keyLower))
        {
            AllTranslations.Add(keyLower);
        }
#endif

        if (Translations.ContainsKey(keyLower))
        {
            return Translations[keyLower];
        }

#if UNITY_EDITOR
        if (LogsEnabled)
        {
            Debug.Log($"The key {keyLower} is missing");
        }
#endif

        return key;
    }

    public static string GetFormat(string key, params object[] args)
    {
        return string.Format(Get(key), args);
    }

    public static bool IsLanguageSupported(SystemLanguage language)
    {
        var data = Resources.Load<TextAsset>($"Translations/{language}");

        return data != null;
    }

    public static void ReloadWithLanguage(SystemLanguage language, bool notifyUI)
    {
        Initialized = false;
        CustomLanguage = language;
        Intialize();

        if (notifyUI)
        {
            TranslateText.ForceRefreshTranslations();
        }
    }

    private static void Intialize()
    {
        if (Initialized)
        {
            return;
        }

        Initialized = true;

        var lang = CustomLanguage.HasValue ? CustomLanguage.Value : Application.systemLanguage;
        var data = Resources.Load<TextAsset>($"Translations/{lang}");

        if (data != null)
        {
            ParseFile(data.text, lang == SystemLanguage.Arabic);
        }
    }

    public static void ParseFile(string data, bool arabic)
    {
        using (var stream = new StringReader(data))
        {
            Translations.Clear();

            var line = stream.ReadLine();
            var temp = new string[2];
            var key = string.Empty;
            var value = string.Empty;
            var size = 0;

            while (line != null)
            {
                if (line.StartsWith(";"))
                {
                    line = stream.ReadLine();
                    continue;
                }

                temp = line.Split('=');

                size = temp.Length;

                if (size >= 2)
                {
                    key = temp[0].Trim().ToLower();

                    if (size > 2)
                    {
                        value = temp[1];

                        for (var i = 2; i < size; i++)
                        {
                            value += $"={temp[i]}";
                        }
                    }
                    else
                    {
                        value = temp[1].Trim();
                    }

                    if (value != string.Empty)
                    {
                        if (arabic)
                        {
                            value = ArabicSupport.ArabicFixer.Fix(value);
                        }

                        if (Translations.ContainsKey(key))
                        {
                            Translations[key] = value;
                        }
                        else
                        {
                            Translations.Add(key, value);
                        }
                    }
                }

                line = stream.ReadLine();
            }
        }
    }

#if UNITY_EDITOR
    [MenuItem("Demonixis/Translation/Dump Missing Translations")]
    public static void DumpMissingTranslations()
    {
        if (AllTranslations?.Count == 0)
        {
            Debug.LogWarning("The Dump function can be called only in play mode.");
            return;
        }

        File.WriteAllLines("Missing-Translations.txt", AllTranslations.ToArray());
    }
#endif
}
