using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RopeGameManager : MonoBehaviour
{
    public GameObject gamePiecePrefab;

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;

    [Header("Gameplay Settings")]
    public float connectionDistance = 1.5f;
    public List<GamePieceData> allPieceData;
    public int ballAmount = 100;

    [Header("Destroy Settings")]
    public float destroyDelay = 0.1f;
    public float destroyPopScale = 1.5f;
    public float destroyPopDuration = 0.2f;
    public GameObject popParticlePrefab;

    [Header("Link Settings")]
    public float linkPopScale = 1.2f;
    public float linkPopDuration = 0.15f;

    [Header("Spring Settings")]
    public float springDistance = 0.6f;

    private List<GameObject> allGamePieces;
    private List<GameObject> selectedChain;
    private string currentPieceType;

    private InputAction pointerPosition;
    private InputAction pointerPress;
    private bool isDragging = false;
    private GameObject originPiece;
    // Global lock to block any mouse / pointer interaction while destroy animation runs
    public static bool InputBlocked { get; private set; }
    

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
        PopulateBoard(ballAmount);
    }

    void Update()
    {
        if (isDragging && originPiece != null)
        {
            Vector2 pointerPos = Camera.main.ScreenToWorldPoint(pointerPosition.ReadValue<Vector2>());
            Rigidbody2D rb = originPiece.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.MovePosition(pointerPos);
            }

            UpdateLineRenderer();
            TryAddNeighbor(originPiece);
        }
        else if (selectedChain.Count > 0)
        {
            UpdateLineRenderer();
        }
    }

    private void OnPointerDown(InputAction.CallbackContext context)
    {
    if (InputBlocked) return; // ignore while blocked
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
    if (InputBlocked) return; // ignore while blocked
        isDragging = false;
        if (selectedChain.Count >= 2)
        {
            ProcessChain();
        }
        else
        {
            ClearChainImmediately();
        }
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

            DistanceJoint2D starJoint = closestPiece.AddComponent<DistanceJoint2D>();
            starJoint.connectedBody = rbOrigin;
            starJoint.autoConfigureDistance = false;
            starJoint.distance = springDistance;
            starJoint.maxDistanceOnly = true;

            if (selectedChain.Count > 1)
            {
                GameObject prevRing = selectedChain[selectedChain.Count - 2];
                DistanceJoint2D ringJoint = closestPiece.AddComponent<DistanceJoint2D>();
                ringJoint.connectedBody = prevRing.GetComponent<Rigidbody2D>();
                ringJoint.autoConfigureDistance = false;
                ringJoint.distance = springDistance;
                ringJoint.maxDistanceOnly = true;
            }

            // 🔹 Pop animasi untuk piece baru masuk chain
            StartCoroutine(PopLinkAnim(closestPiece));

            UpdateLineRenderer();
        }
    }

    void PopulateBoard(int amount)
    {
        for (int i = 0; i < amount; i++)
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
        if (originPiece == null || selectedChain.Count < 2) return;

        int connectionCount = (selectedChain.Count - 1) * 2 + (selectedChain.Count - 1) * 2;
        lineRenderer.positionCount = connectionCount;

        int index = 0;

        for (int i = 1; i < selectedChain.Count; i++)
        {
            if (selectedChain[i] != null)
            {
                lineRenderer.SetPosition(index, originPiece.transform.position);
                index++;
                lineRenderer.SetPosition(index, selectedChain[i].transform.position);
                index++;
            }
        }

        for (int i = 1; i < selectedChain.Count; i++)
        {
            if (selectedChain[i - 1] != null && selectedChain[i] != null)
            {
                lineRenderer.SetPosition(index, selectedChain[i - 1].transform.position);
                index++;
                lineRenderer.SetPosition(index, selectedChain[i].transform.position);
                index++;
            }
        }
    }

    void ProcessChain()
    {
        Debug.Log("Chain of " + selectedChain.Count + " pieces cleared!");
        StartCoroutine(DestroyChainCoroutine(selectedChain));
    }

    private IEnumerator DestroyChainCoroutine(List<GameObject> chain)
    {
    // Block further pointer interactions and dragging
    InputBlocked = true;
    isDragging = false;
    // Disable input actions so they don't fire during animation
    if (pointerPress.enabled) pointerPress.Disable();
    if (pointerPosition.enabled) pointerPosition.Disable();

        foreach (GameObject g in allGamePieces)
        {
            if (g != null)
            {
                Rigidbody2D rb = g.GetComponent<Rigidbody2D>();
                if (rb != null) rb.bodyType = RigidbodyType2D.Static;
            }
        }

        foreach (GameObject piece in chain)
        {
            if (piece != null)
            {
                allGamePieces.Remove(piece);
                yield return StartCoroutine(PopAndDestroy(piece));
                yield return new WaitForSeconds(destroyDelay);
            }
        }

        PopulateBoard(chain.Count);

        lineRenderer.positionCount = 0;
        originPiece = null;
        selectedChain.Clear();

        foreach (GameObject g in allGamePieces)
        {
            if (g != null)
            {
                Rigidbody2D rb = g.GetComponent<Rigidbody2D>();
                if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

    // Re-enable input actions after processing
    if (!pointerPress.enabled) pointerPress.Enable();
    if (!pointerPosition.enabled) pointerPosition.Enable();
    InputBlocked = false;
    }

    private IEnumerator PopAndDestroy(GameObject piece)
    {
        Transform t = piece.transform;
        Vector3 originalScale = t.localScale;
        Vector3 targetScale = originalScale * destroyPopScale;

        float elapsed = 0f;
        while (elapsed < destroyPopDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / destroyPopDuration;
            t.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < destroyPopDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / destroyPopDuration;
            t.localScale = Vector3.Lerp(targetScale, Vector3.zero, progress);
            UpdateLineRenderer();
            yield return null;
        }

        if (popParticlePrefab != null)
        {
            Instantiate(popParticlePrefab, t.position, Quaternion.identity);
        }

        Destroy(piece);
    }

    private IEnumerator PopLinkAnim(GameObject piece)
    {
        Transform t = piece.transform;
        Vector3 originalScale = t.localScale;
        Vector3 targetScale = originalScale * linkPopScale;

        float elapsed = 0f;
        while (elapsed < linkPopDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / linkPopDuration;
            t.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < linkPopDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / linkPopDuration;
            t.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }
    }

    void ClearChainImmediately()
    {
        foreach (var piece in selectedChain)
        {
            foreach (var joint in piece.GetComponents<DistanceJoint2D>())
            {
                Destroy(joint);
            }
        }
        selectedChain.Clear();
        lineRenderer.positionCount = 0;
        originPiece = null;
    }
}
