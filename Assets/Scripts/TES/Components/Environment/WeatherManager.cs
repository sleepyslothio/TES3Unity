using System.Collections;
using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components
{
    public class WeatherManager : MonoBehaviour
    {
        private Color m_DefaultFogColor;
        private float m_DefaultFogDensity;

        private IEnumerator Start()
        {
            m_DefaultFogColor = RenderSettings.fogColor;
            m_DefaultFogDensity = RenderSettings.fogDensity;

            yield return new WaitForEndOfFrame();

            var tes = TES3Manager.Instance;
            tes.Engine.CurrentCellChanged += Engine_CellLoaded;
            Engine_CellLoaded(tes.Engine.CurrentCell);
        }

        private void Engine_CellLoaded(CELLRecord cell)
        {
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
