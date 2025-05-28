using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public bool isDraggable = true;
    public float dragSpeed = 0.01f;
    public float minX = -10f;
    public float maxX = 10f;

    private Vector3 lastMousePosition;

    void Update()
    {
        Debug.Log("CameraDrag.isDraggable = " + isDraggable);

        if (!isDraggable) return;

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 newPosition = transform.position - new Vector3(delta.x * dragSpeed, 0, 0);

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            transform.position = newPosition;

            lastMousePosition = Input.mousePosition;
        }
    }
}
