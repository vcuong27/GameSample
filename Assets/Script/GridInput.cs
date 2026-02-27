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
    public LayerMask buildingLayer;

    [Header("Highlight")]
    public Transform cellHighlight;
    public float highlightYOffset = 0.02f;

    [Header("Drag & Drop")]
    public float pickedYOffset = 0.05f;
    public bool hideHighlightWhenOutOfGrid = true;

    private Vector2Int _currentCell;
    private bool _hasValidCell;

    private Transform _pickedHouse;
    private bool _isHoldingMouse;
    private bool _isDragging;

    private void Reset()
    {
        cam = Camera.main;
    }

    private void Awake()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    private void Update()
    {
        if (grid == null || cam == null) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        UpdateHoverCellAndHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            _isHoldingMouse = true;
            TryPickHouseUnderMouse();
            if(_pickedHouse == null)
            {
                GameController.Instance.CloseBuildMenu();
            }
        }

        if (Input.GetMouseButton(0) && _isHoldingMouse && _pickedHouse != null)
        {
            _isDragging = true;

            if (_hasValidCell)
            {
                Vector3 center = grid.CellToWorldCenter(_currentCell);
                _pickedHouse.position = new Vector3(center.x, center.y + pickedYOffset, center.z);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isHoldingMouse = false;

            if (_pickedHouse != null && _isDragging)
            {
                TryPlacePickedHouse();
            }

            _isDragging = false;
        }
    }

    private void UpdateHoverCellAndHighlight()
    {
        if (TryGetMouseHitPoint(out Vector3 hitPoint))
        {
            Vector2Int cell = grid.WorldToCell(hitPoint);
            _hasValidCell = grid.IsInside(cell);

            if (_hasValidCell)
            {
                _currentCell = cell;

                //if (cellHighlight != null)
                //{
                //    Vector3 center = grid.CellToWorldCenter(cell);
                //    cellHighlight.position = new Vector3(center.x, center.y + highlightYOffset, center.z);

                //    if (!cellHighlight.gameObject.activeSelf)
                //        cellHighlight.gameObject.SetActive(true);
                //}
            }
            else
            {
                if (cellHighlight != null && hideHighlightWhenOutOfGrid && cellHighlight.gameObject.activeSelf)
                    cellHighlight.gameObject.SetActive(false);
            }
        }
        else
        {
            _hasValidCell = false;
            if (cellHighlight != null && hideHighlightWhenOutOfGrid && cellHighlight.gameObject.activeSelf)
                cellHighlight.gameObject.SetActive(false);
        }
    }

    private void TryPickHouseUnderMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, buildingLayer, QueryTriggerInteraction.Ignore))
        {
            _pickedHouse = hit.transform;
            IBuilding sellectBuilding = _pickedHouse.GetComponent<IBuilding>();

            sellectBuilding.Select();

            GameController.Instance.OpenBuildMenu(sellectBuilding);

            Debug.Log($"[Grid] Pick house: {_pickedHouse.name}");
        }
        else
        {
            _pickedHouse = null;
        }
    }

    private void TryPlacePickedHouse()
    {
        if (_pickedHouse == null) return;

        if (!_hasValidCell)
        {
            Debug.Log("[Grid] Cannot place: out of grid.");
            return;
        }

        Vector3 center = grid.CellToWorldCenter(_currentCell);

        _pickedHouse.position = new Vector3(center.x, center.y, center.z);

        IBuilding sellectBuilding = _pickedHouse.GetComponent<IBuilding>();

        sellectBuilding.Place();

        Debug.Log($"[Grid] Place house {_pickedHouse.name} at Cell=({_currentCell.x},{_currentCell.y})");

        _pickedHouse = null;
    }

    private bool TryGetMouseHitPoint(out Vector3 hitPoint)
    {
        hitPoint = default;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

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
