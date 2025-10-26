using UnityEngine;

public class InteractableBase : MonoBehaviour, IInteractable
{
    [Header("Visual")]
    public Renderer rend;
    private Material matInstance;

    [Range(0f, 3f)] public float emissionPower = 2f;
    public Color emissionColor = Color.yellow;

    protected virtual void Start()
    {
        if (rend != null)
        {
            matInstance = rend.material;
            DisableEmission();
        }
    }

    public virtual void OnFocus()
    {
        EnableEmission();
    }

    public virtual void OnLoseFocus()
    {
        DisableEmission();
    }

    public virtual void Interact(GameObject player)
    {
        Debug.Log($"{name} fue interactuado por {player.name}");
    }

    protected void EnableEmission()
    {
        if (matInstance != null && matInstance.HasProperty("_EmissionColor"))
        {
            matInstance.EnableKeyword("_EMISSION");
            matInstance.SetColor("_EmissionColor", emissionColor * emissionPower);
        }
    }

    protected void DisableEmission()
    {
        if (matInstance != null && matInstance.HasProperty("_EmissionColor"))
        {
            matInstance.SetColor("_EmissionColor", Color.black);
            matInstance.DisableKeyword("_EMISSION");
        }
    }
}
