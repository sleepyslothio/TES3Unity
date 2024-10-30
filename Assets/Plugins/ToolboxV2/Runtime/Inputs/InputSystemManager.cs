using UnityEngine;
using UnityEngine.InputSystem;

namespace Demonixis.ToolboxV2.Inputs
{
    public static class InputSystemManager
    {
        public static InputActionAsset InputActions { get; set; }

        static InputSystemManager()
        {
            if (InputActions == null)
            {
                InputActions = Resources.Load<InputActionAsset>("InputActions");
            }
        }

        public static void SetActive(string map, bool active)
        {
            var actionMap = GetActionMap(map);
            if (actionMap != null)
            {
                if (active)
                {
                    actionMap.Enable();
                }
                else
                {
                    actionMap.Disable();
                }
            }
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

        public static InputAction GetAction(string map, string actionName, bool clone = false)
        {
            var actionMap = GetActionMap(map);
            if (actionMap == null)
            {
                return null;
            }

            var action = actionMap[actionName];
            return clone ? action.Clone() : action;
        }

        public static InputActionMap Enable(string actionMap)
        {
            var map = GetActionMap(actionMap);
            map?.Enable();
            return map;
        }

        public static InputActionMap Disable(string actionMap)
        {
            var map = GetActionMap(actionMap);
            map?.Disable();
            return map;
        }
    }
}
