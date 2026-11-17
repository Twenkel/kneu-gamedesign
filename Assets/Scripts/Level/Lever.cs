using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject target;
    [SerializeField] private SpriteRenderer handle;

    private bool isOn;

    public void Interact()
    {
        isOn = !isOn;
        target.SetActive(isOn);
        if (handle != null) handle.flipX = isOn;
    }
}
