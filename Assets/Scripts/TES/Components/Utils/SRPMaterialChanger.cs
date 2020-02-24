using System.Collections;
using TESUnity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
#if HDRP_ENABLED
using UnityEngine.Rendering.HighDefinition;
#endif
using UnityEngine.Rendering.Universal;

namespace TESUnity.Components.Utilities
{
    [RequireComponent(typeof(Renderer))]
    public sealed class SRPMaterialChanger : MonoBehaviour
    {
#if LWRP_ENABLED || HDRP_ENABLED
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            if (GraphicsSettings.renderPipelineAsset != null)
            {
                var renderer = GetComponent<Renderer>();
                var material = renderer.sharedMaterial;
                var shaderName = string.Empty;

#if UNITY_EDITOR
                // To prevent the script to change the material in the editor.
                material = renderer.material;
#endif

                if (GraphicsSettings.renderPipelineAsset is UniversalRenderPipelineAsset)
                {
                    shaderName = TESMaterial.URPLitPath;
                }

#if HDRP_ENABLED
                if (GraphicsSettings.renderPipelineAsset is HDRenderPipelineAsset)
                    shaderName = TESMaterial.HDRPLitPath;
#endif

                var shader = Shader.Find(shaderName);

                if (material.shader != shader)
                    material.shader = shader;
            }
        }
#endif
    }
}
