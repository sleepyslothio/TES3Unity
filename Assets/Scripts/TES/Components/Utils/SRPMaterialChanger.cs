using System.Collections;
using TESUnity.Rendering;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

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

                if (GraphicsSettings.renderPipelineAsset is LightweightRenderPipelineAsset)
                {
                    var pbr = GameSettings.Get().MaterialType == MWMaterialType.PBR;
                    shaderName = pbr ? LWRPMaterial.LitPath : LWRPMaterial.SimpleLitPath;

                }
                else if (GraphicsSettings.renderPipelineAsset is HDRenderPipelineAsset)
                    shaderName = HDRPMaterial.LitPath;

                var shader = Shader.Find(shaderName);

                if (material.shader != shader)
                    material.shader = shader;
            }
        }
#endif
    }
}
