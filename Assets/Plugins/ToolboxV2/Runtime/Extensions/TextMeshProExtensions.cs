using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Demonixis.ToolboxV2
{
    public static class TextMeshProExtensions
    {
        private static Func<string, string> TranslatorFunc = null;

        static TextMeshProExtensions()
        {
            TranslatorFunc = Translator.Get;
        }

        public static void SetTranslatorFunction(Func<string, string> func)
        {
            TranslatorFunc = func;
        }

        public static void SetValue(this TMP_Dropdown dropdown, string value)
        {
            var options = dropdown.options;

            for (var i = 0; i < options.Count; i++)
            {
                if (options[i].text == value)
                {
                    dropdown.value = i;
                    dropdown.RefreshShownValue();
                    return;
                }
            }
        }

        public static void SetupEnum<T>(this TMP_Dropdown dropdown, int value, UnityAction<int> callback, bool clearPreviousListeners = false)
        {
            if (clearPreviousListeners)
            {
                dropdown.onValueChanged.RemoveAllListeners();
            }

            var names = Enum.GetNames(typeof(T));
            var values = Enum.GetValues(typeof(T));
            var valueIndex = 0;

            dropdown.options.Clear();

            for (var i = 0; i < names.Length; i++)
            {
                var text = names[i];

                if (TranslatorFunc != null)
                {
                    text = TranslatorFunc(names[i]);
                }

                dropdown.options.Add(new TMP_Dropdown.OptionData
                {
                    text = names[i]
                });

                var enumValue = (int)values.GetValue(i);

                if (enumValue == value)
                {
                    valueIndex = i;
                }
            }

            dropdown.value = valueIndex;
            dropdown.RefreshShownValue();

            if (callback != null)
            {
                dropdown.onValueChanged.AddListener(callback);
            }
        }

        public static void SetupStrings(this TMP_Dropdown dropdown, string[] names, int value, UnityAction<int> callback, bool clearPreviousListeners = false)
        {
            if (clearPreviousListeners)
            {
                dropdown.onValueChanged.RemoveAllListeners();
            }

            dropdown.options.Clear();

            for (var i = 0; i < names.Length; i++)
            {
                var text = names[i];

                if (TranslatorFunc != null)
                {
                    text = TranslatorFunc(names[i]);
                }

                dropdown.options.Add(new TMP_Dropdown.OptionData
                {
                    text = names[i]
                });
            }

            dropdown.value = value;
            dropdown.RefreshShownValue();

            if (callback != null)
            {
                dropdown.onValueChanged.AddListener(callback);
            }
        }

        public static void SetupFloats(this TMP_Dropdown dropdown, float[] array, float value, UnityAction<int> callback, bool clearPreviousListeners = false)
        {
            if (clearPreviousListeners)
            {
                dropdown.onValueChanged.RemoveAllListeners();
            }

            dropdown.options.Clear();

            var valueIndex = 0;

            for (var i = 0; i < array.Length; i++)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData
                {
                    text = array[i].ToString()
                });

                if (Mathf.Approximately(array[i], value))
                {
                    valueIndex = i;
                }
            }

            dropdown.value = valueIndex;

            if (callback != null)
            {
                dropdown.onValueChanged.AddListener(callback);
            }
        }

        public static void SetupUShorts(this TMP_Dropdown dropdown, ushort[] array, ushort value, UnityAction<int> callback, bool clearPreviousListeners = false)
        {
            if (clearPreviousListeners)
            {
                dropdown.onValueChanged.RemoveAllListeners();
            }

            dropdown.options.Clear();

            var valueIndex = 0;

            for (var i = 0; i < array.Length; i++)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData
                {
                    text = array[i].ToString()
                });

                if (array[i] == value)
                {
                    valueIndex = i;
                }
            }

            dropdown.value = valueIndex;

            if (callback != null)
            {
                dropdown.onValueChanged.AddListener(callback);
            }
        }

        public static void SetupRange(this TMP_Dropdown dropdown, ushort start, ushort end, ushort value, UnityAction<int> callback, bool clearPreviousListeners = false)
        {
            if (clearPreviousListeners)
            {
                dropdown.onValueChanged.RemoveAllListeners();
            }

            dropdown.options.Clear();

            var valueIndex = 0;

            for (var i = start; i <= end; i++)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData
                {
                    text = i.ToString()
                });

                if (i == value)
                {
                    valueIndex = i;
                }
            }

            dropdown.value = valueIndex;

            if (callback != null)
            {
                dropdown.onValueChanged.AddListener(callback);
            }
        }

        public static void SetupResolution(this TMP_Dropdown dropdown, Resolution[] array, Resolution value, UnityAction<int> callback, bool clearPreviousListeners = false)
        {
            if (clearPreviousListeners)
            {
                dropdown.onValueChanged.RemoveAllListeners();
            }

            dropdown.options.Clear();

            var valueIndex = 0;

            for (var i = 0; i < array.Length; i++)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData
                {
                    text = $"{array[i].width}x{array[i].height}"
                });

                if (array[i].width == value.width && array[i].height == value.height)
                {
                    valueIndex = i;
                }
            }

            dropdown.value = valueIndex;

            if (callback != null)
            {
                dropdown.onValueChanged.AddListener(callback);
            }
        }

        public static TMP_Dropdown.OptionData AddOption(this TMP_Dropdown dropdown, string itemName)
        {
            var item = new TMP_Dropdown.OptionData(itemName);
            dropdown.options.Add(item);
            return item;
        }

        public static TMP_Dropdown.OptionData GetSelectedItem(this TMP_Dropdown dropdown)
        {
            var index = dropdown.value;
            return dropdown.options[index];
        }
    }
}
