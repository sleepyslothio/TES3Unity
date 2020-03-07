using System.Collections;
using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components
{
    public class WeatherManager : MonoBehaviour
    {
        private IEnumerator Start()
        {
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
            }
            else
            {
                RenderSettings.fog = false;
            }
        }
    }
}
