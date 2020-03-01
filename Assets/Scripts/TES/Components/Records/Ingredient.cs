using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class Ingredient : RecordComponent
    {
        void Start()
        {
            var INGR = (INGRRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(INGR.ITEX.value, "icons"); 
            objData.name = INGR.Name;
            objData.weight = INGR.Data.Weight.ToString();
            objData.value = INGR.Data.Value.ToString();
            objData.interactionPrefix = "Take ";

            TryAddScript(INGR.Script);
        }
    }
}
