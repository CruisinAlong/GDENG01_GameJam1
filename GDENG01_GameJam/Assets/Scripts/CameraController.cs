using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference to the player's transform
    [SerializeField] private float rotationSpeed = 5.0f; // Speed of the camera rotation
    [SerializeField] private float distanceFromPlayer = 5.0f; // Distance from the player

    private Vector3 offset;

    void Start()
    {
        // Calculate the initial offset from the player
        offset = transform.position - player.position;
    }

    void LateUpdate()
    {
        // Rotate the camera around the player when the right mouse button is held
        if (Input.GetMouseButton(1))
        {
            float horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed;
            float verticalInput = -Input.GetAxis("Mouse Y") * rotationSpeed;

            // Rotate around the player horizontally
            transform.RotateAround(player.position, Vector3.up, horizontalInput);

            // Calculate the new offset after the horizontal rotation
            offset = transform.position - player.position;

            // Rotate around the player vertically
            Vector3 right = Vector3.Cross(Vector3.up, offset).normalized;
            transform.RotateAround(player.position, right, verticalInput);

            // Ensure the camera maintains the original distance from the player
            offset = offset.normalized * distanceFromPlayer;
            transform.position = player.position + offset;

            // Ensure the camera is always looking at the player
            transform.LookAt(player);
        }
        else
        {
            // Maintain the distance from the player
            transform.position = player.position + offset;
            transform.LookAt(player);
        }
    }
}

