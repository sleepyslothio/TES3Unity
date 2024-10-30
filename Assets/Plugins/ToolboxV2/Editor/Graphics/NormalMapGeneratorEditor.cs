#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Demonixis.ToolboxV2
{
    public sealed class NormalMapGeneratorEditor : EditorWindow
    {
        private readonly string[] SearchedShaderName =
        {
            "standard", "diffuse", "universal", "hdrp"
        };

        private float m_Strength = 1;
        private bool m_ForceRegenerate = false;
        private string m_SavePath = string.Empty;
        private int m_JPEGQuality = 95;
        private bool m_BatchMode = false;
        private Texture m_AlbedoTexture = null;

        [MenuItem("Demonixis/NormalMap Generator")]
        public static void ShowWindow() => GetWindow(typeof(NormalMapGeneratorEditor));

        private void OnGUI()
        {
            GUILayout.Label("Choose a mode", EditorStyles.boldLabel);
            m_BatchMode = GUILayout.Toggle(m_BatchMode, "Batch Mode");

            GUILayout.Space(5f);
            GUILayout.Label("Generation Settings", EditorStyles.boldLabel);

            if (!m_BatchMode)
            {
                m_AlbedoTexture = EditorGUILayout.ObjectField("Albedo Texture", m_AlbedoTexture, typeof(Texture), true) as Texture;
            }

            m_Strength = EditorGUILayout.FloatField("Strength", m_Strength);
            m_JPEGQuality = (int)EditorGUILayout.FloatField("JPEG Quality", m_JPEGQuality);

            if (m_BatchMode)
            {
                m_ForceRegenerate = GUILayout.Toggle(m_ForceRegenerate, "Regenerate Maps");
            }

            GUILayout.Space(5f);
            GUILayout.Label("Save Settings", EditorStyles.boldLabel);
            m_SavePath = GUILayout.TextField(m_SavePath);

            GUILayout.Space(5f);

            if (GUILayout.Button("Generate"))
            {
                if (m_BatchMode)
                {
                    var materialsGUID = AssetDatabase.FindAssets("t:Material");

                    if (!string.IsNullOrEmpty(m_SavePath) && !Directory.Exists(m_SavePath))
                    {
                        Directory.CreateDirectory(m_SavePath);
                    }

                    foreach (var guid in materialsGUID)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                        var shaderName = material.shader.name.ToLower();

                        if (!IsValidShader(ref shaderName))
                        {
                            continue;
                        }

                        if (material.mainTexture == null)
                        {
                            continue;
                        }

                        if (material.GetTexture("_BumpMap") != null && !m_ForceRegenerate)
                        {
                            continue;
                        }

                        var albedo = material.mainTexture;
                        var bump = GenerateNormal(albedo);

                        material.EnableKeyword("_NORMALMAP");
                        material.SetTexture("_BumpMap", bump);

                        Debug.Log($"Create NormalMap for material {material.name}");
                    }

                    //AssetDatabase.SaveAssets();
                }
                else
                {
                    GenerateNormal(m_AlbedoTexture);
                }
            }
        }

        private Texture2D GenerateNormal(Texture albedo)
        {
            var assetPath = AssetDatabase.GetAssetPath(albedo);
            var albedoName = Path.GetFileName(assetPath);
            var albedoNameWE = Path.GetFileNameWithoutExtension(assetPath);
            var assetAbs = assetPath.Replace(albedoName, "");
            var clone = Clone(albedo);
            var bump = GenerateNormalMap(clone, m_Strength);
            var jpg = bump.EncodeToJPG(m_JPEGQuality);

            if (!string.IsNullOrEmpty(m_SavePath))
            {
                assetAbs = m_SavePath;
            }

            assetPath = Path.Combine(assetAbs, $"{albedoNameWE}_Normal.jpg");

            File.WriteAllBytes(assetPath, jpg);

            AssetDatabase.ImportAsset(assetPath);

            return bump;
        }

        private Texture2D Clone(Texture source)
        {
            var target = (Texture2D)source;
            var dest = new Texture2D(source.width, source.height, target.format, target.mipmapCount > 0);

            UnityEngine.Graphics.CopyTexture(target, dest);

            dest.Apply();

            return dest;
        }

        private bool IsValidShader(ref string name)
        {
            foreach (var item in SearchedShaderName)
            {
                if (name.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        // https://gamedev.stackexchange.com/questions/106703/create-a-normal-map-using-a-script-unity
        private static Texture2D GenerateNormalMap(Texture2D source, float strength)
        {
            strength = Mathf.Clamp(strength, 0.0F, 100.0f);

            Texture2D normalTexture;
            float xLeft;
            float xRight;
            float yUp;
            float yDown;
            float yDelta;
            float xDelta;

            normalTexture = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);

            for (int y = 0; y < normalTexture.height; y++)
            {
                for (int x = 0; x < normalTexture.width; x++)
                {
                    xLeft = source.GetPixel(x - 1, y).grayscale * strength;
                    xRight = source.GetPixel(x + 1, y).grayscale * strength;
                    yUp = source.GetPixel(x, y - 1).grayscale * strength;
                    yDown = source.GetPixel(x, y + 1).grayscale * strength;
                    xDelta = ((xLeft - xRight) + 1) * 0.5f;
                    yDelta = ((yUp - yDown) + 1) * 0.5f;
                    normalTexture.SetPixel(x, y, new Color(xDelta, yDelta, 1.0f, yDelta));
                }
            }

            normalTexture.Apply();

            return normalTexture;
        }
    }
}
#endif