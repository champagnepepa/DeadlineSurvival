using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public static MouseLook Instance { get; set; }

    public float mouseSensitivity = 100f;

    public bool isInterrupted = false;


    //float xRotation;

    float targetXRotation;
    float targetYRotation;

    
    public Transform playerBody;

    Transform parent;
    Transform cam;

    PlayerMovement pm;

    // Start is called before the first frame update
    void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main.transform;
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        //float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        //float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //
        //targetXRotation -= mouseY;
        //targetXRotation = Mathf.Clamp(targetXRotation, -90f, 50f);
        //
        //transform.localRotation = Quaternion.Euler(targetXRotation, 0f, 0f);
        //playerBody.Rotate(Vector3.up * mouseX);

        if (pm.isInterrupted)
            return;

        targetXRotation -= Input.GetAxisRaw("Mouse Y");
        targetYRotation += Input.GetAxisRaw("Mouse X");
        targetXRotation = Mathf.Clamp(targetXRotation, -65f, 70f);

        parent.eulerAngles = new Vector3(0, targetYRotation, 0);
        cam.localEulerAngles = new Vector3(targetXRotation, 0, 0);

        //playerBody.Rotate(Vector3.up * targetXRotation);


    }
}
