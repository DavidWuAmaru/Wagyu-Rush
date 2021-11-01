using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragged = false;
    private Vector3 mouseDragStartPosition;
    private Vector3 spriteDragStartPosition;

    private void OnMouseDown()
    {
        Debug.Log("test");
        isDragged = true;
        mouseDragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spriteDragStartPosition = transform.localPosition;
    }
    private void OnMouseDrag()
    {
        if (isDragged)
        {
            transform.localPosition = spriteDragStartPosition + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseDragStartPosition);
            Debug.Log(transform.localPosition);
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
        transform.localPosition = new Vector3((transform.localPosition.x + 4.0f) / 8.0f, (transform.localPosition.y + 4.0f) / 8.0f, (transform.localPosition.z + 4.0f) / 8.0f);
    }

}
