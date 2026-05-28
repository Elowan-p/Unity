using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadScene("Player");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    } 

    public void QuitGame()
    {
        Application.Quit();
    }
}