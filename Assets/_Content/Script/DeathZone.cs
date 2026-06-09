using UnityEngine;

/// <summary>
/// Zone de mort (ex: vide, lave, etc.).
/// Lorsqu'un objet ayant le tag "Player" entre dans ce déclencheur, sa mort/réapparition est provoquée.
/// </summary>
public class DeathZone : MonoBehaviour
{
    // Appelée par le moteur physique d'Unity lorsqu'un autre Collider entre dans cette zone Trigger
    private void OnTriggerEnter(Collider other)
    {
        // On s'assure que c'est bien le joueur qui a touché la zone de mort
        if (other.CompareTag("Player"))
        {
            // On cherche le composant de réapparition attaché au joueur
            PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();

            // Si le script est trouvé, on lance la méthode pour le faire réapparaître au dernier checkpoint
            if (respawn != null)
            {
                respawn.Die();
            }
        }
    }
}