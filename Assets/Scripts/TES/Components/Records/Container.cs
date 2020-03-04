using TESUnity.ESM.Records;

namespace TESUnity.Components.Records
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