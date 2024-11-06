using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Demonixis.ToolboxV2.Utils
{
    public sealed class CommandLineParser
    {
        private static CommandLineParser s_CommandLineParser = null;

        private readonly Dictionary<string, string> m_Parameters = new Dictionary<string, string>();

        public string this[string key] => GetString(key);

        public static CommandLineParser Get()
        {
            if (s_CommandLineParser == null)
            {
                s_CommandLineParser = new CommandLineParser();
#if UNITY_STANDALONE
                var configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", Application.productName);
                var filePath = Path.Combine(configPath, $"config.ini");
                s_CommandLineParser.ParseConfigFile(filePath);
#endif
                // Command Line overrides config file.
                s_CommandLineParser.ParseCommandLine(Environment.CommandLine);
            }

            return s_CommandLineParser;
        }

        public void AddFromParser(CommandLineParser parser, bool replace)
        {
            var additionalParameters = parser.m_Parameters;

            foreach (var keyValue in additionalParameters)
            {
                var contains = m_Parameters.ContainsKey(keyValue.Key);
                if (contains && replace)
                {
                    m_Parameters[keyValue.Key] = keyValue.Value;
                }
                else if (!contains)
                {
                    m_Parameters.Add(keyValue.Key, keyValue.Value);
                }
            }
        }

        public void ParseCommandLine(ref string data)
        {
            ParseCommandLine(data);
        }

        public void AddKey(string key, string value)
        {
            AddKey(ref key, ref value);
        }

        public void AddKey(ref string key, ref string value)
        {
            if (Contains(ref key))
            {
                m_Parameters[key] = value;
            }
            else
            {
                m_Parameters.Add(key, value);
            }
        }

        public void AddKeys(ref Dictionary<string, string> data)
        {
            foreach (var keyValue in data)
            {
                AddKey(keyValue.Key, keyValue.Value);
            }
        }

        public bool HasKey(string key)
        {
            return Contains(ref key);
        }

        public void GetFloat(string key, ref float property)
        {
            if (Contains(ref key))
            {
                var value = m_Parameters[key].Replace('.', ',');

                if (float.TryParse(value, out float result))
                {
                    property = result;
                }
            }
        }

        public float GetFloat(string key)
        {
            if (Contains(ref key))
            {
                var value = m_Parameters[key].Replace('.', ',');

                if (float.TryParse(value, out float result))
                {
                    return result;
                }
            }

            return 0.0f;
        }

        public void GetInteger(string key, ref int property)
        {
            if (Contains(ref key))
            {
                var value = m_Parameters[key];

                if (int.TryParse(value, out int result))
                {
                    property = result;
                }
            }
        }

        public int GetInteger(string key)
        {
            if (Contains(ref key))
            {
                var value = m_Parameters[key];

                if (int.TryParse(value, out int result))
                {
                    return result;
                }
            }

            return 0;
        }

        public void GetBool(string key, ref bool property)
        {
            if (Contains(ref key))
            {
                var b = m_Parameters[key].ToLower();
                property = (b == "true" || b == "1");
            }
        }

        public bool GetBool(string key)
        {
            var value = false;
            GetBool(key, ref value);
            return value;
        }

        public void GetString(string key, ref string property)
        {
            if (Contains(ref key))
            {
                property = m_Parameters[key];
            }
        }

        public string GetString(string key)
        {
            if (Contains(ref key))
            {
                return m_Parameters[key];
            }

            return string.Empty;
        }

        private bool Contains(ref string key)
        {
            key = key.Trim();
            return m_Parameters.ContainsKey(key);
        }

        private string Get(ref string key)
        {
            if (Contains(ref key))
            {
                return m_Parameters[key];
            }

            return string.Empty;
        }

        public void ParseCommandLine(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            var commands = data.Split(' ');
            var property = string.Empty;
            var value = string.Empty;
            string[] tmp;

            for (var i = 0; i < commands.Length; i++)
            {
                tmp = commands[i].Split('=');

                if (tmp.Length != 2)
                {
                    continue;
                }

                property = tmp[0].Trim();
                value = tmp[1].Trim();

                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }

                if (!m_Parameters.ContainsKey(property))
                {
                    m_Parameters.Add(property, value);
                }
                else
                {
                    m_Parameters[property] = value;
                }
            }
        }

        public void ParseConfigFile(string filePath)
        {
#if !UNITY_STANDALONE
            return;
#endif
            if (!File.Exists(filePath))
            {
                return;
            }

            var data = File.ReadAllText(filePath);

            using (var stream = new StringReader(data))
            {
                var line = stream.ReadLine();
                var temp = new string[2];
                var key = string.Empty;
                var value = string.Empty;

                while (line != null)
                {
                    if (line.StartsWith(";"))
                    {
                        line = stream.ReadLine();
                        continue;
                    }

                    temp = line.Split('=');

                    if (temp.Length == 2)
                    {
                        key = temp[0].Trim();
                        value = temp[1].Trim();

                        if (value != string.Empty)
                        {
                            if (m_Parameters.ContainsKey(key))
                            {
                                m_Parameters[key] = value;
                            }
                            else
                            {
                                m_Parameters.Add(key, value);
                            }
                        }
                    }

                    line = stream.ReadLine();
                }
            }
        }
    }
}
