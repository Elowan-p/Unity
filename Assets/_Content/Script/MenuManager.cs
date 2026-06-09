using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gère la navigation du menu principal.
/// Les méthodes publiques de cette classe sont conçues pour être assignées aux événements OnClick() des boutons de l'interface.
/// </summary>
public class MenuManager : MonoBehaviour
{
    // Charge la scène de jeu principale pour démarrer la partie
    public void PlayGame()
    {
        SceneManager.LoadScene("Level_1");
    }

    // Ouvre la scène des options / paramètres
    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    } 

    // Ferme complètement l'application
    public void QuitGame()
    {
        // Note : Application.Quit() est ignoré dans l'éditeur Unity, mais fonctionne dans la version compilée
        Application.Quit();
    }
}