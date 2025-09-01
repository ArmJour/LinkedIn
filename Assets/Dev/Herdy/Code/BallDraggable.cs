using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallDraggable : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;

    [Header("Drag Settings")]
    public float dragSpeed = 10f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
    }

    void OnMouseUp()
    {
        isDragging = false;
        rb.linearVelocity = Vector2.zero; 
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z) + offset;

            Vector2 direction = (targetPos - transform.position);

            rb.linearVelocity = direction * dragSpeed;
        }
    }
}
