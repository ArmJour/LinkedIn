using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class BallDraggable : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    //private transform savedLocation;

    [Header("Drag Settings")]
    public float dragSpeed = 10f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
    if (RopeGameManager.InputBlocked) return; // block during destroy
        isDragging = true;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
    }

    void OnMouseUp()
    {
    if (RopeGameManager.InputBlocked) return; // already blocked
    StopDrag();
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            //if ()
            if (RopeGameManager.InputBlocked)
            {
                StopDrag();
                return;
            }

            // Jangan proses drag jika body di-set Static oleh manager
            if (rb.bodyType == RigidbodyType2D.Static)
            {
                StopDrag();
                return;
            }

            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 targetPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z) + offset;

            Vector2 direction = (targetPos - transform.position);

            rb.linearVelocity = direction * dragSpeed;
        }
    }

    private void StopDrag()
    {
        isDragging = false;
        if (rb != null && rb.bodyType != RigidbodyType2D.Static)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
