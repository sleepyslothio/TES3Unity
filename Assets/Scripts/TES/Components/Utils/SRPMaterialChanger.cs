using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace TESUnity.Components.Utilities
{
    [RequireComponent(typeof(Renderer))]
    public sealed class SRPMaterialChanger : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            if (GraphicsSettings.renderPipelineAsset != null)
            {
                var renderer = GetComponent<Renderer>();
                var material = renderer.sharedMaterial;
#if UNITY_EDITOR
                // To prevent the script to change the material in the editor.
                material = renderer.material;
#endif
                var pbr = TESManager.instance.materialType == TESManager.MWMaterialType.PBR;
                var shaderName = pbr ? LightweightMaterial.LitName : LightweightMaterial.SimpleLitName;
                var shader = Shader.Find(shaderName);

                if (material.shader != shader)
                    material.shader = shader;
            }
        }
    }
}
