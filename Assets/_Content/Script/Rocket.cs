using UnityEngine;

/// <summary>
/// Gère le comportement d'une roquette physique (projectile).
/// Elle applique une force constante vers l'avant (transform.up dans son cas),
/// puis instancie un prefab d'explosion et se détruit lors de l'impact physique.
/// </summary>
public class Rocket : MonoBehaviour
{
    /// <summary>
    /// Regroupe les dépendances et références de composants de la roquette.
    /// </summary>
    [System.Serializable]
    public class References
    {
        [Tooltip("Rigidbody de la roquette utilisé pour appliquer les forces physiques de poussée.")]
        public Rigidbody Rigidbody;
        [Tooltip("Effet de particules de feu/fumée simulant le moteur de la roquette.")]
        public ParticleSystem Fire;
        [Tooltip("Prefab de l'explosion qui sera généré à l'impact.")]
        public GameObject Explosion;
    }

    /// <summary>
    /// Réglages généraux du comportement de la roquette.
    /// </summary>
    [System.Serializable]
    public class Settings
    {
        [Tooltip("Si coché, la roquette est propulsée automatiquement dès son apparition en jeu.")]
        public bool LaunchOnEnable = false;
        [Tooltip("Vitesse de poussée du réacteur.")]
        public float Speed = 20;
        [Tooltip("Temps de poussée maximum (en secondes) avant l'extinction du moteur.")]
        public float Duration = 5;
    }
    
    /// <summary>
    /// État dynamique de la roquette en cours de vol (affichage en lecture seule).
    /// </summary>
    [System.Serializable]
    public class State
    {
        [Tooltip("Indique si le projectile a été lancé.")]
        public bool Launched = false;
        [Tooltip("Indique si le moteur est actuellement éteint (carburant épuisé).")]
        public bool Disabled = false;
        [Tooltip("Temps de vol cumulé depuis le décollage.")]
        public float FlightTime = 0;
    }

    [SerializeField] private References _references;
    [SerializeField] private Settings _settings;
    [SerializeField, ReadOnly] private State _state;
    
    void OnEnable()
    {
        // Lancement automatique si configuré dans l'Inspecteur
        if (_settings.LaunchOnEnable)
        {
            Launch();
        }
    }

    void Update()
    {
        // Si la roquette est lancée et qu'elle est toujours dans sa durée de poussée active
        if (_state.Launched && _state.FlightTime < _settings.Duration)
        {
            // On calcule le vecteur de poussée (vers le haut local 'transform.up' correspondant à l'alignement du modèle)
            Vector3 force = transform.up * _settings.Speed * Time.deltaTime * 100;

            // Application de la force physique continue
            _references.Rigidbody.AddForce(force);

            _state.FlightTime += Time.deltaTime;
        }
        // Si le temps de poussée est écoulé et que la roquette n'est pas encore désactivée
        else if (!_state.Disabled)
        {
            _state.Disabled = true;

            // Extinction de l'effet visuel de réacteur
            _references.Fire.Stop();
        }
    }

    // Gère l'impact physique du projectile
    void OnCollisionEnter(Collision col)
    {
        // Sécurité : on n'autorise l'explosion que si la roquette vole depuis plus de 0,5 seconde
        // afin d'éviter qu'elle n'explose instantanément sur l'objet qui l'a fait apparaître (le lanceur)
        if (_state.Launched && _state.FlightTime > .5f)
        {
            // Instanciation de l'explosion
            GameObject explosion = Instantiate(_references.Explosion);
            explosion.transform.position = transform.position;

            // Destruction du projectile
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Active la propulsion et allume le réacteur de la roquette.
    /// Peut être appelée via l'éditeur grâce au clic droit -> Launch sur le script.
    /// </summary>
    [ContextMenu("Launch")]
    public void Launch()
    {
        _state.Launched = true;
        _state.Disabled = false;
        _state.FlightTime = 0;

        _references.Fire.Play();
    }
}