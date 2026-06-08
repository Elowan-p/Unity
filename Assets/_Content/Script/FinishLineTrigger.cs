using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishLineTrigger : MonoBehaviour
{
    [SerializeField] private GameTimer gameTimer;
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
        gameTimer?.Stop();
        if (winText == null)
        {
            Debug.LogWarning("winText non assigné dans l'inspecteur !");
        }
        else
        {
            Debug.Log($"Activation winText: {winText.name}");
            winText.SetActive(true);
        }

        StartCoroutine(LoadNextSceneAfterDelay());
    }

    private IEnumerator LoadNextSceneAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        SceneManager.LoadScene(nextSceneName);
    }
}
