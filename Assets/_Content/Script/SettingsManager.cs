using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Gère l'interface graphique des options et la persistance des préférences utilisateur (ex: sensibilité de la caméra).
/// Les données sont enregistrées localement via PlayerPrefs.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Tooltip("Zone de texte affichant la sensibilité sous forme de pourcentage.")]
    public TMP_Text sensivityLabel;

    [Tooltip("Composant Scrollbar d'Unity UI permettant de modifier la sensibilité.")]
    public UnityEngine.UI.Scrollbar sensivityScrollbar;

    void Start()
    {
        // On récupère la sensibilité stockée en base (1.0 par défaut si aucune valeur n'est trouvée)
        float value = PlayerPrefs.GetFloat("Sensitivity", 1f);
        
        // Initialisation de la position de la Scrollbar de l'UI
        if (sensivityScrollbar != null)
            sensivityScrollbar.value = value;
            
        // Mise à jour visuelle du label de texte (ex: '100%')
        UpdateLabel(value);
    }

    /// <summary>
    /// Modifie, enregistre et applique la sensibilité de la caméra. 
    /// Doit être liée à l'événement OnValueChanged de la Scrollbar dans l'Inspecteur Unity.
    /// </summary>
    /// <param name="value">La valeur décimale de la Scrollbar (de 0 à 1).</param>
    public void setSensivity(float value)
    {
        UpdateLabel(value);
        Debug.Log("setSensivity" + value);
        
        // Sauvegarde de la sensibilité dans les options locales
        PlayerPrefs.SetFloat("Sensitivity", value);

        // Transformation de la valeur décimale en un multiplicateur entier (bridé entre 1 et 20)
        int sensitivity = Mathf.Clamp(Mathf.RoundToInt(value * 100f), 1, 20);

        // Si la caméra est active dans la scène en cours (utile en phase de jeu/test), on lui applique la valeur à la volée
        PlayerCamera cam = FindFirstObjectByType<PlayerCamera>();
        if (cam)
        {
            cam.SetSensitivity(sensitivity);
            Debug.Log("Caméra trouvé");
        }
    }

    // Met à jour la valeur textuelle du pourcentage de sensibilité dans l'UI
    private void UpdateLabel(float value)
    {
        int percentage = Mathf.RoundToInt(value * 100f);
        sensivityLabel.text = percentage + "%";
    }

    // Action de retour au menu principal (bouton Retour)
    public void loadMenus()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
