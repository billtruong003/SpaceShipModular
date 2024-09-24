using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [Range(1.0f, 20.0f)] public float zoomSpeed = 5.0f;
    [Range(1.0f, 10.0f)] public float rotationSpeed = 5.0f;

    [Header("Zoom Limits")]
    public float minDistance = 5.0f; // Khoảng cách tối thiểu
    public float maxDistance = 15.0f; // Khoảng cách tối đa

    [SerializeField] private Transform target;
    [SerializeField, ReadOnly] private float currentDistance;

    void Start()
    {
        currentDistance = maxDistance / 2; // Khởi tạo khoảng cách ban đầu
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * zoomSpeed;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance); // Giới hạn khoảng cách

        if (Input.GetMouseButton(2))
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
            float vertical = Input.GetAxis("Mouse Y") * rotationSpeed;

            if (target != null)
            {
                transform.RotateAround(target.position, Vector3.up, horizontal);
                transform.RotateAround(target.position, transform.right, -vertical);
            }
        }

        if (target != null)
        {
            Vector3 direction = transform.position - target.position;
            transform.position = target.position + direction.normalized * currentDistance;
            transform.LookAt(target.position);
        }

        // if (Input.GetMouseButtonDown(0))
        // {
        //     ChangeTarget();
        // }
    }

    private Transform transformSpaceshipPart;

    void ChangeTarget()
    {
        if (IsPointerOverUIObject())
        {
            return;
        }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Selectable"))
            {
                transformSpaceshipPart = hit.transform;
                currentDistance = maxDistance / 2;
                SelectableObject selectableObject = transformSpaceshipPart.GetComponent<SelectableObject>();
                target = selectableObject.GetPivot;
            }
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
