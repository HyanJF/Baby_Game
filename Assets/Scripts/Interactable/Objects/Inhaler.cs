using Unity.VisualScripting;
using UnityEngine;

public class Inhaler : InteractableBase
{
    public override void Interact(GameObject player)
    {
        if (player.GetComponent<PlayerController>() is PlayerController controller)
        {
            controller.AddInhaler(1);
            Debug.Log($"Inhaler recogido. Total: {controller.inhalersStored}");
            Destroy(gameObject);
        }
    }

}
