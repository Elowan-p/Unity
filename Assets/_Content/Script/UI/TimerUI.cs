using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameTimer gameTimer;

    private void Start()
    {
        gameTimer?.StartTimer();
    }

    private void Update()
    {
        if (gameTimer == null) return;

        gameTimer.Tick(Time.deltaTime);

        if (timerText != null)
            timerText.text = gameTimer.ToString();
    }
}
