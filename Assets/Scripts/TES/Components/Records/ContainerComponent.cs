using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class ContainerComponent : RecordComponent
    {
        void Start()
        {
            pickable = false;
            objData.name = ((CONTRecord)record).FNAM.value;
            objData.interactionPrefix = "Open ";
        }
    }
}