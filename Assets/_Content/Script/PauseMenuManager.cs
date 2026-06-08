using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseCanvas;
    bool paused;

    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            paused = !paused;
            pauseCanvas.SetActive(paused);
            Time.timeScale = paused ? 0f : 1f;
            Player.Instance.State.IsPaused = paused;
        }
    }

    public void resumeGame(){
        paused = !paused; 
        pauseCanvas.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
        Player.Instance.State.IsPaused = paused;
    }

    public void accessMenu(){
        SceneManager.LoadScene("MenuScene");
    }

    public void quitGame(){
        Application.Quit();
    }
}