using System.Collections;
using UnityEngine;

namespace Demonixis.ToolboxV2.Utils
{
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class LifeTimeParticle : MonoBehaviour
    {
        [SerializeField]
        private bool _disable = false;

        private IEnumerator Start()
        {
            var particle = GetComponent<ParticleSystem>();

            yield return CoroutineFactory.WaitForSeconds(particle.main.duration);

            if (_disable)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
