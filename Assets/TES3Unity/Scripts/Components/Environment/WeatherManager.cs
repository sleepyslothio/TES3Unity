using System;
using TES3Unity.ESM.Records;
using UnityEngine;
using UnityEngine.Rendering;

namespace TES3Unity.Components
{
    public class WeatherManager : MonoBehaviour
    {
        private const float TweetyFourHoursInSeconds = 86400.0f;
        private const float TwelveHoursInSeconds = 43200.0f;

        private Transform _sunTransform;
        private Color _defaultFogColor;
        private Color _defaultSunLightColor;
        private readonly Color32 _defaultAmbientColor = new (137, 140, 160, 255);
        private float _defaultFogDensity;
        private bool _dayNightCycle;

        [SerializeField] private Light _sunLight;
        [SerializeField] private Light _moonLight;

        [SerializeField, Range(0.0f, TweetyFourHoursInSeconds)]
        private float _timeInSeconds = 34000;

        [SerializeField] private float _timeScale = 1.0f;

        public TimeSpan GameTime
        {
            get => TimeSpan.FromSeconds(_timeInSeconds);
            set => _timeInSeconds = (float)value.TotalSeconds;
        }

        public int Hours => GameTime.Hours;
        public int Minutes => GameTime.Minutes;
        public int Seconds => GameTime.Seconds;
        public int TotalSeconds => GameTime.Seconds;

        public float TimeScaleMultiplier { get; set; } = 1.0f;

        private void Start()
        {
            _defaultFogColor = RenderSettings.fogColor;
            _defaultFogDensity = RenderSettings.fogDensity;
            _defaultSunLightColor = _sunLight.color;

            var config = GameSettings.Get();
            _dayNightCycle = config.DayNightCycle;

            var engine = Tes3Engine.Instance;
            engine.CurrentCellChanged += OnCurrentCellChanged;
            OnCurrentCellChanged(engine.CurrentCell);
        }
        
        private void Update()
        {
            _timeInSeconds += Time.deltaTime * _timeScale * TimeScaleMultiplier;

            if (_timeInSeconds >= TweetyFourHoursInSeconds)
                _timeInSeconds = 0.0f;

            if (_sunLight == null || !_dayNightCycle) return;

            var position = Quaternion.Euler((_timeInSeconds / TweetyFourHoursInSeconds) * 360.0f, 0, 0) *
                           new Vector3(0.0f, -300.0f, 0.0f);
            _sunTransform.position = position;
            _sunTransform.rotation = Quaternion.LookRotation(-position);

            _sunLight.intensity = 1.25f - Mathf.Abs(_timeInSeconds / TwelveHoursInSeconds - 1.0f);
            _sunLight.color = new Color(
                1.0f,
                Mathf.Min(_sunLight.intensity + 0.05f, 1.0f),
                Mathf.Min(_sunLight.intensity, 1.0f)
            );
        }

        private void OnCurrentCellChanged(CELLRecord cell)
        {
            if (cell == null) return;

            var ambientColor = _defaultAmbientColor;
            var sunColor = _defaultSunLightColor;
            var ambientData = cell.AMBI;
            var fogColor = _defaultFogColor;
            var fogDensity = _defaultFogDensity;
            var fog = !cell.isInterior;
            var rain = false;

            if (ambientData != null)
            {
                ambientColor = ColorUtils.B8G8R8ToColor32(ambientData.ambientColor);
                sunColor = ColorUtils.B8G8R8ToColor32(ambientData.sunlightColor);
                fogColor = ColorUtils.B8G8R8ToColor32(ambientData.fogColor);
                fogDensity = ambientData.fogDensity;
            }

            if (!cell.isInterior)
            {
                var regions = Tes3Engine.DataReader.MorrowindESMFile.GetRecords<REGNRecord>();

                foreach (var region in regions)
                {
                    if (region.Id == cell.RGNN.value)
                    {
                        fog = region.Data.Foggy > 0;
                        rain = region.Data.Rain > 0;
                        break;
                    }
                }
            }

            RenderSettings.fog = fog;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.ambientLight = ambientColor;
            RenderSettings.ambientMode = cell.isInterior ? AmbientMode.Flat : AmbientMode.Skybox;
            RenderSettings.ambientIntensity = Tes3Engine.Instance.ambientIntensity;

            if (rain)
            {
                Debug.Log("It's raining!");
            }

            if (_sunLight == null) return;
            
            _sunLight.enabled = !cell.isInterior;
            _sunLight.color = sunColor;
        }
    }
}