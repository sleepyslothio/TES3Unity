using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity.Inputs
{
    public static class InputManager
    {
        private static InputActionAsset InputActions = null;

        static InputManager()
        {
            InputActions = Resources.Load<InputActionAsset>("Input/TESInputActions");
        }

        public static InputActionMap GetActionMap(string actionMap)
        {
            var map = InputActions.FindActionMap(actionMap);

            if (map == null)
            {
                Debug.LogError($"Can't find the action map {actionMap}");
            }

            return map;
        }

        public static InputAction GetAction(string actionMap, string actionName)
        {
            var map = GetActionMap(actionMap);
            return map?[actionMap];
        }

        public static void Enable(string actionMap)
        {
            var map = GetActionMap(actionMap);
            map?.Enable();
        }

        public static void Disable(string actionMap)
        {
            var map = GetActionMap(actionMap);
            map?.Disable();
        }
    }
}
