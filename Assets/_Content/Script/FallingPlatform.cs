using UnityEngine;

/// <summary>
/// Gère les plateformes qui s'effondrent sous les pas du joueur.
/// La plateforme tremble ou attend un délai après le contact, tombe, puis remonte à sa position initiale.
/// </summary>
public class FallingPlatform : MonoBehaviour
{
    [Tooltip("Délai d'attente (en secondes) entre le moment où le joueur touche la plateforme et sa chute effective.")]
    [Range(0f, 5f)]
    public float fallDelay = 2f;

    [Tooltip("Vitesse de descente de la plateforme lors de sa chute.")]
    public float fallSpeed = 10f;

    [Tooltip("Vitesse à laquelle la plateforme remonte à sa position de départ.")]
    public float riseSpeed = 25f;

    [Tooltip("Délai (en secondes) avant de commencer à remonter la plateforme après le début de sa chute.")]
    public float resetDelay = 7f;

    private bool isFalling = false;
    private bool isRising = false;
    private bool hasBeenTriggered = false;
    private Vector3 initialPosition;

    void Start()
    {
        // On sauvegarde la position d'origine de la plateforme pour la réinitialiser plus tard
        initialPosition = transform.position;
    }

    // Détecte le moment où le joueur marche sur la plateforme (via un trigger)
    void OnTriggerEnter(Collider collision)
    {
        // Si c'est le joueur et que le cycle de chute n'est pas déjà lancé
        if (collision.gameObject.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            // On planifie le début de la chute après le délai imparti
            Invoke(nameof(StartFalling), fallDelay);
        }
    }

    // Déclenche l'état de chute
    void StartFalling()
    {
        isFalling = true;
        // On planifie la remontée après le délai de réinitialisation
        Invoke(nameof(StartRising), resetDelay);
    }

    // Désactive la chute et commence la phase de remontée
    void StartRising()
    {
        isFalling = false;
        isRising = true;
    }

    void Update()
    {
        // Gestion de la chute vers le bas
        if (isFalling)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
        // Gestion du retour progressif à la position initiale
        else if (isRising)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, riseSpeed * Time.deltaTime);
            
            // Une fois revenue à son point de départ, la plateforme redevient active et prête à retomber
            if (transform.position == initialPosition)
            {
                isRising = false;
                hasBeenTriggered = false;
            }
        }
    }
}
