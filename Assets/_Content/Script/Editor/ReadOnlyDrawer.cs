#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

// Intercepte le rendu de l'inspecteur pour dessiner les variables marquées en lecture seule.
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    // Calcule la hauteur nécessaire pour afficher le champ (gère aussi les listes, structures et dépliants)
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    // Rendu graphique du champ dans l'inspecteur Unity
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // On garde en mémoire si l'UI d'Unity était active ou non avant notre dessin
        bool previous = GUI.enabled;

        // On force la désactivation de l'UI pour rendre ce champ grisé et non modifiable
        GUI.enabled = false;
        
        // On affiche le champ normalement avec sa valeur actuelle
        EditorGUI.PropertyField(position, property, label, true);
        
        // On rétablit l'état de l'UI pour ne pas perturber les champs qui suivent dans l'inspecteur
        GUI.enabled = previous;
    }
}

#endif