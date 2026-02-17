using UnityEngine;

public class MovementCamera : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] float sensitivity = 2f;

    float rotX = 0f, rotY = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleCursor();
        MoveCamera();

    }

    void HandleCursor()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void MoveCamera()
    {
        if (!Input.GetMouseButton(1)) return;

        rotX -= Input.GetAxis("Mouse Y") * sensitivity;
        rotY += Input.GetAxis("Mouse X") * sensitivity;
        rotX = Mathf.Clamp(rotX, -90f, 90f);
        Camera.main.transform.rotation = Quaternion.Euler(rotX, rotY, 0f);

        float right = 0, up = 0, forward = 0;

        if (Input.GetKey(KeyCode.W)) forward = 1f;
        else if (Input.GetKey(KeyCode.S)) forward = -1f;

        if (Input.GetKey(KeyCode.A)) right = -1f;
        else if (Input.GetKey(KeyCode.D)) right = 1f;

        if (Input.GetKey(KeyCode.Q)) up = -1f;
        else if (Input.GetKey(KeyCode.E)) up = 1f;

        Vector3 dir = transform.TransformDirection(new Vector3(right, up, forward).normalized);
        transform.position += dir * speed * Time.deltaTime;


    }
}
