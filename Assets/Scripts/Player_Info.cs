using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Info : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float cameraSpeed = 1.0f;
    public LayerMask layerMask;
    
    Rigidbody rb;
    Vector3 velocity;
    Camera mainCamera;
    bool w, a, s, d, leftclick;
    float mouseX, mouseY;
    Vector3 mouseDir;
    RaycastHit hit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Start is called before the first frame update
    void Start()
    {
        mouseDir = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        w = Input.GetKey(KeyCode.W);
        a = Input.GetKey(KeyCode.A);
        s = Input.GetKey(KeyCode.S);
        d = Input.GetKey(KeyCode.D);
        leftclick = Input.GetMouseButtonDown(0);

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        mouseDir += new Vector3(-mouseY, mouseX, 0) * cameraSpeed;
        mouseDir.x = Mathf.Clamp(mouseDir.x, -85f, 85f);

        if (leftclick)
        {
            if (Physics.Raycast(this.transform.position, mainCamera.transform.forward, out hit, 100f, layerMask))
            {
                Debug.Log(hit.transform.name);
                hit.collider.gameObject.GetComponent<Enemy_Info>().Hurt(20);
            }
            Debug.DrawLine(this.transform.position, this.transform.position + mainCamera.transform.forward * 100f, Color.red, 2f);
        }
    }

    void FixedUpdate()
    {
        velocity = Vector3.zero;

        if (w) velocity += this.transform.forward;
        if (a) velocity += -this.transform.right;
        if (s) velocity += -this.transform.forward;
        if (d) velocity += this.transform.right;
        velocity *= movementSpeed;
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;

        this.transform.localEulerAngles = Vector3.up * mouseDir.y;
        mainCamera.transform.localEulerAngles = Vector3.right * mouseDir.x;
    }
}

