using UnityEngine;
using System.Collections;

// Gère un effet de tremblement (secousses) périodique sur le GameObject.
// Utile pour donner un indicateur visuel de danger sur des plateformes fragiles ou sur le point de s'effondrer.
public class TremblingEffect : MonoBehaviour
{
    [Tooltip("Intensité de la secousse (amplitude du décalage de position).")]
    [SerializeField] private float force = 0.05f;

    [Tooltip("Intervalle de temps (en secondes) entre le début de deux secousses.")]
    [SerializeField] private float delayBetweenShakes = 3f;

    [Tooltip("Durée active (en secondes) d'un tremblement.")]
    [SerializeField] private float shakeDuration = 0.2f;

    // Position d'origine locale pour pouvoir y réaligner l'objet après la secousse
    private Vector3 positionOriginale;

    void Start()
    {
        // On mémorise la position de base
        positionOriginale = transform.localPosition;
        
        // Démarrage de la coroutine de répétition infinie
        StartCoroutine(TrembleRepeatedly());
    }

    // Boucle infinie gérant le rythme des tremblements (attente -> tremble -> recommence)
    private IEnumerator TrembleRepeatedly()
    {
        while (true)
        {
            // On attend le temps défini avant la secousse suivante
            yield return new WaitForSeconds(delayBetweenShakes);
            
            // On lance la secousse et on attend qu'elle se termine avant de reprendre le décompte
            yield return StartCoroutine(Tremble());
        }
    }

    // Effectue les calculs de décalage aléatoire frame par frame pendant la durée du séisme
    private IEnumerator Tremble()
    {
        float elapsed = 0;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            // Calcul de décalages aléatoires compris entre [-force, force]
            float x = Random.Range(-force, force);
            float y = Random.Range(-force, force);

            // Application du décalage à l'objet (uniquement sur le plan local X/Y)
            transform.localPosition = positionOriginale + new Vector3(x, y, 0);

            // On rend la main au moteur de jeu jusqu'à la frame suivante
            yield return null;
        }

        // On repositionne précisément l'objet à ses coordonnées d'origine
        transform.localPosition = positionOriginale;
    }
}
