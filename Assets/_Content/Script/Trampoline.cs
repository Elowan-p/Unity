using UnityEngine;

/// <summary>
/// Gère le comportement d'un trampoline ou d'un bumper.
/// Détecte l'entrée du joueur dans son trigger physique, le propulse verticalement 
/// et déclenche des effets visuels (VFX) et sonores (SFX).
/// </summary>
[RequireComponent(typeof(Collider))]
public class Trampoline : MonoBehaviour
{
    [Tooltip("Intensité de la propulsion verticale (en m/s) imprimée au joueur.")]
    [SerializeField] private float _bounceForce = 15f;

    [Tooltip("Effet de particules optionnel à jouer lors du rebond.")]
    [SerializeField] private ParticleSystem _bounceVFX;

    [Tooltip("Source sonore optionnelle à déclencher lors du rebond.")]
    [SerializeField] private AudioSource _bounceSFX;

    // Détection physique
    private void OnTriggerEnter(Collider other)
    {
        TryBounce(other);
    }

    /// <summary>
    /// Tente d'appliquer le rebond à l'objet qui a touché le trampoline.
    /// </summary>
    private void TryBounce(Collider other)
    {
        // On vérifie si l'objet qui entre possède bien le script de contrôle du joueur
        if (!other.TryGetComponent(out Player player))
            return;

        // On applique l'impulsion physique verticale au joueur
        player.Bounce(_bounceForce);

        // Lecture des effets visuels s'ils ont été configurés
        if (_bounceVFX)
            _bounceVFX.Play();

        // Lecture de l'effet sonore s'il a été configuré
        if (_bounceSFX)
            _bounceSFX.Play();
    }
}
