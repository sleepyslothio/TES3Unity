using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
{
    public class Container : RecordComponent
    {
        void Start()
        {
            var CONT = (CONTRecord)record;
            pickable = false;
            objData.name = CONT.Name;
            objData.interactionPrefix = "Open ";
        }
    }
}