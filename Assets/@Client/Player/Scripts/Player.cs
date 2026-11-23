using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{

    [SerializeField]
    private float moveSpeed = 0f;
    [SerializeField]
    private float jumpPower = 0f;
    [SerializeField]
    private float currentPitch = 0f;
    [SerializeField]
    private float hp = 0f;
    private float maxHp = 100f;

    private Rigidbody rb = null;
    private MainCamera mainCamera = null;
    private MeshRenderer meshRenderer = null;
    private Color originalColor = default;

    private Gun gun = null;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = GetComponentInChildren<MainCamera>();
        gun = GetComponentInChildren<Gun>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        hp = maxHp;
    }

    public void Init()
    {
        originalColor = meshRenderer.material.color;
    }

    private void Update()
    {
        Move();
        Jump();
        Rotate();
        HandleCursor();
        Shoot();
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //ÃÑÀ» ¹ß»ç
            gun.Attack();
        }
    }

    private void HandleCursor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LockCursor();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }
    }
    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction
            = transform.forward * vertical + transform.right * horizontal;

        Vector3 normalizedDirection = direction.normalized;

        Vector3 velocity =
            new Vector3(normalizedDirection.x * moveSpeed, rb.linearVelocity.y, normalizedDirection.z * moveSpeed);

        rb.linearVelocity = velocity;
    }

    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        currentPitch -= mouseY * 2f;
        currentPitch
            = Mathf.Clamp(currentPitch, -90f, 90f);

        mainCamera.transform.localEulerAngles
            = new Vector3(currentPitch, 0f, 0f);


        float playerRotation = mouseX * 2f;
        transform.Rotate(0f, playerRotation, 0f);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 1.1f))
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(new Vector3(0f, 1f, 0f) * jumpPower, ForceMode.Impulse);
            }
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        StartCoroutine(CoPlayDamagedEffect());
        if (hp <= 0f)
        {
            Die();
        }
    }

    private IEnumerator CoPlayDamagedEffect()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        meshRenderer.material.color = originalColor;
    }

    private void Die()
    {
        Debug.Log("Player Á×À½!");
    }
}

