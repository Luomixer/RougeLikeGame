using UnityEngine;
public class PlayerCam : MonoBehaviour
{
    public float SensitivityX;
    public float SensitivityY;

    public Transform orientation;

    float PlayerRotationX;
    float PlayerRotationY;

    private void Start()
    {
        // Centers cursor
        Cursor.lockState = CursorLockMode.Locked;

        // Hides cursor
        Cursor.visible = false;
    }

    void Update()
    {
        // Get mouse input
        float MouseXDirection = Input.GetAxisRaw("Mouse X") * SensitivityX * 10 * Time.deltaTime;
        float MouseYDirection = Input.GetAxisRaw("Mouse Y") * SensitivityY * 10 * Time.deltaTime;

        // Unity bs
        PlayerRotationX -= MouseYDirection;
        PlayerRotationY += MouseXDirection;

        // Prevents camera from going upsidedown
        PlayerRotationX = Mathf.Clamp(PlayerRotationX, -90f, 90f);

        // Moves the camera
        transform.rotation = Quaternion.Euler(PlayerRotationX, PlayerRotationY, 0);
        orientation.rotation = Quaternion.Euler(0, PlayerRotationY, 0);
    }
}
