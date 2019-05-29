using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TESUnity.Inputs
{
    public interface IInputProvider
    {
        bool TryInitialize();
        float GetAxis(MWAxis axis);
        bool Get(MWButton button);
        bool GetDown(MWButton button);
        bool GetUp(MWButton button);
    }
}
