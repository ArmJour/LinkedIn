using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RopeGameManager : MonoBehaviour
{
    public GameObject gamePiecePrefab;

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;   // cuma 1 line renderer

    [Header("Gameplay Settings")]
    public float connectionDistance = 1.5f; // radius deteksi neighbor
    public float linkSpacing = 0.6f;        // jarak antar piece yang ikut ketarik
    public float followSmoothness = 10f;    // kehalusan gerakan follow
    public List<GamePieceData> allPieceData;

    private List<GameObject> allGamePieces;
    private List<GameObject> selectedChain;
    private string currentPieceType;

    private InputAction pointerPosition;
    private InputAction pointerPress;
    private bool isDragging = false;
    private GameObject originPiece; // piece pertama yang di-drag

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
        if (isDragging && selectedChain.Count > 0)
        {
            // update line renderer
            UpdateLineRenderer();

            // cek dari lastPiece
            GameObject lastPiece = selectedChain[selectedChain.Count - 1];
            TryAddNeighbor(lastPiece);

            // cek juga dari originPiece
            if (originPiece != null)
            {
                TryAddNeighbor(originPiece);
            }

            // 🔹 Tarik semua piece agar mengikuti seperti di Cafe Mix
            UpdateDraggedPieces();
        }
    }

    void LateUpdate()
    {
        if (selectedChain.Count > 0)
        {
            UpdateLineRenderer();
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
                originPiece = hitPiece; // simpan pusat
                UpdateLineRenderer();
            }
        }
    }

    private void OnPointerUp(InputAction.CallbackContext context)
    {
        isDragging = false;
        if (selectedChain.Count >= 2) // minimal 2 biar valid
        {
            ProcessChain();
        }

        selectedChain.Clear();
        lineRenderer.positionCount = 0;
        originPiece = null;
    }

    void TryAddNeighbor(GameObject sourcePiece)
    {
        Vector2 pointerPos = Camera.main.ScreenToWorldPoint(pointerPosition.ReadValue<Vector2>());
        RaycastHit2D hit = Physics2D.Raycast(pointerPos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject newPiece = hit.collider.gameObject;
            GamePiece newPieceScript = newPiece.GetComponent<GamePiece>();

            if (newPieceScript != null &&
                newPieceScript.pieceData.pieceType == currentPieceType &&
                !selectedChain.Contains(newPiece))
            {
                // pastikan dia neighbor (tidak terlalu jauh dari last piece)
                float dist = Vector2.Distance(sourcePiece.transform.position, newPiece.transform.position);
                if (dist <= connectionDistance)
                {
                    selectedChain.Add(newPiece);
                    UpdateLineRenderer();
                }
            }
        }
    }


    void PopulateBoard()
    {
        for (int i = 0; i < 20; i++)
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
        if (originPiece == null) return;

        // total titik = rantai antar piece + garis pusat ke setiap piece (selain pusat)
        int totalPoints = selectedChain.Count + (selectedChain.Count - 1) * 2;
        lineRenderer.positionCount = totalPoints;

        int index = 0;

        // --- 1. Rantai antar piece ---
        for (int i = 0; i < selectedChain.Count; i++)
        {
            lineRenderer.SetPosition(index, selectedChain[i].transform.position);
            index++;
        }

        // --- 2. Tambahkan garis pusat ke setiap piece ---
        for (int i = 1; i < selectedChain.Count; i++)
        {
            lineRenderer.SetPosition(index, originPiece.transform.position);
            index++;
            lineRenderer.SetPosition(index, selectedChain[i].transform.position);
            index++;
        }
    }

    /// <summary>
    /// 🔹 Bikin semua piece dalam rantai ikut ketarik seperti Cafe Mix
    /// </summary>
    void UpdateDraggedPieces()
    {
        if (originPiece == null) return;

        for (int i = 1; i < selectedChain.Count; i++)
        {
            GameObject prev = selectedChain[i - 1];
            GameObject current = selectedChain[i];

            Vector2 dir = (current.transform.position - prev.transform.position).normalized;
            Vector2 targetPos = (Vector2)prev.transform.position + dir * linkSpacing;

            Rigidbody2D rb = current.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.MovePosition(Vector2.Lerp(current.transform.position, targetPos, Time.deltaTime * followSmoothness));
            }
            else
            {
                current.transform.position = Vector2.Lerp(current.transform.position, targetPos, Time.deltaTime * followSmoothness);
            }
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
