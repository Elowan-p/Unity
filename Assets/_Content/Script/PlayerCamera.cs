using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gère le comportement de la caméra à la troisième personne.
/// Suit le joueur de manière fluide tout en lui permettant de tourner la caméra avec les mouvements de souris/joystick.
/// Le script s'exécute également en mode édition [ExecuteInEditMode] pour permettre de prévisualiser la caméra dans l'éditeur.
/// </summary>
[ExecuteInEditMode]
public class PlayerCamera : MonoBehaviour
{
    /// <summary>
    /// Réglages de sensibilité et de positionnement de la caméra.
    /// </summary>
    [System.Serializable]
    private class Settings
    {
        [Header("Sensivity")]

        [Tooltip("Lissage du mouvement de suivi (0 = instantané, 1 = très amorti/retardé).")]
        [Range(0, 1)]
        public float FollowSmoothness = .1f;

        [Tooltip("Sensibilité globale de rotation de la caméra.")]
        public float LookSensitivity = 20;

        [Header("Position")]

        [Tooltip("Distance de recul de la caméra par rapport au joueur.")]
        public float Distance = 5;

        [Tooltip("Hauteur relative de la caméra par rapport aux pieds du joueur.")]
        public float VerticalOffset = 2;

        [Header("Pitch")]

        [Tooltip("Angle vertical initial de la caméra (inclinaison par défaut).")]
        public float DefaultPitch = 20;

        [Tooltip("Angle d'inclinaison vertical minimal (limite haute pour regarder le ciel).")]
        public float MinPitch = -30;

        [Tooltip("Angle d'inclinaison vertical maximal (limite basse pour regarder le sol).")]
        public float MaxPitch = 60;
    }

    /// <summary>
    /// Liens vers les objets requis.
    /// </summary>
    [System.Serializable]
    public class References
    {
        [Tooltip("Asset d'Input System contenant les actions de visée.")]
        public InputActionAsset InputActions;
        [Tooltip("La cible que la caméra doit suivre (le joueur).")]
        public Transform Target;
    }

    [SerializeField]
    private Settings _settings;

    [SerializeField]
    private References _references;

    // Variables d'angles de rotation
    private float _yaw;   // Rotation horizontale (axe Y)
    private float _pitch; // Rotation verticale (axe X)

    private Vector3 _playerPosition;
    private InputAction _lookAction;


    private void Awake()
    {
        // Liaison avec l'action de visée du Input System
        _lookAction = _references.InputActions.FindActionMap("Player").FindAction("Look");

        // Charge la sensibilité enregistrée par PlayerPrefs s'il y en a une (ex: configurée dans les options)
        if (PlayerPrefs.HasKey("Sensitivity"))
            _settings.LookSensitivity = Mathf.Clamp(Mathf.RoundToInt(PlayerPrefs.GetFloat("Sensitivity") * 20f), 1, 20);
    }

    // Permet de modifier la sensibilité de visée (ex: via le menu des paramètres)
    public void SetSensitivity(float value) => _settings.LookSensitivity = value;

    void OnEnable()
    {
        _lookAction?.Enable();
        
        // Initialisation de la position pour éviter des sursauts brusques de caméra au spawn
        if (_references.Target != null)
            _playerPosition = _references.Target.position;
            
        _pitch = _settings.DefaultPitch;
    }

    void OnDisable()
    {
        _lookAction?.Disable();
    }

    // On utilise LateUpdate pour s'assurer que la caméra calcule sa position APRÈS
    // que le joueur a fini de se déplacer (dans Update/FixedUpdate).
    void LateUpdate()
    {
        if (_references.Target == null)
            return;

        float t = Time.deltaTime;

        SetCursor();
        SetYawAndPitch(t);
        SetPosition(t);
    }

    /// <summary>
    /// Gère le verrouillage du curseur de la souris (caché au centre de l'écran en jeu, libéré en pause).
    /// </summary>
    private void SetCursor()
    {
        if(!Application.isPlaying) 
            return;

        // Si le joueur est en pause, on libère le curseur pour lui permettre de cliquer sur l'UI
        bool lockCursor = (Player.Instance != null) ? !Player.Instance.State.IsPaused : true;
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
    }

    /// <summary>
    /// Calcule les rotations Yaw (horizontale) et Pitch (verticale) d'après les mouvements souris/joystick.
    /// </summary>
    private void SetYawAndPitch(float deltaTime)
    {
        Vector2 lookInput = _lookAction?.ReadValue<Vector2>() ?? Vector2.zero;

        // Pitch : inclinaison haut/bas avec contrainte de limites
        _pitch -= lookInput.y * _settings.LookSensitivity * deltaTime;
        _pitch = Mathf.Clamp(_pitch, _settings.MinPitch, _settings.MaxPitch);
        
        // Yaw : rotation gauche/droite (sans contrainte d'angles)
        _yaw  += lookInput.x * _settings.LookSensitivity * deltaTime;
    }

    /// <summary>
    /// Positionne et oriente physiquement la caméra par rapport au joueur.
    /// </summary>
    private void SetPosition(float deltaTime)
    {
        // Interpolation de la position de suivi pour obtenir un effet de lissage
        float t = (1.1f - _settings.FollowSmoothness) * 20 * deltaTime;
        _playerPosition = Vector3.Lerp(_playerPosition, _references.Target.position, t);

        // Calcul de la position de recul de base
        Vector3 camPos = Vector3.back * _settings.Distance;
        
        // Application de la rotation sphérique sur le vecteur de recul
        camPos = Quaternion.Euler(_pitch,  _yaw, 0) * camPos;
        camPos += _playerPosition;

        // Orientation de la caméra pour regarder la cible
        Quaternion camRot = Quaternion.LookRotation(_playerPosition - camPos);

        // Application de l'ajustement de hauteur
        camPos.y += _settings.VerticalOffset;

        transform.position = camPos;
        transform.rotation = camRot;
    }

    // Méthode de rappel pour l'UI ou des scripts externes
    public void setSensivity(float value){
        _settings.LookSensitivity = value;
        Debug.Log("senesivity changed");
    }
}