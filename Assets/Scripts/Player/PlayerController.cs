using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Parameters
    [Header("Motion")]
    public float speed = 5f;
    public float gravity = -9.81f;

    [Header("Camera")]
    public float mouseSensitivity = 2f;
    public float verticalClamp = 30f;
    public Transform cameraPlayer;

    #endregion

    private void start()
    {

    }
}
