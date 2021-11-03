using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float cameraSpeed = 1.0f;
    public float sprintMultiplier = 1.5f;
    public float jumpSpeed = 2f;
    public LayerMask shootMask, teraainMask;

    public float attackSpeed = 1f;
    public float reloadSpeed = 1f;
    private float attackCooldown = 0f;
    private bool reloading = false;
    public int resource = 0;

    public int ammo = 10;
    public int magazineCap = 10;
    public int totalAmmo = 100;
    public int health = 100;
    
    Rigidbody rb;
    Collider col;
    Vector3 velocity;
    Camera mainCamera;
    bool w, a, s, d, leftclick, sprint, jump, r;
    float mouseX, mouseY;
    Vector3 mouseDir;
    RaycastHit hit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
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
        r = Input.GetKeyDown(KeyCode.R);
        leftclick = Input.GetMouseButton(0);
        sprint = Input.GetButton("Fire3");
        jump = Input.GetButtonDown("Jump");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        mouseDir += new Vector3(-mouseY, mouseX, 0) * cameraSpeed;
        mouseDir.x = Mathf.Clamp(mouseDir.x, -85f, 85f);
        attackCooldown -= Time.deltaTime;

        if (reloading && attackCooldown <= 0)
        {
            reloading = false;
            Debug.Log("Reloaded");
        }

        if (jump && IsGrounded())
        {
            velocity = rb.velocity;
            velocity.y = jumpSpeed;
            rb.velocity= velocity;
        }

        if (leftclick && !sprint && attackCooldown <= 0)
        {
            if (totalAmmo == 0)
            {
                //no ammo
                Debug.Log("No ammo");
            }
            else if (ammo == 0)
            {
                //out of ammo
                Debug.Log("Need to reload");
                Reload();
            }
            else
            {
                ammo--;
                totalAmmo--;
                attackCooldown = 1f / attackSpeed;
                if (Physics.Raycast(this.transform.position, mainCamera.transform.forward, out hit, 100f, shootMask))
                {
                    Debug.Log("Hit " + hit.collider.name);
                    hit.collider.gameObject.GetComponent<EnemyInfo>().Hurt(20);
                }
                if (Physics.Raycast(this.transform.position, mainCamera.transform.forward, out hit, 100f, teraainMask))
                {
                }
                Debug.DrawLine(this.transform.position, this.transform.position + mainCamera.transform.forward * 100f, Color.red, 2f);
            }
        }

        if (r)
        {
            Reload();
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
        if (sprint && !s && (w || a || d))
        {
            velocity *= sprintMultiplier;
        }
        velocity.y = rb.velocity.y;

        rb.velocity = velocity;

        this.transform.localEulerAngles = Vector3.up * mouseDir.y;
        mainCamera.transform.localEulerAngles = Vector3.right * mouseDir.x;
    }

    private bool IsGrounded()
    {
        Vector3 boxCenter = this.transform.position;
        boxCenter.y -= col.bounds.size.y * 0.5f;

        Vector3 bound = col.bounds.size / 2;
        bound.x -= 0.1f;
        bound.y = 0.1f;
        bound.z -= 0.1f;

        return Physics.CheckBox(boxCenter, bound, Quaternion.identity, teraainMask);
    }

    private void Reload()
    {
        Debug.Log("Reloading");
        reloading = true;
        attackCooldown = 2.5f / reloadSpeed;
        ammo = magazineCap;
    }

    public void Hurt(int damage)
    {
        health -= damage;
        Debug.Log("Got hurt");
        if (health <= 0)
        {
            Debug.Log("You're Dead");
        }
    }

    public void PickLoot(int getResource)
    {
        Debug.Log("Get " + getResource + " resources");
        resource += getResource;
    }
}

