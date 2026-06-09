using UnityEngine;

/// <summary>
/// Gère l'expansion physique de l'onde de choc d'une explosion.
/// Élargit rapidement un SphereCollider (Trigger) pour projeter ou affecter les objets aux alentours,
/// puis nettoie l'objet après quelques secondes.
/// </summary>
public class RocketExplosion : MonoBehaviour
{
    [System.Serializable]
    public class References
    {
        [Tooltip("Le SphereCollider de détection de zone utilisé pour propager l'effet physique de souffle.")]
        public SphereCollider Collider;
        [Tooltip("Le système de particules de l'explosion visuelle.")]
        public ParticleSystem Explosion;
    }

    [System.Serializable]
    public class Settings
    {
        [Tooltip("Le rayon d'impact maximum que le souffle physique peut atteindre.")]
        public float Radius = 5;
    }

    [SerializeField] private References _references;
    [SerializeField] private Settings _settings;

    // Rayon d'impact actuel de l'onde de choc en cours d'agrandissement
    private float radius;
    // Chronomètre pour la durée de vie du GameObject
    private float time;

    void Awake()
    {
        // Taille de départ de la détection physique au moment de l'impact
        radius = 2;
    }

    void Update()
    {
        // On agrandit la zone physique du souffle à chaque image (vitesse d'expansion de 15 m/s)
        radius += Time.deltaTime * 15;
        
        // Si on est en dessous de la taille max autorisée, on applique le rayon.
        // Dès qu'on dépasse la taille max, on met le rayon à 0 pour couper les effets physiques de l'onde de choc.
        _references.Collider.radius = radius <= _settings.Radius ? radius : 0;

        // Nettoyage automatique : on détruit l'objet après 5 secondes pour libérer de la mémoire
        // (ce délai laisse le temps à l'effet visuel des particules de se terminer proprement)
        time += Time.deltaTime;
        if (time > 5)
            Destroy(gameObject);
    }
}