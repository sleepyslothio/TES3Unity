using UnityEngine;
using UnityEngine.InputSystem;

namespace Demonixis.ToolboxV2.Inputs
{
    public static class InputSystemManager
    {
        private static readonly InputActionAsset InputActions;

        static InputSystemManager()
        {
            InputActions = InputSystem.actions;
        }

        public static void SetActive(string map, bool active)
        {
            var actionMap = GetActionMap(map);
            if (actionMap == null) return;

            if (active)
                actionMap.Enable();
            else
                actionMap.Disable();
        }

        public static InputActionMap GetActionMap(string actionMap)
        {
            var map = InputActions.FindActionMap(actionMap);

            if (map == null)
                Debug.LogError($"Can't find the action map {actionMap}");

            return map;
        }

        public static InputAction GetAction(string map, string actionName, bool clone = false)
        {
            var actionMap = GetActionMap(map);
            if (actionMap == null) return null;

            var action = actionMap[actionName];
            return clone ? action.Clone() : action;
        }

        public static InputActionMap Enable(string actionMap)
        {
            var map = GetActionMap(actionMap);
            if (map is { enabled: false }) 
                map.Enable();

            return map;
        }

        public static InputActionMap Disable(string actionMap)
        {
            var map = GetActionMap(actionMap);
            if (map is { enabled: true})
                map.Disable();
            
            return map;
        }
    }
}