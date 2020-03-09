using System.Collections;
using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components
{
    public class WeatherManager : MonoBehaviour
    {
        private Color m_DefaultFogColor;
        private float m_DefaultFogDensity;

        private void Start()
        {
            m_DefaultFogColor = RenderSettings.fogColor;
            m_DefaultFogDensity = RenderSettings.fogDensity;

            var engine = TES3Engine.Instance;
            engine.CurrentCellChanged += OnCurrentCellChanged;
            OnCurrentCellChanged(engine.CurrentCell);
        }

        private void OnCurrentCellChanged(CELLRecord cell)
        {
            if (cell == null)
            {
                return;
            }

            RenderSettings.fog = !cell.isInterior;
            
            if (!cell.isInterior)
            {
                var regions = TES3Manager.MWDataReader.MorrowindESMFile.GetRecords<REGNRecord>();

                foreach (var region in regions)
                {
                    if (region.Id == cell.RGNN.value)
                    {
                        RenderSettings.fog = region.Data.Foggy > 0;
                        // Rain
                        break;
                    }
                }

                var ambientData = cell.AMBI;
                var fogColor = m_DefaultFogColor;
                var fogDensity = m_DefaultFogDensity;

                if (ambientData != null)
                {
                    fogColor = ColorUtils.B8G8R8ToColor32(ambientData.fogColor);
                    fogDensity = ambientData.fogDensity; Debug.Log(fogDensity);
                }

                RenderSettings.fogColor = fogColor;
                RenderSettings.fogDensity = fogDensity;
            }
            else
            {
                RenderSettings.fog = false;
            }
        }
    }
}
