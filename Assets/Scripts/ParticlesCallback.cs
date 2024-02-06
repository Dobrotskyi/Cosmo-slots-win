using UnityEngine;
using UnityEngine.Events;

public class ParticlesCallback : MonoBehaviour
{
    public UnityEvent Stoped;
    private void OnParticleSystemStopped()
    {
        Stoped?.Invoke();
    }

    private void OnDestroy()
    {
        Stoped?.RemoveAllListeners();
    }
}
