using TESUnity.ESM;
using UnityEngine;

namespace TESUnity.Components.Records
{
    public class CreatureComponent : RecordComponent
    {
        protected CREARecord _creature;

        void Start()
        {
            _creature = (CREARecord)record;

            transform.rotation = Quaternion.Euler(-70, 0, 0);
        }
    }
}
