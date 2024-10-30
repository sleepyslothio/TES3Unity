using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Demonixis.ToolboxV2.Graphics
{
    public sealed class ShaderSwitcher : MonoBehaviour
    {
        private static ShaderSwitcher instance = null;
        private List<Material> m_OldMaterials = new List<Material>();
        private List<Material> m_NewMaterials = new List<Material>();
        private Material[] m_ModifiedMaterials = null;

        [Header("Settings")]
        [SerializeField]
        private Transform m_TargetRenderers = null;
        [SerializeField]
        private bool m_AutoStart = false;
        [SerializeField]
        private bool m_UseChildrenRenderers = false;
        [SerializeField]
        private bool m_KeepNormalMaps = true;
        [SerializeField]
        private bool m_KeepEmissive = true;

        [Header("Shaders")]
        [SerializeField]
        private ReplacementList[] m_ReplacementList = null;

        public static ShaderSwitcher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<ShaderSwitcher>();
                }

                return instance;
            }
        }

        public int ShaderReplacementIndex { get; set; } = 0;
        public bool KeepNormalMaps
        {
            get => m_KeepNormalMaps;
            set => m_KeepNormalMaps = value;
        }

        public bool KeepEmissiveMaps
        {
            get => m_KeepEmissive;
            set => m_KeepEmissive = value;
        }

        private void OnEnable()
        {
            if (!m_AutoStart)
            {
                return;
            }

            SwitchShaders();
        }

        private Renderer[] GetRenderers()
        {
            if (m_TargetRenderers != null)
            {
                return m_TargetRenderers.GetComponentsInChildren<Renderer>(true);
            }
            else if (m_UseChildrenRenderers)
            {
                return GetComponentsInChildren<Renderer>(true);
            }

            return FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        }

        public void SwitchShaders(Renderer[] renderers = null)
        {
            if (m_ReplacementList == null || m_ReplacementList.Length == 0)
                return;
            
            renderers ??= GetRenderers();

            var materialsReplaced = 0;
            var materialInstancesReplaced = 0;
            var items = m_ReplacementList[ShaderReplacementIndex].items;
            Material material = null;
            Material newMaterial = null;
            Texture normalTexture = null;
            Texture emissiveTexture = null;

#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                return;
            }

            Debug.Log(renderers.Length + " renderers");
#endif

            for (int i = 0, l = items.Length; i < l; i++)
            {
                for (int j = 0, k = renderers.Length; j < k; j++)
                {
                    m_ModifiedMaterials = null;

                    for (int n = 0; n < renderers[j].sharedMaterials.Length; ++n)
                    {
                        material = renderers[j].sharedMaterials[n];

                        if (material == null || material.shader == null)
                        {
                            continue;
                        }

                        if (material.shader == items[i].original)
                        {
                            if (m_ModifiedMaterials == null)
                            {
                                m_ModifiedMaterials = renderers[j].materials;
                            }

                            if (!m_OldMaterials.Contains(material))
                            {
                                m_OldMaterials.Add(material);
                                newMaterial = Instantiate(material);
                                newMaterial.shader = items[i].replacement;

                                if (m_KeepNormalMaps)
                                {
                                    if (material.HasProperty("_BumpMap"))
                                    {
                                        normalTexture = material.GetTexture("_BumpMap");
                                        newMaterial.SetTexture("_BumpMap", normalTexture);
                                        newMaterial.EnableKeyword("_NORMALMAP");
                                    }
                                }
                                else
                                {
                                    newMaterial.DisableKeyword("_NORMALMAP");
                                }

                                if (m_KeepEmissive)
                                {
                                    var hasEmissionMap = material.HasProperty("_EmissiveMap");
                                    var hasEmissionColor = material.HasProperty("_EmissiveColor");

                                    if (hasEmissionMap)
                                    {
                                        emissiveTexture = material.GetTexture("_EmissiveMap");
                                        newMaterial.SetTexture("_EmissiveMap", emissiveTexture);
                                    }

                                    if (hasEmissionColor)
                                    {
                                        var emissionColor = material.GetColor("_EmissionColor");
                                        newMaterial.SetColor("_EmissionColor", emissionColor);
                                    }

                                    if (hasEmissionColor || hasEmissionMap)
                                    {
                                        newMaterial.EnableKeyword("_EMISSION");
                                    }
                                }
                                else
                                {
                                    newMaterial.DisableKeyword("_EMISSION");
                                }

                                m_NewMaterials.Add(newMaterial);
                                ++materialsReplaced;
                            }

                            m_ModifiedMaterials[n] = m_NewMaterials[m_OldMaterials.IndexOf(material)];
                            ++materialInstancesReplaced;
                        }
                    }

                    if (m_ModifiedMaterials != null)
                    {
                        renderers[j].materials = m_ModifiedMaterials;
                    }
                }
            }

