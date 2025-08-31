using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeGameManager : MonoBehaviour
{
    // The prefab for the game pieces.
    public GameObject gamePiecePrefab;
    // The LineRenderer component to draw the rope.
    public LineRenderer lineRenderer;
    // The distance limit for connecting pieces.
    public float connectionDistance = 1.5f;

    // A list of all available GamePieceData Scriptable Objects.
    public List<GamePieceData> allPieceData;

    // List to hold all game pieces on the board.
    private List<GameObject> allGamePieces;
    // List to hold the currently selected pieces in the chain.
    private List<GameObject> selectedChain;
    // The type of the starting piece in the chain.
    private string currentPieceType;

    // A flag to check if the player is currently dragging.
    private bool isDragging = false;

    void Start()
    {
        allGamePieces = new List<GameObject>();
        selectedChain = new List<GameObject>();
        lineRenderer.positionCount = 0;
        PopulateBoard();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject hitPiece = hit.collider.gameObject;
                GamePiece pieceScript = hitPiece.GetComponent<GamePiece>();

                if (pieceScript != null)
                {
                    selectedChain.Add(hitPiece);
                    currentPieceType = pieceScript.pieceData.pieceType;
                    UpdateLineRenderer();
                }
            }
        }

        if (isDragging && Input.GetMouseButton(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

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

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            if (selectedChain.Count >= 3)
            {
                ProcessChain();
            }
            selectedChain.Clear();
            lineRenderer.positionCount = 0;
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
