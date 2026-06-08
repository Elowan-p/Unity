using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint;
    public float deathHeight = -20f;

    private CharacterController cc;
    private int currentCheckpointIndex = 0;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (transform.position.y < deathHeight)
        {
            Die();
        }
    }

    public void Die()
    {
        Respawn();
    }

    public void Respawn()
    {
        if (respawnPoint == null)
        {
            return;
        }

        if (cc != null)
            cc.enabled = false;

        transform.position = respawnPoint.position;

        if (cc != null)
            cc.enabled = true;
    }

    public void SetRespawnPoint(Transform newRespawnPoint, int checkpointIndex)
    {
        if (checkpointIndex <= currentCheckpointIndex)
        {
            return;
        }

        respawnPoint = newRespawnPoint;
        currentCheckpointIndex = checkpointIndex;
    }
}