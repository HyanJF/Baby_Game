using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Parameters
    [Header("Components")]
    public PlayerDetector pD;

    [Header("Motion")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;
    private float currentSpeed;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float stamina;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 15f;
    public float staminaSlowRegenRate = 5f;

    private bool canRun = true;
    private bool isRunning = false;

    [Header("Inhaler Inventory")]
    public int inhalersStored = 0;

    [Header("Camera")]
    public float mouseSensitivity = 2f;
    public float verticalClamp = 30f;
    public Transform cameraPlayer;
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 7f;

    private Vector3 cameraBasePosition;

    [Header("Hand & Flashlight")]
    public Transform handPivot;
    public Transform flashlightPivot;
    public float walkBobIntensity = 0.05f;
    public float runBobIntensity = 0.1f;
    public float bobSpeedWalk = 4f;
    public float bobSpeedRun = 7f;

    private float bobTimer;
    private Vector3 handBasePosition;
    private Vector3 flashlightBasePosition;

    [Header("Interaction")]
    public float interactDistance = 4f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private Vector3 moveDirection;
    private float verticalRotation = 0f;
    private bool lockCursor;
    #endregion

    private void Start()
    {
        TryGetComponent(out controller);
        lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = walkSpeed;
        stamina = maxStamina;

        //PosicionesIniciales
        cameraBasePosition = cameraPlayer.localPosition;
        handBasePosition = handPivot.localPosition;
        flashlightBasePosition = flashlightPivot.localPosition;

    }

    private void Update()
    {
        //Movimiento
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDirection = move * currentSpeed;
        controller.Move(moveDirection * Time.deltaTime);

        //Resistencia
        if (isRunning && moveInput.magnitude > 0f)
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);

            if (stamina <= 0)
            {
                canRun = false;
                isRunning = false;
                currentSpeed = walkSpeed;
                Debug.LogWarning("No puedes correr, estás cansado");
            }
        }
        else
        {
            if (stamina < maxStamina)
            {
                float regenRate = canRun ? staminaRegenRate : staminaSlowRegenRate;
                stamina += regenRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0, maxStamina);

                if (!canRun && stamina >= maxStamina)
                {
                    canRun = true;
                    Debug.Log("Resistencia recuperada");
                }
            }
        }

        //Gravedad
        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //Cámara
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalClamp, verticalClamp);
        cameraPlayer.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // mano y linterna efecto
        HandleHandAndFlashlightBob();
    }

    public void AddInhaler(int amount)
    {
        inhalersStored += amount;
    }

    private void HandleHandAndFlashlightBob()
    {

        float bobIntensity = isRunning ? runBobIntensity : walkBobIntensity;
        float bobSpeed = isRunning ? bobSpeedRun : bobSpeedWalk;

        //Cansado
        if (!canRun)
        {
            float shakeOffsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeAmount;
            float shakeOffsetY = Mathf.Cos(Time.time * shakeSpeed) * shakeAmount;

            cameraPlayer.localPosition = cameraBasePosition + new Vector3(shakeOffsetX, shakeOffsetY, 0);
        }
        else
        {
            cameraPlayer.localPosition = Vector3.Lerp(cameraPlayer.localPosition, cameraBasePosition, Time.deltaTime * 10f);
        }

        if (moveInput.magnitude > 0.1f)
        {
            bobTimer += Time.deltaTime * bobSpeed;
        }
        else
        {
            bobTimer = Mathf.Lerp(bobTimer, 0f, Time.deltaTime * 2f);
        }

        float offsetY = Mathf.Sin(bobTimer) * bobIntensity;
        float offsetX = Mathf.Cos(bobTimer * 0.5f) * bobIntensity * 0.5f;

        // Mano
        Vector3 handTarget = handBasePosition + new Vector3(offsetX, offsetY, 0);
        handPivot.localPosition = Vector3.Lerp(handPivot.localPosition, handTarget, Time.deltaTime * 10f);

        // Linterna
        Vector3 lightTarget = flashlightBasePosition + new Vector3(offsetX * 0.6f, offsetY * 1.2f, 0);
        flashlightPivot.localPosition = Vector3.Lerp(flashlightPivot.localPosition, lightTarget, Time.deltaTime * 6f);

    }

    #region Input System
    public void OnLockMouse(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        lockCursor = !lockCursor;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed && canRun && stamina > 0)
        {
            isRunning = true;
            currentSpeed = runSpeed;
            Debug.Log("Empezó a correr");
        }
        else if (context.canceled)
        {
            isRunning = false;
            currentSpeed = walkSpeed;
            Debug.Log("Dejó de correr");
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (pD != null && pD.HasTarget())
        {
            pD.TryInteract(gameObject);
        }
        else
        {
            Debug.Log("No hay nada wey");
        }

        
    }

    #endregion
}
