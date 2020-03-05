using TESUnity.ESM;
using TESUnity.ESM.Records;

namespace TESUnity.Components.Records
{
    public class TESCloth : RecordComponent
    {
        void Start()
        {
            var CLOT = (CLOTRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = CLOT.Name;
            objData.weight = CLOT.Data.Weight.ToString();
            objData.value = CLOT.Data.Value.ToString();
            objData.interactionPrefix = "Take ";

            TryAddScript(CLOT.Script);
        }
    }
}