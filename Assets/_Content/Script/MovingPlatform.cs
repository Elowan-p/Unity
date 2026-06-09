using UnityEngine;

/// <summary>
/// Gère le déplacement automatique d'une plateforme le long d'un parcours prédéfini par des waypoints.
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [Tooltip("Liste ordonnée de points de passage (Transforms) que la plateforme doit suivre en boucle.")]
    public Transform[] waypoints;

    [Tooltip("Vitesse linéaire de déplacement de la plateforme.")]
    public float speed = 3f;
    
    // Index du waypoint actuellement ciblé
    private int currentWaypointIndex = 0;

    void Update()
    {
        // Sécurité : on ne fait rien si aucun point de passage n'est configuré
        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        // Sécurité : si un point de passage de la liste est manquant, on arrête le mouvement
        if (waypoints[currentWaypointIndex] == null)
        {
            return;
        }

        // Dès que la plateforme s'approche suffisamment du waypoint ciblé (marge d'erreur de 10 cm),
        // on passe au point de passage suivant.
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            currentWaypointIndex++;
            
            // Si on a atteint la fin de la boucle, on revient au tout premier waypoint
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }

        // Déplace la plateforme de sa position actuelle vers celle du waypoint ciblé à vitesse constante
        transform.position = Vector3.MoveTowards(
            transform.position, 
            waypoints[currentWaypointIndex].position, 
            speed * Time.deltaTime
        );
    }

    /// <summary>
    /// Note de conception : Dans ce projet 3D, cet événement physique 2D (OnCollisionExit2D) est obsolète
    /// et ne sera jamais déclenché. Le déplacement synchronisé du joueur sur la plateforme mobile est
    /// géré dynamiquement dans le script Player.cs (CheckGround) à l'aide d'un calcul de déplacement relatif.
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}