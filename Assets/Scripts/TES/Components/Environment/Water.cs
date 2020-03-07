using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Effects
{
    public sealed class Water : MonoBehaviour
    {
        private bool _defaultFog;
        private Color _defaultFogColor;
        private float _defaultFogDensity;
        private Material _defaultSkybox = null;
        private bool _isUnderwater = false;
        private Transform _transform = null;
        private bool _enabled = false;
        private bool _hdrp = false;

        [SerializeField]
        private float underwaterLevel = 0.0f;
        [SerializeField]
        private Color ambientColor = Color.white;
        [SerializeField]
        private Color fogColor = new Color(0, 0.4f, 0.7f, 0.6f);
        [SerializeField]
        private float fogDensity = 0.04f;

        public float Level
        {
            get { return underwaterLevel; }
            set { underwaterLevel = value; }
        }

        private void Awake()
        {
            var tes = TES3Manager.instance;
            tes.Engine.CurrentCellChanged += Engine_CurrentCellChanged; ;
        }

        void Start()
        {
            _hdrp = GameSettings.Get().RendererMode == RendererMode.HDRP;

            _transform = GetComponent<Transform>();
        }

        void Update()
        {
            if (!_enabled)
            {
                return;
            }

            if (_transform.position.y < underwaterLevel && !_isUnderwater)
                SetEffectEnabled(true);
            else if (_transform.position.y > underwaterLevel && _isUnderwater)
                SetEffectEnabled(false);
        }

        public void SetEffectEnabled(bool enabled, bool force = false)
        {
            if ((_isUnderwater == enabled && !force) && !_hdrp)
            {
                return;
            }

            _isUnderwater = enabled;

            if (enabled)
            {
                _defaultFog = RenderSettings.fog;
                _defaultFogColor = RenderSettings.fogColor;
                _defaultFogDensity = RenderSettings.fogDensity;
                _defaultSkybox = RenderSettings.skybox;
            }

            RenderSettings.fog = enabled;
            RenderSettings.fogColor = enabled ? fogColor : _defaultFogColor;
            RenderSettings.fogDensity = enabled ? fogDensity : _defaultFogDensity;
            RenderSettings.skybox = enabled ? null : _defaultSkybox;
        }

        private void Engine_CurrentCellChanged(CELLRecord cell)
        {
            _enabled = !cell.isInterior;
        }
    }
}