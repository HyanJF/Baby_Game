using UnityEngine;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    #region Parameters
    [Header("References")]
    public Light flashlight;
    public Transform flashlightTip;
    public LayerMask detectionMask;

    [Header("Energy Settings")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float drainRate = 10f;
    public float rechargeRate = 5f;

    [Header("Light Settings")]
    public float maxRange = 15f;
    public float minRange = 3f;
    public Color fullEnergyColor = Color.yellow;
    public Color lowEnergyColor = Color.black;

    private bool isOn = false;

    [Header("Cone Detection")]
    [Range(0, 180)] public float coneAngle = 30f;
    private bool detectedThisFrame = false;

    #endregion

    private void Start()
    {
        currentEnergy = maxEnergy;
        flashlight.enabled = false;
        flashlight.range = maxRange;
        flashlight.color = fullEnergyColor;
    }

    private void Update()
    {
        if (isOn)
        {
            ConsumeEnergy();
            DetectObjectsInCone();
        }
        else
        {
            RechargeEnergy();
        }

        UpdateLightProperties();
    }

    private void ConsumeEnergy()
    {
        currentEnergy -= drainRate * Time.deltaTime;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        if (currentEnergy <= 0)
        {
            TurnOff();
            Debug.LogWarning("No hay energía");
        }
    }

    private void RechargeEnergy()
    {
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += rechargeRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
        }
    }

    private void UpdateLightProperties()
    {
        float t = currentEnergy / maxEnergy;
        flashlight.range = Mathf.Lerp(minRange, maxRange, t);
        flashlight.color = Color.Lerp(lowEnergyColor, fullEnergyColor, t);
    }

    public void TurnOff()
    {
        isOn = false;
        flashlight.enabled = false;
    }

    private void DetectObjectsInCone()
    {
        if (!flashlightTip || !flashlight.enabled) return;

        detectedThisFrame = false;

        Collider[] colliders = Physics.OverlapSphere(flashlightTip.position, flashlight.range, detectionMask);
        foreach (Collider col in colliders)
        {
            Vector3 dirToTarget = (col.transform.position - flashlightTip.position).normalized;
            float angle = Vector3.Angle(flashlightTip.forward, dirToTarget);

            if (angle <= coneAngle)
            {
                detectedThisFrame = true;
                Debug.DrawLine(flashlightTip.position, col.transform.position, Color.red);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!flashlightTip || !flashlight.enabled) return;

        // Color según detección
        Gizmos.color = detectedThisFrame ? Color.red : Color.green;

        // Sphere del rango
        Gizmos.DrawWireSphere(flashlightTip.position, flashlight.range);

        // Cono
        Vector3 forward = flashlightTip.forward * flashlight.range;
        Vector3 rightLimit = Quaternion.Euler(0, coneAngle, 0) * forward;
        Vector3 leftLimit = Quaternion.Euler(0, -coneAngle, 0) * forward;

        Gizmos.DrawLine(flashlightTip.position, flashlightTip.position + rightLimit);
        Gizmos.DrawLine(flashlightTip.position, flashlightTip.position + leftLimit);
    }

    #region Input System
    public void OnToggleFlashlight(InputAction.CallbackContext context)
    {
        if (context.performed && currentEnergy > 0)
        {
            isOn = !isOn;
            flashlight.enabled = isOn;
            Debug.Log(isOn ? "Linterna encendida" :  "Linterna apagada");
        }
    }
    #endregion
}
