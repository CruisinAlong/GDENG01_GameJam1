using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayerController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float rotationSpeed = 5.0f;

    private Vector3 offset;

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned in LookAtController.");
            return;
        }

        offset = transform.position - player.position;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Quaternion camTurnAngle = Quaternion.AngleAxis(mouseX * rotationSpeed, Vector3.up);
        offset = camTurnAngle * offset;

        Vector3 newPos = player.position + offset;
        transform.position = Vector3.Slerp(transform.position, newPos, Time.deltaTime * rotationSpeed);

        transform.LookAt(player);
    }
}
