using UnityEngine;
using TMPro;

// Contrôle l'interface utilisateur affichant le chronomètre en jeu.
// Ce script fait le lien entre la logique du chronomètre (GameTimer) et l'affichage TextMeshPro.
public class TimerUI : MonoBehaviour
{
    [Tooltip("Zone de texte TextMeshPro pour l'affichage du temps.")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Tooltip("Référence vers le ScriptableObject contenant les données du chronomètre de jeu.")]
    [SerializeField] private GameTimer gameTimer;

    private void Start()
    {
        // Dès le lancement de la scène, on réinitialise et démarre le chrono de la partie
        gameTimer?.StartTimer();
    }

    private void Update()
    {
        // Sécurité au cas où le ScriptableObject ne serait pas renseigné dans l'Inspecteur
        if (gameTimer == null) return;

        // On met à jour le temps accumulé dans le ScriptableObject
        gameTimer.Tick(Time.deltaTime);

        // Si le composant texte existe, on y injecte la chaîne de caractères formatée du chronomètre
        if (timerText != null)
            timerText.text = gameTimer.ToString();
    }
}
