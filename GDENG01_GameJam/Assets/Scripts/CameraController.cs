using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float x;
    private float y;
    [SerializeField] public float sensitivity = -1.0f;
    private Vector3 rotate;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        y = Input.GetAxis("Mouse X");
        x = Input.GetAxis("Mouse Y");
        rotate = new Vector3(0, y * sensitivity, 0);
        transform.eulerAngles = transform.eulerAngles - rotate;
    }
}
