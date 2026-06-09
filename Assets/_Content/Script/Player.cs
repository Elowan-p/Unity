using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.CullingGroup;

/// <summary>
/// Contrôleur principal du joueur. Gère la physique, les mouvements 3D, les sauts, 
/// et la synchronisation avec le déplacement des plateformes mobiles.
/// </summary>
public class Player : MonoBehaviour
{
    // Accès statique global (Singleton)
    public static Player Instance { get; private set; }

    /// <summary>
    /// Représente les différents états d'animation et de jeu du joueur.
    /// </summary>
    public enum PlayerState
    {
        Idle,
        Moving,
        Jumping,
        Falling,
        Stunned,
        Eliminated,
        Loser,
        Winner,
    }

    /// <summary>
    /// Paramètres généraux de configuration du joueur (modifiables dans l'éditeur).
    /// </summary>
    [System.Serializable]
    public class Settings
    {
        [Header("Movements")]

        [Tooltip("Vitesse maximale de déplacement du joueur en km/h.")]
        public float Speed = 18f;

        [Tooltip("Force d'impulsion verticale du saut en m/s.")]
        public float JumpForce = 8f;

        [Tooltip("Vitesse de rotation du joueur sur lui-même pour s'aligner avec sa direction de marche.")]
        public float RotationSpeed = 10f;

        [Tooltip("Distance tolérée pour la détection du sol sous le joueur.")]
        public float GroundTolerance = 0.2f;

        [Tooltip("Masque de collision identifiant ce qui est considéré comme du sol stable.")]
        public LayerMask GroundLayer = 1;

        [Tooltip("Masque de collision identifiant ce qui élimine instantanément le joueur.")]
        public LayerMask DeathLayer = 0;

        [Header("Forces")]

        [Tooltip("Vitesse d'atténuation (drag) des forces externes appliquées au joueur (m/s²).")]
        public float ExtraForcesDrag = 8f;

        [Header("Debug")]

        [Tooltip("Activer les logs pour observer les transitions d'états dans la console Unity.")]
        public bool StateLogs;
    }

    /// <summary>
    /// Références aux composants externes nécessaires.
    /// </summary>
    [System.Serializable]
    public class References
    {
        [Tooltip("Le composant CharacterController gérant les collisions et le déplacement physique du joueur.")]
        public CharacterController Controller;
        [Tooltip("La configuration des touches et axes d'entrées clavier/manette.")]
        public InputActionAsset InputActions;
    }

    /// <summary>
    /// Conteneur regroupant l'état dynamique en temps réel du joueur.
    /// </summary>
    [System.Serializable]
    public class StateContainer
    {
        [Tooltip("L'état actuel du joueur (Idle, Moving, Falling, etc.).")]
        public PlayerState CurrentState = PlayerState.Idle;

        [Tooltip("Est-ce que le jeu est en pause (bloquant certains mouvements et caméras) ?")]
        public bool IsPaused = false;
        
        [Tooltip("La vitesse actuelle calculée en m/s (axes X, Y, Z).")]
        public Vector3 Velocity;

        [Tooltip("Le Transform du sol actuellement touché (sert de référence pour les plateformes mobiles).")]
        public Transform Ground;
        
        public bool IsGrounded => Ground;
        public float VerticalVelocity => Velocity.y;
        public Vector3 HorizontalVelocity => new Vector3(Velocity.x, 0, Velocity.z);
    }

    [SerializeField] private Settings _settings;
    [SerializeField] private References _references;
    [SerializeField, ReadOnly] private StateContainer _state;

    // Propriété d'accès en lecture seule à l'état du joueur
    public StateContainer State => _state;

    #region Constants
    private const float KMH_TO_MS = 1 / 3.6f; // Facteur de conversion de km/h vers m/s
    private const float STICK_FORCE = -5f;     // Force plaquant le joueur au sol pour éviter de glisser dans les descentes
    private const float GRAVITY = -20f;        // Valeur de la gravité appliquée en l'air
    private const float MAX_GRAVITY = -50f;    // Vitesse de chute maximale autorisée (vitesse terminale)
    #endregion

    #region Private Fields
    // Touches d'action
    private InputAction _moveAction;
    private InputAction _jumpAction;

    // Caméra principale pour orienter les déplacements par rapport à la vue
    private Camera _camera;

    // Variables de géométrie de détection de sol
    private Vector3 _groundCheckRayOffset;
    private Vector3 _groundCheckSphereOffset;
    private float _groundCheckRadius;
    private Collider[] _overlapResults = new Collider[1];
    
