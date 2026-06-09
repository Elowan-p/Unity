using UnityEngine;

/// <summary>
/// Attribut personnalisé pour rendre un champ de l'Inspecteur Unity en lecture seule (non modifiable).
/// Très utile pour afficher des informations de débogage ou des états internes directement dans l'inspecteur.
/// </summary>
public class ReadOnlyAttribute : PropertyAttribute
{
}