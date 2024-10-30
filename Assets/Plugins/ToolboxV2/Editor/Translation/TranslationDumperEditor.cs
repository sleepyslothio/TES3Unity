#if UNITY_EDITOR
using Demonixis.ToolboxV2;
using Demonixis.ToolboxV2.UI;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TranslationDumperEditor : EditorWindow
{
    [MenuItem("Demonixis/Translation/Manager")]
    public static void ShowWindow() => GetWindow(typeof(TranslationDumperEditor));

    private SystemLanguage m_Language = SystemLanguage.English;

    private void OnGUI()
    {
        m_Language = (SystemLanguage)EditorGUILayout.EnumPopup("Language", m_Language);

        if (GUILayout.Button("Generate Diff Translations"))
        {
            GenerateDiffTranslationFile(m_Language);
        }

        if (GUILayout.Button("Dump All Translations"))
        {
            DumpAllTranslations();
        }

        if (GUILayout.Button("Generate Blank Translations"))
        {
            GenerateTranslationBlankFile();
        }
    }

    public static void GenerateDiffTranslationFile(SystemLanguage language)
    {
        if (!Translator.IsLanguageSupported(language))
        {
            Debug.Log($"The language {language} is not supported. Generating a blank file instead");
            GenerateTranslationBlankFile();
            return;
        }

        Translator.ReloadWithLanguage(language, false);

        var translations = GetAllTranslations();
        var doc = new List<string>(translations.Length);
        var current = Translator.LoadedTranslations;

        foreach (var translation in translations)
        {
            var lower = translation.Trim().ToLower();

            if (current.ContainsKey(lower))
            {
                doc.Add($"{lower} = {current[lower]}");
            }
            else
            {
                doc.Add($"{lower} = ");
            }
        }

        File.WriteAllLines($"{language}.txt", doc.ToArray());
    }

    [MenuItem("Demonixis/Translation/Dump All Translations")]
    public static void DumpAllTranslations()
    {
        var translations = GetAllTranslations();
        File.WriteAllLines("All-Translations.txt", translations);
    }

    [MenuItem("Demonixis/Translation/Generate Translations")]
    public static void GenerateTranslationBlankFile()
    {
        var translations = GetAllTranslations();

        var doc = new List<string>(translations.Length);

        foreach (var translation in translations)
        {
            doc.Add($"{translation} = ");
        }

        File.WriteAllLines("Blank-Translations.txt", doc.ToArray());
    }

    private static string[] GetAllTranslations()
    {
        var allTranslations = new List<string>();

        foreach (var scene in EditorBuildSettings.scenes)
        {
            EditorSceneManager.OpenScene(scene.path);

            var translateObjects = FindObjectsByType<TranslateObject>(FindObjectsSortMode.None);

            foreach (var obj in translateObjects)
            {
                var translations = obj.GetComponentsInChildren<TranslateText>(true);

                foreach (var translation in translations)
                {
                    TryAddTranslation(allTranslations, translation.TranslationKey);
                }
            }

            var allCanvas = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);

            foreach (var canvas in allCanvas)
            {
                var translations = canvas.GetComponentsInChildren<TranslateText>(true);

                foreach (var translation in translations)
                {
                    TryAddTranslation(allTranslations, translation.TranslationKey);
                }
            }
        }

        return allTranslations.ToArray();
    }

    private static void TryAddTranslation(List<string> allTranslations, string key)
    {
        var lower = key.Trim().ToLower();
        if (!allTranslations.Contains(lower))
        {
            allTranslations.Add(lower);
        }
    }
}
#endif