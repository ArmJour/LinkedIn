using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RopeGameManager : MonoBehaviour
{
    [Header("Prefabs & Line Renderer")]
    public GameObject gamePiecePrefab;
    public LineRenderer lineRenderer;

    [Header("Gameplay Settings")]
    public float connectionDistance = 1.5f;
    public List<GamePieceData> allPieceData;

    [Header("Spring Settings")]
    public float springDistance = 0.6f;
    public float springFrequency = 5f;
    public float springDamping = 0.8f;

    [Header("Drag Settings")]
    [Range(0f, 1f)]
    public float dragLerp = 0.2f; // smooth factor untuk drag origin

    private List<GameObject> allGamePieces;
    private List<GameObject> selectedChain;
    private string currentPieceType;

    private InputAction pointerPosition;
    private InputAction pointerPress;
    private bool isDragging = false;
    private GameObject originPiece;

    void Awake()
    {
        pointerPosition = new InputAction(type: InputActionType.PassThrough, binding: "<Pointer>/position");
        pointerPress = new InputAction(type: InputActionType.Button, binding: "<Pointer>/press");
    }

    void OnEnable()
    {
        pointerPosition.Enable();
        pointerPress.Enable();
        pointerPress.started += OnPointerDown;
        pointerPress.canceled += OnPointerUp;
    }

    void OnDisable()
    {
        pointerPosition.Disable();
        pointerPress.Disable();
        pointerPress.started -= OnPointerDown;
        pointerPress.canceled -= OnPointerUp;
    }

    void Start()
    {
        allGamePieces = new List<GameObject>();
        selectedChain = new List<GameObject>();
        lineRenderer.positionCount = 0;
        PopulateBoard();
    }

    void Update()
    {
        if (isDragging && originPiece != null)
        {
            // 🔹 Smooth drag origin
            Vector2 pointerPos = Camera.main.ScreenToWorldPoint(pointerPosition.ReadValue<Vector2>());
            Rigidbody2D rb = originPiece.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 newPos = Vector2.Lerp(rb.position, pointerPos, dragLerp);
                rb.MovePosition(newPos);
            }

            // 🔹 Tambah neighbor baru dari origin
            TryAddNeighbor(originPiece);

            UpdateLineRenderer();
        }
        for (int i = 0; i < allGamePieces.Count; i++)
        {
            for (int j = i + 1; j < allGamePieces.Count; j++)
            {
                Rigidbody2D rbA = allGamePieces[i].GetComponent<Rigidbody2D>();
                Rigidbody2D rbB = allGamePieces[j].GetComponent<Rigidbody2D>();

                Vector2 dir = rbA.position - rbB.position;
                float distance = dir.magnitude;

                float minDist = 0.5f; // jarak minimal antar node
                if (distance < minDist && distance > 0)
                {
                    Vector2 repulse = dir.normalized * (minDist - distance) * 5f; // multiplier gaya tolakan
                    rbA.AddForce(repulse);
                    rbB.AddForce(-repulse);
                }
            }
        }
    }

    private void OnPointerDown(InputAction.CallbackContext context)
    {
        isDragging = true;
        Vector2 pointerPos = Camera.main.ScreenToWorldPoint(pointerPosition.ReadValue<Vector2>());
        RaycastHit2D hit = Physics2D.Raycast(pointerPos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject hitPiece = hit.collider.gameObject;
            GamePiece pieceScript = hitPiece.GetComponent<GamePiece>();

            if (pieceScript != null)
            {
                selectedChain.Clear();
                selectedChain.Add(hitPiece);
                currentPieceType = pieceScript.pieceData.pieceType;
                originPiece = hitPiece;

                UpdateLineRenderer();
            }
        }
    }

    private void OnPointerUp(InputAction.CallbackContext context)
    {
        isDragging = false;

        // 🔹 Tutup ring
        if (selectedChain.Count > 2)
        {
            GameObject first = selectedChain[0];
            GameObject last = selectedChain[selectedChain.Count - 1];

            SpringJoint2D joint = last.AddComponent<SpringJoint2D>();
            joint.connectedBody = first.GetComponent<Rigidbody2D>();
            joint.autoConfigureDistance = false;
            joint.distance = springDistance;
            joint.frequency = springFrequency;
            joint.dampingRatio = springDamping;
        }

        if (selectedChain.Count >= 2)
        {
            ProcessChain();
        }

        // 🔹 Hapus semua joint agar board siap lagi
        foreach (var piece in selectedChain)
        {
            foreach (var joint in piece.GetComponents<SpringJoint2D>())
            {
                Destroy(joint);
            }
        }

        selectedChain.Clear();
        lineRenderer.positionCount = 0;
        originPiece = null;
    }

    void TryAddNeighbor(GameObject sourcePiece)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(sourcePiece.transform.position, connectionDistance);

        GameObject closestPiece = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            GamePiece newPieceScript = hit.GetComponent<GamePiece>();
            if (newPieceScript != null)
            {
                GameObject newPiece = hit.gameObject;

                if (newPieceScript.pieceData.pieceType == currentPieceType &&
                    !selectedChain.Contains(newPiece))
                {
                    float dist = Vector2.Distance(originPiece.transform.position, newPiece.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestPiece = newPiece;
                    }
                }
            }
        }

        if (closestPiece != null)
        {
            selectedChain.Add(closestPiece);

            Rigidbody2D rbNew = closestPiece.GetComponent<Rigidbody2D>();
            Rigidbody2D rbOrigin = originPiece.GetComponent<Rigidbody2D>();

            // 🔹 Star: semua node terhubung ke origin
            SpringJoint2D starJoint = closestPiece.AddComponent<SpringJoint2D>();
            starJoint.connectedBody = rbOrigin;
            starJoint.autoConfigureDistance = false;
            starJoint.distance = springDistance;
            starJoint.frequency = springFrequency;
            starJoint.dampingRatio = springDamping;

            // 🔹 Ring: hubungkan ke node terakhir dalam ring (chain)
            if (selectedChain.Count > 1)
            {
                GameObject prevRing = selectedChain[selectedChain.Count - 2];
                SpringJoint2D ringJoint = closestPiece.AddComponent<SpringJoint2D>();
                ringJoint.connectedBody = prevRing.GetComponent<Rigidbody2D>();
                ringJoint.autoConfigureDistance = false;
                ringJoint.distance = springDistance;
                ringJoint.frequency = springFrequency;
                ringJoint.dampingRatio = springDamping;
            }

            UpdateLineRenderer();
        }
    }

    void PopulateBoard()
    {
        for (int i = 0; i < 200; i++)
        {
            float x = Random.Range(-5f, 5f);
            float y = Random.Range(-3f, 3f);
            GameObject newPiece = Instantiate(gamePiecePrefab, new Vector2(x, y), Quaternion.identity);

            GamePieceData randomData = allPieceData[Random.Range(0, allPieceData.Count)];
            newPiece.GetComponent<GamePiece>().SetData(randomData);

            allGamePieces.Add(newPiece);
        }
    }

    void UpdateLineRenderer()
    {
        if (selectedChain.Count == 0) return;

        lineRenderer.positionCount = selectedChain.Count + (selectedChain.Count > 2 ? 1 : 0);
        for (int i = 0; i < selectedChain.Count; i++)
        {
            lineRenderer.SetPosition(i, selectedChain[i].transform.position);
        }

        if (selectedChain.Count > 2)
        {
            lineRenderer.SetPosition(selectedChain.Count, selectedChain[0].transform.position);
        }
    }

    void ProcessChain()
    {
        Debug.Log("Chain of " + selectedChain.Count + " pieces cleared!");
        foreach (GameObject piece in selectedChain)
        {
            allGamePieces.Remove(piece);
            Destroy(piece);
        }
    }
}
