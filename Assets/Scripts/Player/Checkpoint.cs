using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private SpriteRenderer flag;
    [SerializeField] private Color activeColor = Color.white;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<PlayerRespawn>(out var respawn)) return;

        respawn.SetCheckpoint(transform.position);
        if (flag != null) flag.color = activeColor;
    }
}
