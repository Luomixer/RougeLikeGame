using UnityEngine;
public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    void Update()
    {
        // Avoid sudden camera movements
        transform.position = cameraPosition.position;
    }
}