using UnityEngine;

public class Battery : InteractableBase
{
    public override void Interact(GameObject player)
    {
        if (player.GetComponentInChildren<FlashlightController>() is FlashlightController flashlight)
        {
            flashlight.AddBattery(1);
            Debug.Log($"Batería recogida. Total: {flashlight.batteriesStored}");
            Destroy(gameObject);
        }
    }

}
