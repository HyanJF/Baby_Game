using UnityEngine;

public interface IInteractable
{
    void OnFocus();
    void OnLoseFocus();

    void Interact(GameObject player);
}
