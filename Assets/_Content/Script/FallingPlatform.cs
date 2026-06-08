using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Range(0f, 5f)]
    public float fallDelay = 2f;
    public float fallSpeed = 10f;
    public float riseSpeed = 25f;
    public float resetDelay = 7f;

    private bool isFalling = false;
    private bool isRising = false;
    private bool hasBeenTriggered = false;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

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
        Invoke(nameof(StartRising), resetDelay);
    }

    void StartRising()
    {
        isFalling = false;
        isRising = true;
    }

    void Update()
    {
        if (isFalling)
        {
            transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
        }
        else if (isRising)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, riseSpeed * Time.deltaTime);
            if (transform.position == initialPosition)
            {
                isRising = false;
                hasBeenTriggered = false;
            }
        }
    }
}
