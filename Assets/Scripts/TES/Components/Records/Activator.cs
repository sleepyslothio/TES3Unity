using TESUnity.ESM.Records;

namespace TESUnity.Components.Records
{
    public class Activator : RecordComponent
    {
        void Start()
        {
            usable = true;
            pickable = false;
            var ACTI = (ACTIRecord)record;
            objData.name = ACTI.Name;
            objData.interactionPrefix = "Use ";

            TryAddScript(ACTI.Script);
        }
    }
}