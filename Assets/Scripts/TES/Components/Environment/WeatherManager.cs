using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components
{
    public class WeatherManager : MonoBehaviour
    {
        private void Awake()
        {
            var tes = TES3Manager.instance;
            tes.Engine.CurrentCellChanged += Engine_CellLoaded;
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
            }
            else
            {
                RenderSettings.fog = false;
            }
        }
    }
}
