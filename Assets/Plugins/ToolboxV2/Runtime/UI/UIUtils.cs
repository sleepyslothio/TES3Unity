using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    public static class UIUtils
    {
        public static void SelectFirstButton(GameObject target, bool children)
        {
            Button btn = null;

            if (children)
            {
                btn = target.GetComponentInChildren<Button>();
            }
            else
            {
                btn = target.GetComponent<Button>();
            }

            SelectFirstButton(btn);
        }

        public static void SelectFirstButton(GameObject target)
        {
            SelectFirstButton(target, true);
        }

        public static void SelectFirstButton(Button button)
        {
            if (button != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            }
        }

        public static void SetCanvasToOverlay(Canvas canvas = null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            //canvas.transform.SetParent(null);
            canvas.transform.position = Vector3.zero;
            canvas.transform.rotation = Quaternion.identity;
        }

        public static void ScaleCanvasToScreen(GameObject canvas)
        {
            if (!canvas.TryGetComponent(out CanvasScaler canvasScaler)) return;
            
            canvasScaler.matchWidthOrHeight = 1;

            var ratio = (float)Screen.width / Screen.height;
            if (ratio >= 1.3f && ratio <= 1.5f)
                canvasScaler.matchWidthOrHeight = 0.7f;
            else if (ratio >= 2f && ratio <= 2.2f)
                canvasScaler.matchWidthOrHeight = 0.9f;
        }

        public static void ScaleCanvas(Canvas canvas = null)
        {
            var width = 1280;
            var height = 800;
            ScaleCanvas(canvas, width, height);
        }

        public static void ScaleCanvas(Canvas canvas, int width, int height)
        {
            var canvasScaler = (CanvasScaler)canvas.GetComponent(typeof(CanvasScaler));
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(width, height);
        }

        public static void SetCanvasToWorldSpace(Canvas canvas, Transform parent, float depth, float scale, float y = 0.0f)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            var canvasTransform = canvas.GetComponent<RectTransform>();
            canvasTransform.SetParent(parent);

            var canvasScaler = canvas.GetComponent<CanvasScaler>();
            if (canvasScaler != null)
            {
                canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, canvasScaler.referenceResolution.x);
                canvasTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, canvasScaler.referenceResolution.y);
            }

            canvasTransform.localPosition = new Vector3(0.0f, y, depth);
            canvasTransform.localRotation = Quaternion.identity;
            canvasTransform.localScale = new Vector3(scale, scale, scale);
        }
    }
}