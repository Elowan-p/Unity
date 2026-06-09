using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Déclencheur générique.
/// Permet de lancer un événement UnityEvent configurable depuis l'Inspecteur Unity
/// (ex: ouvrir une porte, démarrer une musique, activer un effet) dès que le joueur pénètre dans le Trigger.
/// </summary>
public class PlayerEvent : MonoBehaviour
{
    [Tooltip("Événement ou liste d'actions à exécuter lorsque le joueur entre en collision avec ce trigger.")]
    [SerializeField] private UnityEvent _onPlayer;
    
    // Appelée par le moteur physique d'Unity lorsqu'un objet pénètre dans la zone de détection
    void OnTriggerEnter(Collider col)
    {
        // On s'assure que l'objet qui entre en collision est bien le GameObject de notre joueur
        if (Player.Instance && Player.Instance.gameObject == col.gameObject)
        {
            // Déclenche toutes les fonctions abonnées à cet événement
            _onPlayer?.Invoke();
        }
    }
}
