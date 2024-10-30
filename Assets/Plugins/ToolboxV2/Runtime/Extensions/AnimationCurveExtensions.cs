using UnityEngine;

namespace Demonixis.ToolboxV2
{
    public static class AnimationCurveExtensions
    {
        public static void UpdateKey(this AnimationCurve curve, int index, float time, float value)
        {
            var keys = curve.keys;
            keys[index].time = time;
            keys[index].value = value;
            curve.keys = keys;
        }
    }
}
