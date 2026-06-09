using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Gère le menu de pause en jeu.
/// Permet d'intercepter la touche 'P' du clavier pour stopper le temps de jeu, 
/// afficher l'écran de pause et libérer le curseur de la souris.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    [Tooltip("Référence vers l'objet Canvas contenant l'interface visuelle de pause.")]
    public GameObject pauseCanvas;
    
    // Suivi de l'état actuel de la pause
    bool paused;

    void Update()
    {
        // On surveille à chaque image si la touche P a été pressée
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            paused = !paused;
            
            // Active ou désactive le Canvas de pause
            pauseCanvas.SetActive(paused);
            
            // Bloque (0) ou rétablit (1) l'écoulement du temps physique d'Unity
            Time.timeScale = paused ? 0f : 1f;
            
            // On signale l'état au Singleton du joueur pour libérer/bloquer son contrôle de la caméra
            if (Player.Instance != null)
                Player.Instance.State.IsPaused = paused;
        }
    }

    // Méthode publique appelée par le bouton de reprise de l'UI
    public void resumeGame(){
        paused = !paused; 
        pauseCanvas.SetActive(paused);
        
        // On rétablit le temps normal
        Time.timeScale = paused ? 0f : 1f;
        
        if (Player.Instance != null)
            Player.Instance.State.IsPaused = paused;
    }

    // Redirige le joueur vers le menu principal
    public void accessMenu(){
        // IMPORTANT : Rétablir l'échelle de temps à 1 avant de charger la nouvelle scène,
        // sinon la scène chargée sera également figée.
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    // Quitte l'application
    public void quitGame(){
        Application.Quit();
    }
}