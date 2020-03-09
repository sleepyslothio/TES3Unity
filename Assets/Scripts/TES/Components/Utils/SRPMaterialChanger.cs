using System.Collections;
using TES3Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TES3Unity.Components.Utilities
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
                var shaderName = TES3Material.URPLitPath;

#if UNITY_EDITOR
                // To prevent the script to change the material in the editor.
                material = renderer.material;
#endif

                var shader = Shader.Find(shaderName);

                if (material.shader != shader)
                    material.shader = shader;
            }
        }
#endif
    }
}
