using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Composants requis sur ce GameObject dans Unity :
// - Collider (avec "Is Trigger" coché)
// Composants à assigner dans l'inspecteur :
// - winText : un GameObject contenant un Text (UI > Text) ou TextMeshPro,
//   placé dans un Canvas, désactivé par défaut.
public class FinishLineTrigger : MonoBehaviour
{
    [SerializeField] private GameObject winText;
    [SerializeField] private string nextSceneName = "Player";
    [SerializeField] private float displayDuration = 5f;
    [SerializeField] private string playerTag = "Player";

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger activé");
        if (triggered) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;
        win();
    }

    public void win()
    {
        winText.SetActive(true);

        StartCoroutine(WinRoutine());
    }

    private IEnumerator WinRoutine()
    {
        Player.PlayerState previousState = Player.PlayerState.Idle;
        bool hasPlayer = Player.Instance != null;

        if (hasPlayer)
        {
            previousState = Player.Instance.State.CurrentState;
            Player.Instance.State.CurrentState = Player.PlayerState.Winner;
        }

        yield return new WaitForSeconds(displayDuration);

        if (hasPlayer && Player.Instance != null)
        {
            Player.Instance.State.CurrentState = previousState;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
