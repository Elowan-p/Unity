using UnityEngine;

[CreateAssetMenu(fileName = "Timer", menuName = "Scriptable Objects/Timer")]
public class GameTimer : ScriptableObject
{
    public float ElapsedTime { get; private set; }
    public bool IsRunning { get; private set; }

    private void OnEnable()
    {
        ElapsedTime = 0f;
        IsRunning = false;
    }

    public void StartTimer()
    {
        ElapsedTime = 0f;
        IsRunning = true;
    }

    public void Tick(float deltaTime)
    {
        if (IsRunning)
            ElapsedTime += deltaTime;
    }

    public void Stop()
    {
        IsRunning = false;
    }

    public override string ToString()
    {
        int minutes = Mathf.FloorToInt(ElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(ElapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((ElapsedTime % 1f) * 1000f);
        return $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    }
}
