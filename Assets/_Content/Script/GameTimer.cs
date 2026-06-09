using UnityEngine;

/// <summary>
/// ScriptableObject gérant la logique du chronomètre de la partie.
/// L'utilisation d'un ScriptableObject permet de centraliser et de partager facilement
/// la donnée du temps entre l'interface utilisateur (UI) et les déclencheurs du jeu (fin de niveau)
/// sans coupler directement les classes.
/// </summary>
[CreateAssetMenu(fileName = "Timer", menuName = "Scriptable Objects/Timer")]
public class GameTimer : ScriptableObject
{
    // Temps écoulé en secondes (accessible en lecture publique, modification privée)
    public float ElapsedTime { get; private set; }
    
    // Flag déterminant si le chrono défile ou est en pause
    public bool IsRunning { get; private set; }

    // Réinitialisation automatique à l'activation ou au lancement du jeu
    private void OnEnable()
    {
        ElapsedTime = 0f;
        IsRunning = false;
    }

    // Remet le temps à zéro et active le décompte
    public void StartTimer()
    {
        ElapsedTime = 0f;
        IsRunning = true;
    }

    // Fait avancer le chronomètre. Doit être appelé régulièrement (depuis un Update())
    public void Tick(float deltaTime)
    {
        if (IsRunning)
            ElapsedTime += deltaTime;
    }

    // Stoppe la progression du temps (ex: fin de niveau ou pause)
    public void Stop()
    {
        IsRunning = false;
    }

    // Formate le temps écoulé en chaîne lisible : minutes:secondes:millisecondes (MM:SS:mmm)
    public override string ToString()
    {
        int minutes = Mathf.FloorToInt(ElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(ElapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((ElapsedTime % 1f) * 1000f);
        return $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }
}
