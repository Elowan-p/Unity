using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   public void PlayGame()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("SettingsScene");
    } 

    public void OpenMenu(){
        SceneManager.LoadScene("MenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}