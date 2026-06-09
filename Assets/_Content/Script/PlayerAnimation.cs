using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Gère les animations du joueur en traduisant son état logique (PlayerState) 
/// en triggers et paramètres pour l'Animator Unity.
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    // Accès statique global (Singleton)
    public static PlayerAnimation Instance { get; private set; }

    [System.Serializable]
    public class References
    {
        [Tooltip("Composant Animator attaché au modèle du joueur.")]
        public Animator Anim;
    }
    
    /// <summary>
    /// Permet de mapper un état de jeu (ex: Jumping) avec son animation (ex: jump) 
    /// et le trigger de transition associé.
    /// </summary>
    [System.Serializable]
    private class PlayerAnimationStateMapper
    {
        public Player.PlayerState PlayerState;
        public string AnimatorState;
        public string BlockingState;
        public string Trigger;
    }

    [SerializeField] private References _references;
    [SerializeField, ReadOnly] private PlayerAnimationStateMapper[] _stateMapper;

    // Utilisé pour rafraîchir l'initialisation des états directement dans l'éditeur Unity lors de modifications
    void OnValidate()
    {
        Init();
    }

    void Awake()
    {
        Init();

        if (!Instance)
            Instance = this;
    }

    // On utilise LateUpdate pour s'assurer que les positions et états finaux de la frame 
    // ont bien été calculés par la physique (dans Update) avant de mettre à jour les animations.
    void LateUpdate()
    {
        UpdateAnimation();
    }

    /// <summary>
    /// Renseigne le tableau de mapping pour faire correspondre chaque état logique à un état de l'Animator.
    /// </summary>
    private void Init()
    {
        _stateMapper = new PlayerAnimationStateMapper[8];

        _stateMapper[0] = new PlayerAnimationStateMapper()
        {
            PlayerState = Player.PlayerState.Idle,
            AnimatorState = "move",
            BlockingState = "",
            Trigger = "trigger_move",
        };

        _stateMapper[1] = new PlayerAnimationStateMapper()
        {
            PlayerState = Player.PlayerState.Moving,
            AnimatorState = "move",
            BlockingState = "",
            Trigger = "trigger_move",
        };

        _stateMapper[2] = new PlayerAnimationStateMapper()
        {
            PlayerState = Player.PlayerState.Jumping,
            AnimatorState = "jump",
            BlockingState = "fall",
            Trigger = "trigger_jump",
        };

        _stateMapper[3] = new PlayerAnimationStateMapper()
        {
            PlayerState = Player.PlayerState.Falling,
            AnimatorState = "fall",
            BlockingState = "jump",
            Trigger = "trigger_fall",
        };

        _stateMapper[4] = new PlayerAnimationStateMapper()
        {
            PlayerState = Player.PlayerState.Stunned,
            AnimatorState = "stun",
            BlockingState = "",
            Trigger = "trigger_stun",
        };

        _stateMapper[5] = new PlayerAnimationStateMapper()
        {
            PlayerState = Player.PlayerState.Eliminated,
            AnimatorState = "eliminate",
            BlockingState = "",
            Trigger = "trigger_eliminate",
        };

        _stateMapper[6] = new PlayerAnimationStateMapper()
        {
            PlayerState = Player.PlayerState.Loser,
            AnimatorState = "lose",
            BlockingState = "",
            Trigger = "trigger_lose",
        };

        _stateMapper[7] = new PlayerAnimationStateMapper()
        {
            PlayerState = Player.PlayerState.Winner,
            AnimatorState = "win",
            BlockingState = "",
            Trigger = "trigger_win",
        };
    }

    /// <summary>
    /// Analyse l'état logique actuel du joueur et envoie le trigger correspondant à l'Animator.
    /// </summary>
    private void UpdateAnimation()
    {
        if (!Player.Instance)
            return;

        // On évite de renvoyer un trigger si une transition est déjà en cours dans l'Animator
        if (_references.Anim.IsInTransition(0))
            return;

        AnimatorStateInfo currentState = _references.Anim.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextState = _references.Anim.GetNextAnimatorStateInfo(0);

        // Trouve la configuration correspondant à l'état du joueur
        PlayerAnimationStateMapper currentStateMapper = _stateMapper.FirstOrDefault(m => m.PlayerState == Player.Instance.State.CurrentState);

        // Si l'état actuel n'est pas déjà en cours ou planifié, et qu'il n'est pas bloqué par un état prioritaire, on déclenche l'animation
        if (currentStateMapper != null &&
            !currentState.IsName(currentStateMapper.AnimatorState) &&
            !nextState.IsName(currentStateMapper.AnimatorState) &&
            !currentState.IsName(currentStateMapper.BlockingState))
        {
            _references.Anim.SetTrigger(currentStateMapper.Trigger);
        }

        // Pour les états de mouvement, on applique la vitesse horizontale réelle 
        // pour doser l'intensité de la marche/course (via un Blend Tree)
        switch (Player.Instance.State.CurrentState)
        {
            case Player.PlayerState.Idle:
            case Player.PlayerState.Moving:
                _references.Anim.SetFloat("move", Player.Instance.State.HorizontalVelocity.magnitude);
                break;
        }
    }
}
