using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class Container : RecordComponent
    {
        void Start()
        {
            pickable = false;
            objData.name = ((CONTRecord)record).FNAM.value;
            objData.interactionPrefix = "Open ";
        }
    }
}