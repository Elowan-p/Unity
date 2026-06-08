using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint;
    public float deathHeight = -20f;

    private CharacterController cc;

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
        if (cc != null)
            cc.enabled = false;

        transform.position = respawnPoint.position;

        if (cc != null)
            cc.enabled = true;
    }
}