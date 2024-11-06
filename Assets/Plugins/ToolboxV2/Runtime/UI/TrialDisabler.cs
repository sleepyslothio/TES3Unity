using UnityEngine;
#if TRIAL_MODE
using UnityEngine.UI;
#endif

namespace Demonixis.DVRSimulator.UI
{
    public sealed class TrialDisabler : MonoBehaviour
    {
        private void Awake()
        {
#if TRIAL_MODE
            if (TryGetComponent(out Selectable selectable))
            {
                selectable.interactable = false;
            }
            else
            {
                gameObject.SetActive(false);
            }
#endif
        }
    }
}
