using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float cameraSpeed = 1.0f;
    public float sprintMultiplier = 1.5f;
    public float jumpSpeed = 2f;
    public LayerMask shootMask, teraainMask;

    public int attackDamage;
    public float attackMultiplier = 1f;
    public float attackSpeed = 1f;
    public float reloadSpeed = 1f;
    private float attackCooldown = 0f;
    public bool reloading = false;
    public int resource = 0;

    public int ammo = 10;
    public int magazineCap = 10;
    public int totalAmmo = 100;
    public int maxHealth = 100;
    public int health = 100;

    [SerializeField] GameObject UI;
    [SerializeField] GameObject deadScreen;
    [SerializeField] GameObject HitEffect;
    [SerializeField] GameObject HurtEffect;
    [SerializeField] Material NormalGun;
    [SerializeField] Material ReloadGun;
    Rigidbody rb;
    Collider col;
    Vector3 velocity;
    Camera mainCamera;
    public bool w, a, s, d, leftclick, sprint, jump, r, isGrounded;
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
        if (PlayerManager.gamePaused)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            Cursor.lockState = CursorLockMode.Confined;
            PlayerManager.instance.upgragePanel.SetActive(true);
            w = false;
            a = false;
            s = false;
            d = false;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            PlayerManager.instance.upgragePanel.SetActive(false);
        }

        if (isGrounded)
        {
            w = Input.GetKey(KeyCode.W);
            a = Input.GetKey(KeyCode.A);
            s = Input.GetKey(KeyCode.S);
            d = Input.GetKey(KeyCode.D);
        }
        sprint = Input.GetButton("Fire3");
        r = Input.GetKeyDown(KeyCode.R);
        leftclick = Input.GetMouseButton(0);
        jump = Input.GetButtonDown("Jump");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        mouseDir += new Vector3(-mouseY, mouseX, 0) * cameraSpeed;
        mouseDir.x = Mathf.Clamp(mouseDir.x, -85f, 85f);
        attackCooldown -= Time.deltaTime;
        isGrounded = IsGrounded();

        if (reloading && attackCooldown <= 0)
        {
            reloading = false;
            Debug.Log("Reloaded");
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
                StartCoroutine("FireEffect");

                if (Physics.Raycast(this.transform.position, mainCamera.transform.forward, out hit, 100f, shootMask))
                {
                    //Debug.Log("Hit " + hit.collider.name);
                    if (hit.collider.gameObject.tag == "Zombie")
                    {
                        hit.collider.gameObject.GetComponent<EnemyControl>().Hurt((int)(attackDamage * Mathf.Pow(attackMultiplier, 1.05f)));
                    }
                    else if (hit.collider.gameObject.tag == "Exploder")
                    {
                        hit.collider.gameObject.GetComponent<EnemyInfo>().Hurt((int)(attackDamage * Mathf.Pow(attackMultiplier, 1.05f)));
                    }
                    Instantiate(HitEffect, hit.point, Quaternion.identity);
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
        if (!isGrounded) velocity.y -= 9.8f * Time.deltaTime;
        else velocity.y = 0;

        if (jump && isGrounded)
        {
            velocity.y = jumpSpeed;
            isGrounded = false;
        }
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
        if (ammo == magazineCap) return;
        Debug.Log("Reloading");
        reloading = true;
        attackCooldown = 1f / reloadSpeed;
        //ammo = Mathf.Min(magazineCap, totalAmmo);
        mainCamera.transform.Find("SciFiGunLightBlack").gameObject.GetComponent<Renderer>().material = ReloadGun;
        StartCoroutine("Reloading", attackCooldown);
    }

    IEnumerator Reloading(float reloadTime)
    {
        for (;reloadTime >= 0; reloadTime -= Time.deltaTime)
        {
            yield return null;
        }
        ammo = Mathf.Min(magazineCap, totalAmmo);
        mainCamera.transform.Find("SciFiGunLightBlack").gameObject.GetComponent<Renderer>().material = NormalGun;
    }

    public void Hurt(int damage, Vector3 enemyPos)
    {
        health -= damage;
        StartCoroutine("ShowDamaged", enemyPos);
        //Debug.Log("Take " + damage + " damage");
        if (health <= 0)
        {    
            this.Dead();
        }
    }

    public void Dead()
    {
        UI.SetActive(false);
        deadScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        PlayerManager.gamePaused = true;
    }

    public void PickLoot(int getResource)
    {
        Debug.Log("Get " + getResource + " resources");
        resource += getResource;
    }

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
    }

    public void SpendResource(int cost)
    {
        resource -= cost;
    }

    public void HealToFull()
    {
        health = maxHealth;
    }

    public void ATKup()
    {
        attackDamage += 1;
    }

    public void ATKMulup(float amount)
    {
        attackMultiplier += amount;
    }

    public void ASup()
    {
        attackSpeed += 0.25f;
    }

    public void MaxHPup()
    {
        maxHealth += 10;
    }

    public void magCapup()
    {
        magazineCap += 1;
    }

    IEnumerator ShowDamaged(Vector3 pos)
    {
        Vector2 face = new Vector2(transform.forward.x, transform.forward.z);
        Vector2 right = new Vector2(transform.right.x, transform.right.z);
        Vector2 attackFrom = new Vector2(pos.x - transform.position.x, pos.z - transform.position.z);
        float angle = Vector2.Angle(face, attackFrom);
        if (Vector2.Dot(right, attackFrom) >=0)
        {
            angle = 360 - angle;
        }
        float angleR = angle * Mathf.PI / 180;

        // Draw effect
        Vector2 effectPos = new Vector2(-30 * Mathf.Sin(angleR), 30 * Mathf.Cos(angleR));
        GameObject tmp = Instantiate(HurtEffect, new Vector3(effectPos.x, effectPos.y, 0), Quaternion.identity);
        tmp.transform.SetParent(UI.transform, false);
        tmp.transform.localEulerAngles = Vector3.forward * angle;
        Color32 oriColor = tmp.GetComponent<Image>().color;
        for (float alpha = 200; alpha >= 0; alpha -= 200 * Time.deltaTime)
        {
            oriColor.a = (byte)alpha;
            tmp.GetComponent<Image>().color = oriColor;
            yield return null;
        }
        Destroy(tmp);
    }

    IEnumerator FireEffect()
    {
        mainCamera.transform.Find("SciFiGunLightBlack").Find("MuzzleFlash").gameObject.SetActive(true);
        for (int i = 0; i <= 2; i++)
        {
            yield return null;
        }
        mainCamera.transform.Find("SciFiGunLightBlack").Find("MuzzleFlash").gameObject.SetActive(false);
    }
}

