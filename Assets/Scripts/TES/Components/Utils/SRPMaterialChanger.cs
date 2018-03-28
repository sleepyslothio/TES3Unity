using UnityEngine;

namespace TESUnity.Components.Utils
{
    [RequireComponent(typeof(Renderer))]
    public sealed class SRPMaterialChanger : MonoBehaviour
    {
        [SerializeField]
        private Shader m_LightweightShaderPBR = null;
        [SerializeField]
        private Shader m_LightweightShaderSimple = null;
        [SerializeField]
        private Shader m_UnlitShader = null;
        [SerializeField]
        private Shader m_HDShader = null;

        private void Start()
        {
            var renderer = GetComponent<Renderer>();
            var material = renderer.sharedMaterial;

            var tes = TESManager.instance;

            var shader = m_UnlitShader;

            if (tes.renderPath == TESManager.RendererType.LightweightSRP)
            {
                if (tes.materialType == TESManager.MWMaterialType.StandardLighting)
                    shader = m_LightweightShaderSimple;
                else if (tes.materialType == TESManager.MWMaterialType.PBR)
                    shader = m_LightweightShaderPBR;
            }
            else if (tes.renderPath == TESManager.RendererType.HDSRP)
                shader = m_HDShader;

            if (material.shader != shader)
                material.shader = shader;
        }
    }
}
