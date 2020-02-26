using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class AlchemyApparatus : RecordComponent
    {
        void Start()
        {
            var APPA = (APPARecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = APPA.FNAM.value;
            objData.weight = APPA.AADT.weight.ToString();
            objData.value = APPA.AADT.value.ToString();
            objData.interactionPrefix = "Take ";
        }
    }
}