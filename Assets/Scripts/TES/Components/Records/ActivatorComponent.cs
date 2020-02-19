using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class ActivatorComponent : RecordComponent
    {
        void Start()
        {
            usable = true;
            pickable = false;
            var ACTI = (ACTIRecord)record; 
            objData.name = ACTI.FNAM.value;
            objData.interactionPrefix = "Use ";
        }
    }
}