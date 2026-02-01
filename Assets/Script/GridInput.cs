// GridInput.cs
using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class GridInput : MonoBehaviour
{
    [Header("References")]
    public GridManager grid;
    public Camera cam;

    [Header("Raycast")]
    public LayerMask groundLayer;

    [Header("Highlight")]
    public Transform cellHighlight;
    public float highlightYOffset = 0.02f;

    [Header("Debug")]
    public bool logOnClick = true;
    public bool hideHighlightWhenOutOfGrid = true;

    private Vector2Int _currentCell;
    private bool _hasValidCell;

    private void Reset()
    {
        cam = Camera.main;
    }

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (grid == null)
        {
            Debug.LogError("[GridInput] Missing GridManager reference.");
        }
    }

    private void Update()
    {
        if (grid == null || cam == null) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (TryGetMouseHitPoint(out Vector3 hitPoint))
        {
            Vector2Int cell = grid.WorldToCell(hitPoint);
            _hasValidCell = grid.IsInside(cell);

            if (_hasValidCell)
            {
                _currentCell = cell;

                if (cellHighlight != null)
                {
                    Vector3 center = grid.CellToWorldCenter(cell);
                    cellHighlight.position = new Vector3(center.x, center.y + highlightYOffset, center.z);

                    if (!cellHighlight.gameObject.activeSelf)
                        cellHighlight.gameObject.SetActive(true);
                }

                if (logOnClick && Input.GetMouseButtonDown(0))
                {
                    Vector3 center = grid.CellToWorldCenter(cell);
                    Debug.Log($"[Grid] Cell=({_currentCell.x},{_currentCell.y}) Center={center}");
                }
            }
            else
            {
                if (cellHighlight != null && hideHighlightWhenOutOfGrid && cellHighlight.gameObject.activeSelf)
                    cellHighlight.gameObject.SetActive(false);

                if (logOnClick && Input.GetMouseButtonDown(0))
                {
                    Debug.Log($"[Grid] Click out of grid. RawCell=({cell.x},{cell.y}) Hit={hitPoint}");
                }
            }
        }
        else
        {
            if (cellHighlight != null && hideHighlightWhenOutOfGrid && cellHighlight.gameObject.activeSelf)
                cellHighlight.gameObject.SetActive(false);
        }
    }

    private bool TryGetMouseHitPoint(out Vector3 hitPoint)
    {
        hitPoint = default;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(cam.transform.position, ray.direction * 1000f, Color.yellow);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer, QueryTriggerInteraction.Ignore))
        {

            hitPoint = hit.point;
            return true;
        }

        return false;
    }

    public bool TryGetCurrentCell(out Vector2Int cell)
    {
        cell = _currentCell;
        return _hasValidCell;
    }
}
