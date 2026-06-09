using UnityEngine;

/// <summary>
/// Gère la réapparition du joueur au point de départ ou au dernier checkpoint franchi.
/// Assure également une surveillance constante de la hauteur de chute du joueur.
/// </summary>
public class PlayerRespawn : MonoBehaviour
{
    [Tooltip("Point de réapparition actuel du joueur (ex: point de départ ou dernier checkpoint).")]
    public Transform respawnPoint;

    [Tooltip("Altitude minimale acceptable. En dessous de cette hauteur Y, le joueur est considéré comme tombé dans le vide.")]
    public float deathHeight = -20f;

    private CharacterController cc;
    
    // Suivi de l'index du dernier checkpoint franchi pour s'assurer que le joueur ne puisse pas réactiver un checkpoint précédent
    private int currentCheckpointIndex = 0;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Si le joueur passe sous le seuil minimal d'altitude, on déclenche sa mort/réapparition
        if (transform.position.y < deathHeight)
        {
            Die();
        }
    }

    // Déclenche le processus de mort
    public void Die()
    {
        Respawn();
    }

    /// <summary>
    /// Téléporte le joueur au point de réapparition actif.
    /// </summary>
    public void Respawn()
    {
        // Sécurité : si aucun point de réapparition n'est défini, on évite la téléportation
        if (respawnPoint == null)
        {
            return;
        }

        // NOTE TECHNIQUE : Dans Unity, pour téléporter un objet équipé d'un CharacterController,
        // il faut obligatoirement désactiver ce dernier le temps du déplacement du Transform,
        // sinon le moteur physique risque de rejeter ou d'ignorer la nouvelle position.
        if (cc != null)
            cc.enabled = false;

        transform.position = respawnPoint.position;

        if (cc != null)
            cc.enabled = true;
    }

    /// <summary>
    /// Permet aux zones de checkpoint de mettre à jour le point de réapparition.
    /// </summary>
    /// <param name="newRespawnPoint">Le transform du nouveau point de passage.</param>
    /// <param name="checkpointIndex">L'identifiant/ordre du checkpoint (doit être croissant).</param>
    public void SetRespawnPoint(Transform newRespawnPoint, int checkpointIndex)
    {
        // On n'autorise pas le joueur à réactiver un checkpoint précédent ou identique
        if (checkpointIndex <= currentCheckpointIndex)
        {
            return;
        }

        respawnPoint = newRespawnPoint;
        currentCheckpointIndex = checkpointIndex;
    }
}