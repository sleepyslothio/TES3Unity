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
        bool GetButton(MWButton button);
        bool GetButtonDown(MWButton button);
        bool GetButtonUp(MWButton button);
    }
}
