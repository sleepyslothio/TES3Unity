using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    [ExecuteInEditMode]
    public sealed class UISliderValue : MonoBehaviour
    {
        [SerializeField]
        private Slider m_Slider = null;
        [SerializeField]
        private Text m_Label = null;
        [SerializeField]
        private string m_Unit = string.Empty;
        [SerializeField]
        private int m_Truncate = 0;

        public bool WholeNumbers
        {
            get => m_Slider.wholeNumbers;
            set => m_Slider.wholeNumbers = value;
        }

        public event Action<float> ValueChanged = null;

        public float Value
        {
            get => m_Slider.value;
            set
            {
                m_Slider.SetValueWithoutNotify(value);
                ShowValue(value);
            }
        }

        private void Awake()
        {
            m_Slider.onValueChanged.AddListener(OnValueChanged);
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                return;
            }

            OnValidate();
#endif
        }

        public void SetMinMax(float min, float max)
        {
            m_Slider.minValue = min;
            m_Slider.maxValue = max;
        }

        private void OnValueChanged(float value)
        {
            ShowValue(value);
            ValueChanged?.Invoke(value);
        }

        private void ShowValue(float value)
        {
            if (m_Truncate > 0)
            {
                value = Mathf.Round(value * m_Truncate) / m_Truncate;
            }

            m_Label.text = $"{value}{m_Unit}";
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (m_Label == null)
            {
                return;
            }
#endif
            m_Label.text = $"{m_Slider.value}{m_Unit}";
        }
    }
}