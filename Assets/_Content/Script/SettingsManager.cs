using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public TMP_Text sensivityLabel;
    public UnityEngine.UI.Scrollbar sensivityScrollbar;

    void Start()
    {
        float value = PlayerPrefs.GetFloat("Sensitivity", 1f);
        if (sensivityScrollbar != null)
            sensivityScrollbar.value = value;
        UpdateLabel(value);
    }

    public void setSensivity(float value)
    {
        UpdateLabel(value);
        Debug.Log("setSensivity" + value);
        PlayerPrefs.SetFloat("Sensitivity", value);

        int sensitivity = Mathf.Clamp(Mathf.RoundToInt(value * 100f), 1, 20); // Mutiplicateur

        PlayerCamera cam = FindFirstObjectByType<PlayerCamera>();
        if (cam)
        {
            cam.SetSensitivity(sensitivity);
            Debug.Log("Caméra trouvé");
        }
    }

    private void UpdateLabel(float value)
    {
        int percentage = Mathf.RoundToInt(value * 100f);
        sensivityLabel.text = percentage + "%";
    }

    public void loadMenus()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