#if UNITY_EDITOR
            Debug.Log(materialInstancesReplaced + " material instances replaced");
            Debug.Log(materialsReplaced + " materials replaced");
#endif
        }

        [Serializable]
        public class ReplacementDefinition
        {
            public Shader original = null;
            public Shader replacement = null;
        }

        [Serializable]
        public class ReplacementList
        {
            public ReplacementDefinition[] items = new ReplacementDefinition[0];
        }
    }

    namespace UnityStandardAssets.Utility.Inspector
    {
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(ShaderSwitcher.ReplacementList))]
        public class ReplacementListDrawer : PropertyDrawer
        {
            private const float k_LineHeight = 18;
            private const float k_Spacing = 4;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property);

                var x = position.x;
                var y = position.y;
                var inspectorWidth = position.width;

                // Don't make child fields be indented
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                var items = property.FindPropertyRelative("items");
                var titles = new string[] { "Original", "Replacement", "" };
                var props = new string[] { "original", "replacement", "-" };
                var widths = new float[] { .45f, .45f, .1f };

                var changedLength = false;

                if (items.arraySize > 0)
                {
                    for (var i = -1; i < items.arraySize; ++i)
                    {
                        var item = items.GetArrayElementAtIndex(i);
                        var rowX = x;

                        for (int n = 0; n < props.Length; ++n)
                        {
                            var w = widths[n] * inspectorWidth;

                            // Calculate rects
                            var rect = new Rect(rowX, y, w, k_LineHeight);
                            rowX += w;

                            if (i == -1)
                            {
                                // draw title labels
                                EditorGUI.LabelField(rect, titles[n]);
                            }
                            else
                            {
                                if (props[n] == "-" || props[n] == "^" || props[n] == "v")
                                {
                                    if (GUI.Button(rect, props[n]))
                                    {
                                        if (props[n] == "-")
                                        {
                                            items.DeleteArrayElementAtIndex(i);
                                            items.DeleteArrayElementAtIndex(i);
                                            changedLength = true;
                                        }
                                        else if (props[n] == "v")
                                        {
                                            if (i > 0)
                                            {
                                                items.MoveArrayElement(i, i + 1);
                                            }
                                        }
                                        else if (props[n] == "^")
                                        {
                                            if (i < items.arraySize - 1)
                                            {
                                                items.MoveArrayElement(i, i - 1);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    SerializedProperty prop = item.FindPropertyRelative(props[n]);
                                    EditorGUI.PropertyField(rect, prop, GUIContent.none);
                                }
                            }
                        }

                        y += k_LineHeight + k_Spacing;

                        if (changedLength)
                        {
                            break;
                        }
                    }
                }

                // add button
                var addButtonRect = new Rect((x + position.width) - widths[widths.Length - 1] * inspectorWidth, y, widths[widths.Length - 1] * inspectorWidth, k_LineHeight);

                if (GUI.Button(addButtonRect, "+"))
                {
                    items.InsertArrayElementAtIndex(items.arraySize);
                }

                y += k_LineHeight + k_Spacing;

                // Set indent back to what it was
                EditorGUI.indentLevel = indent;
                EditorGUI.EndProperty();
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                SerializedProperty items = property.FindPropertyRelative("items");
                var lineAndSpace = k_LineHeight + k_Spacing;
                return 40 + (items.arraySize * lineAndSpace) + lineAndSpace;
            }
        }
#endif
    }
}