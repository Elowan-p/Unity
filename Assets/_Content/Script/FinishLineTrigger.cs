using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Gère le passage de la ligne d'arrivée par le joueur.
/// Stoppe le chronomètre, affiche un texte de victoire et charge la scène suivante après un délai.
/// </summary>
public class FinishLineTrigger : MonoBehaviour
{
    [Tooltip("Référence vers le chronomètre global pour pouvoir l'arrêter.")]
    [SerializeField] private GameTimer gameTimer;

    [Tooltip("Élément de l'interface utilisateur affichant la victoire (ex: écran de fin).")]
    [SerializeField] private GameObject winText;

    [Tooltip("Nom de la scène à charger après avoir terminé le niveau.")]
    [SerializeField] private string nextSceneName = "Player";

    [Tooltip("Délai (en secondes) pendant lequel l'écran de victoire est visible avant la transition.")]
    [SerializeField] private float displayDuration = 5f;

    [Tooltip("Tag d'identification du joueur pour le déclencheur.")]
    [SerializeField] private string playerTag = "Player";

    // Sécurité pour éviter de déclencher la victoire plusieurs fois d'affilée
    private bool triggered;

    // Détection de collision
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger activé");
        if (triggered) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;
        win();
    }

    /// <summary>
    /// Fonction principale exécutant la logique de fin de partie/niveau.
    /// </summary>
    public void win()
    {
        // On stoppe le chronomètre
        gameTimer?.Stop();
        
        // On affiche l'écran de victoire
        if (winText == null)
        {
            Debug.LogWarning("winText non assigné dans l'inspecteur !");
        }
        else
        {
            Debug.Log($"Activation winText: {winText.name}");
            winText.SetActive(true);
        }

        // On lance le chargement différé de la scène suivante
        StartCoroutine(LoadNextSceneAfterDelay());
    }

    // Coroutine gérant la transition vers le niveau suivant après un court délai
    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}
