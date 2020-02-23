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