    // Variables de déplacement de plateformes mobiles
    private Vector3 _lastPlatformPosition;
    private Quaternion _lastPlatformRotation;
    private Vector3 _platformVelocity;
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        // Initialisation de l'instance Singleton
        if (!Instance)
            Instance = this;

        // Liaison des actions d'entrées utilisateur
        _moveAction = _references.InputActions.FindActionMap("Player").FindAction("Move");
        _jumpAction = _references.InputActions.FindActionMap("Player").FindAction("Jump");

        _camera = Camera.main;

        // Précalcul des formes de détection de sol par rapport au CharacterController
        CharacterController cc = _references.Controller;
        // Point de départ du rayon central de contact
        _groundCheckRayOffset = cc.center + Vector3.up * (-cc.height * .5f - cc.skinWidth + _settings.GroundTolerance);
        // Centre de la sphère de contact de sécurité (bords du CharacterController)
        _groundCheckSphereOffset = cc.center + Vector3.up * (-cc.height * .5f + cc.radius - cc.skinWidth - _settings.GroundTolerance);
        _groundCheckRadius = cc.radius;
    }

    void OnEnable()
    {
        _moveAction?.Enable();
        _jumpAction?.Enable();
    }

    void OnDisable()
    {
        _moveAction?.Disable();
        _jumpAction?.Disable();
    }

    void Update()
    {
        float t = Time.deltaTime;

        // Séquence de mise à jour du joueur à chaque frame
        CheckGround(t);  // 1. Détection du sol et adaptation aux plateformes mobiles
        SetGravity(t);   // 2. Calcul et application de la gravité
        SetVelocity(t);  // 3. Traitement des entrées clavier/joystick et orientation
        SetJump();       // 4. Détection du saut
        SetMovement(t);  // 5. Exécution du mouvement final via le CharacterController
        SetState();      // 6. Mise à jour de l'état logique pour la machine à états d'animation
    }
    #endregion

    #region Player Logic
    /// <summary>
    /// Analyse la surface sous le joueur. Si le joueur marche sur une plateforme mobile,
    /// on applique le différentiel de position/rotation de la plateforme pour qu'il la suive naturellement.
    /// </summary>
    private void CheckGround(float deltaTime)
    {
        // 1. Détection centrale par lancer de rayon (Raycast)
        Vector3 rayOrigin = transform.position + _groundCheckRayOffset;
        bool rayHit = Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit rayInfo,
                                      _settings.GroundTolerance * 2f, _settings.GroundLayer);

        // 2. Détection périphérique par sphère d'overlap (pour les bords de plateformes)
        Vector3 sphereOrigin = transform.position + _groundCheckSphereOffset;
        int overlapCount = Physics.OverlapSphereNonAlloc(sphereOrigin, _groundCheckRadius, _overlapResults, _settings.GroundLayer);
        bool sphereHit = overlapCount > 0;

        bool wasGrounded = _state.IsGrounded;
        bool isGrounded = rayHit || sphereHit;

        if (isGrounded)
        {
            // Détermination du sol sous les pieds
            Transform currentGround = rayHit ? rayInfo.collider.transform : _overlapResults[0].transform;

            // Si le joueur vient d'atterrir sur ce sol
            if (currentGround != _state.Ground)
            {
                _state.Ground = currentGround;
                // Enregistrement des valeurs initiales pour calculer la différence de position au prochain frame
                _lastPlatformPosition = _state.Ground.position;
                _lastPlatformRotation = _state.Ground.rotation;

                _platformVelocity.y = 0;

                return;
            }

            // Calcul et application de la rotation de la plateforme (pour faire pivoter le joueur)
            Quaternion rotationDelta = _state.Ground.rotation * Quaternion.Inverse(_lastPlatformRotation);
            float platformYaw = rotationDelta.eulerAngles.y;

            if (Mathf.Abs(platformYaw) > .001f)
            {
                Vector3 dir = transform.position - _state.Ground.position;
                dir = Quaternion.Euler(0, platformYaw, 0) * dir;
                transform.position = _state.Ground.position + dir;
                transform.Rotate(0, platformYaw, 0);
            }

            // Calcul et application du déplacement linéaire de la plateforme
            Vector3 platformDelta = _state.Ground.position - _lastPlatformPosition;
            transform.position += platformDelta;

            // Mise à jour des valeurs pour la frame suivante
            _lastPlatformPosition = _state.Ground.position;
            _lastPlatformRotation = _state.Ground.rotation;

            // Force la mise à jour des transformations physiques d'Unity pour éviter des bugs de collision
            Physics.SyncTransforms();

            _platformVelocity = Vector3.zero;
        }
        else
        {
            // Si le joueur vient de décoller d'une plateforme en mouvement, il conserve sa vitesse acquise (impulsion)
            if (wasGrounded && _state.Ground != null)
            {
                _platformVelocity = (_state.Ground.position - _lastPlatformPosition) / Time.deltaTime;
            }
            // Sinon, réduction progressive de cette vitesse héritée dans les airs (frottement de l'air)
            else
            {
                Vector3 platformVelocity = Vector3.MoveTowards(_platformVelocity, Vector3.zero, _settings.ExtraForcesDrag * deltaTime);
                platformVelocity.y = _platformVelocity.y;
                _platformVelocity = platformVelocity;
            }
            
            _state.Ground = null;
        }
    }

    /// <summary>
    /// Calcule l'impact de la gravité sur la vitesse verticale du joueur.
    /// </summary>
    private void SetGravity(float deltaTime)
    {
        if (_state.IsGrounded && _state.Velocity.y < 0)
        {
            // Au sol, on applique une petite force constante vers le bas
            _state.Velocity.y = STICK_FORCE;
        }
        else
        {
            // En l'air, application de la gravité classique
            if (_platformVelocity.y > 0)
            {
                _platformVelocity.y += GRAVITY * deltaTime;

                if (_platformVelocity.y < 0)
                    _state.Velocity.y += _platformVelocity.y;
            }
            else
            {
                _state.Velocity.y += GRAVITY * deltaTime;
            }

            // Limitation pour ne pas tomber à une vitesse infinie
            _state.Velocity.y = Mathf.Max(_state.Velocity.y, MAX_GRAVITY);
        }
    }

    /// <summary>
    /// Traduit les entrées clavier ou manette en vitesse horizontale X/Z, alignée sur la caméra du joueur,
    /// et oriente le joueur vers sa direction de marche.
    /// </summary>
    private void SetVelocity(float deltaTime)
    {
        Vector2 input = _moveAction.ReadValue<Vector2>();
        
        float speed = _settings.Speed * KMH_TO_MS;
        
        // On oriente le mouvement selon l'angle de vue de la caméra sur l'axe horizontal (Y)
        Vector3 moveInput = new Vector3(input.x, 0, input.y);
        moveInput = Quaternion.Euler(0, _camera.transform.eulerAngles.y, 0) * moveInput;
        moveInput = moveInput * speed;
        
        _state.Velocity.x = moveInput.x;
        _state.Velocity.z = moveInput.z;
        
        // S'orienter face au mouvement
        if (moveInput.sqrMagnitude > .001f)
        {
            Quaternion targetRot =  Quaternion.LookRotation(moveInput);
            float t = _settings.RotationSpeed * deltaTime;
            Vector3 euler = Quaternion.Slerp(transform.rotation, targetRot, t).eulerAngles;
            
            // On conserve uniquement la rotation sur l'axe vertical Y (pas de tangage ni roulis)
            transform.rotation = Quaternion.Euler(0, euler.y, 0);
        }
    }

    /// <summary>
    /// Déclenche un saut classique si le joueur presse le bouton de saut et est actuellement au sol.
    /// </summary>
    private void SetJump()
    {
        if (_jumpAction.triggered && _state.IsGrounded)
        {
            _state.Velocity.y = _settings.JumpForce;
            _state.Ground = null;
        }
    }

    /// <summary>
    /// Permet d'injecter une force verticale soudaine (utilisé par exemple par un trampoline).
    /// </summary>
    public void Bounce(float force)
    {
        _state.Velocity.y = force;
        _state.Ground = null;
    }

    /// <summary>
    /// Déplace le joueur via le CharacterController en combinant sa propre vitesse et celle de la plateforme.
    /// </summary>
    private void SetMovement(float deltaTime)
    {
        Vector3 motion = _state.Velocity + _platformVelocity;
        _references.Controller.Move(motion * deltaTime);
    }

    /// <summary>
    /// Détermine l'état du joueur pour piloter la machine d'animation.
    /// </summary>
    private void SetState()
    {
        // Si la partie est terminée pour ce joueur, on ne change plus son état
        if (State.CurrentState == PlayerState.Winner ||
            State.CurrentState == PlayerState.Loser ||
            State.CurrentState == PlayerState.Eliminated)
            return;

        if (State.IsGrounded)
        {
            // Déplacement horizontal significatif au sol
            if (State.HorizontalVelocity.sqrMagnitude > .1f)
            {
                State.CurrentState = PlayerState.Moving;
            }
            else
            {
                State.CurrentState = PlayerState.Idle;
            }
        }
        else
        {
            // En l'air
            if (State.VerticalVelocity > 0)
            {
                State.CurrentState = PlayerState.Falling;
            }
        }
    }
    #endregion
}
