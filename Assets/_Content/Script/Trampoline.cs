using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Trampoline : MonoBehaviour
{
    [Tooltip("Bounce force in m/s")]
    [SerializeField] private float _bounceForce = 15f;

    [Tooltip("Optional VFX played on bounce")]
    [SerializeField] private ParticleSystem _bounceVFX;

    [Tooltip("Optional SFX played on bounce")]
    [SerializeField] private AudioSource _bounceSFX;

    private void OnTriggerEnter(Collider other)
    {
        TryBounce(other);
    }

    private void TryBounce(Collider other)
    {
        if (!other.TryGetComponent(out Player player))
            return;

        player.Bounce(_bounceForce);

        if (_bounceVFX)
            _bounceVFX.Play();

        if (_bounceSFX)
            _bounceSFX.Play();
    }
}
