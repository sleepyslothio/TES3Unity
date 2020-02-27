using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class Container : RecordComponent
    {
        void Start()
        {
            var CONT = (CONTRecord)record;
            pickable = false;
            objData.name = CONT.FNAM.value;
            objData.interactionPrefix = "Open ";
        }
    }
}