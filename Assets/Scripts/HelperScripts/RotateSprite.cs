using UnityEngine;



public class RotateSprite : MonoBehaviour
{

    [SerializeField] private float rotationSpeed = 50.0f; // Rotation speed in degrees per second



    void Update()
    {
        // Rotate around the z-axis at the specified speed
        transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
    }
}
