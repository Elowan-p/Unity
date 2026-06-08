using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Range(0f, 5f)]
    public float fallDelay = 2f;
    public float fallSpeed = 10f;

    private bool isFalling = false;
    private bool hasBeenTriggered = false;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true;
            Invoke(nameof(StartFalling), fallDelay);
        }
    }

    void StartFalling()
    {
        isFalling = true;
    }

    void Update()
    {
        if (isFalling)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
    }
}
