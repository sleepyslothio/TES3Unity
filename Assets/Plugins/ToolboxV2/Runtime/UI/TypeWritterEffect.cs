using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    [RequireComponent(typeof(Text))]
    public sealed class TypeWritterEffect : MonoBehaviour
    {
        private static StringBuilder m_StringBuilder = new StringBuilder();
        private Text m_Text = null;
        private int m_Size = 0;
        private float m_ElaspedTime = 0;
        private string m_ContentText = string.Empty;
        private bool m_Done = true;
        private int m_Index = 0;
        private List<string> m_ContentTexts = null;

        [SerializeField]
        private float m_CycleDuration = 0.05f;
        [SerializeField]
        private int m_LetterPerCycle = 1;
        [SerializeField]
        private bool m_AutoStart = false;
        [SerializeField]
        private bool m_ActiveOnEnable = false;

        public bool AutoStart
        {
            get => m_AutoStart;
            set => m_AutoStart = value;
        }

        public bool IsActive => !m_Done;
        public float CycleDuration => m_CycleDuration;
        public int LetterPerCycle => m_LetterPerCycle;
        public Text Text => m_Text;

        public event Action Completed = null;

        private void OnEnable()
        {
            EnsureStarted();

            if (m_ActiveOnEnable)
            {
                m_AutoStart = false;
                Begin();
            }
        }

        private void Start()
        {
            EnsureStarted();

            if (m_AutoStart)
            {
                Begin();
            }
        }

        private void EnsureStarted()
        {
            if (m_Text != null)
            {
                return;
            }

            m_ContentTexts = new List<string>();
            m_Text = GetComponent<Text>();
        }

        private void Update()
        {
            if (!m_Done)
            {
                m_ElaspedTime += Time.deltaTime;
                UpdateText();
            }
        }

        public void Begin(string text = null)
        {
            EnsureStarted();

            m_ContentText = text == null ? m_Text.text : text;
            m_Size = m_ContentText.Length;
            m_ElaspedTime = m_CycleDuration;
            m_Index = 0;
            m_Done = false;
            m_StringBuilder.Length = 0;
            m_Text.text = string.Empty;
        }

        public void Stop()
        {
            m_ElaspedTime = m_CycleDuration;
            m_Done = true;
        }

        private void UpdateText()
        {
            if (m_ElaspedTime >= m_CycleDuration)
            {
                var limit = Mathf.Min(m_Index + m_LetterPerCycle, m_Size);

                for (int i = m_Index; i < limit; i++)
                {
                    m_StringBuilder.Append(m_ContentText[i]);
                }

                m_Index = limit;
                m_Text.text = m_StringBuilder.ToString();
                m_ElaspedTime = 0;

                if (m_Index >= m_Size)
                {
                    m_Done = true;

                    Completed?.Invoke();
                }
            }
        }
    }
}