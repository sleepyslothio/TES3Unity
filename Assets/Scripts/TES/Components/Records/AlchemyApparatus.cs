using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class AlchemyApparatus : RecordComponent
    {
        void Start()
        {
            var APPA = (APPARecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = APPA.Name;
            objData.weight = APPA.Data.Weight.ToString();
            objData.value = APPA.Data.Value.ToString();
            objData.interactionPrefix = "Take ";

            TryAddScript(APPA.Script);
        }
    }
}