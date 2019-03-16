using System.Collections.Generic;
using UnityEngine;

namespace TESUnity.Inputs
{
    public enum MWButton
    {
        None = 0, Jump, Light,
        Run, Slow, Attack,
        Recenter, Use, Menu,
        Teleport
    }

    public enum MWAxis
    {
        None = 0, Horizontal, Vertical, MouseX, MouseY
    }

    public static class InputManager
    {
        private static IInputProvider[] InputProviders = null;

        private static void EnsureStarted()
        {
            if (InputProviders != null)
                return;

            var providers = new IInputProvider[]
            {
                new TouchInput(),
                new OculusInput(),
                new UnityXRInput(),
                new DesktopInput()
            };

            var list = new List<IInputProvider>();
            var touchEnabled = false;
            foreach (var provider in providers)
            {
                if (provider.TryInitialize())
                {
                    if (provider is TouchInput)
                        touchEnabled = true;

                    if (provider is DesktopInput && touchEnabled)
                        continue;

                    list.Add(provider);
                }
            }

            InputProviders = list.ToArray();
        }

        public static float GetAxis(MWAxis axis)
        {
            EnsureStarted();

            var result = 0.0f;

            foreach (var provider in InputProviders)
            {
                result = provider.GetAxis(axis);

                if ((int)result != 0)
                {
                    Filter(ref result);
                    return result;
                }
            }

            return result;
        }

        private static void Filter(ref float value)
        {
            if (Mathf.Abs(value) < 0.15f)
                value = 0.0f;
        }

        public static bool GetButton(MWButton button)
        {
            EnsureStarted();

            foreach (var provider in InputProviders)
            {
                if (provider.GetButton(button))
                    return true;
            }

            return false;
        }

        public static bool GetButtonUp(MWButton button)
        {
            EnsureStarted();

            foreach (var provider in InputProviders)
            {
                if (provider.GetButtonUp(button))
                    return true;
            }

            return false;
        }

        public static bool GetButtonDown(MWButton button)
        {
            EnsureStarted();

            foreach (var provider in InputProviders)
            {
                if (provider.GetButtonDown(button))
                    return true;
            }

            return false;
        }
    }
}
