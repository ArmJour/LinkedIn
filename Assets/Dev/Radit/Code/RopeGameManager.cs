using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RopeGameManager : MonoBehaviour
{
    public GameObject gamePiecePrefab;
    public LineRenderer lineRenderer;
    public float connectionDistance = 1.5f;
    public List<GamePieceData> allPieceData;

    private List<GameObject> allGamePieces;
    private List<GameObject> selectedChain;
    private string currentPieceType;

    private InputAction pointerPosition;
    private InputAction pointerPress;
    private bool isDragging = false;

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
        if (isDragging)
        {
            Vector2 pointerPos = Camera.main.ScreenToWorldPoint(pointerPosition.ReadValue<Vector2>());
            RaycastHit2D hit = Physics2D.Raycast(pointerPos, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject newPiece = hit.collider.gameObject;
                GamePiece newPieceScript = newPiece.GetComponent<GamePiece>();

                if (newPieceScript != null && newPieceScript.pieceData.pieceType == currentPieceType && !selectedChain.Contains(newPiece))
                {
                    GameObject lastPieceInChain = selectedChain[selectedChain.Count - 1];
                    if (Vector2.Distance(lastPieceInChain.transform.position, newPiece.transform.position) <= connectionDistance)
                    {
                        selectedChain.Add(newPiece);
                        UpdateLineRenderer();
                    }
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
                UpdateLineRenderer();
            }
        }
    }

    private void OnPointerUp(InputAction.CallbackContext context)
    {
        isDragging = false;
        if (selectedChain.Count >= 3)
        {
            ProcessChain();
        }
        selectedChain.Clear();
        lineRenderer.positionCount = 0;
    }

    void PopulateBoard()
    {
        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(-5f, 5f);
            float y = Random.Range(-3f, 3f);
            GameObject newPiece = Instantiate(gamePiecePrefab, new Vector2(x, y), Quaternion.identity);

            GamePieceData randomData = allPieceData[Random.Range(0, allPieceData.Count)];
            newPiece.GetComponent<GamePiece>().pieceData = randomData;

            allGamePieces.Add(newPiece);
        }
    }

    void UpdateLineRenderer()
    {
        lineRenderer.positionCount = selectedChain.Count;
        for (int i = 0; i < selectedChain.Count; i++)
        {
            lineRenderer.SetPosition(i, selectedChain[i].transform.position);
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
