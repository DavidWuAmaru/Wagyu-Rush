using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public delegate void DragEndedDelegate(Vector2 position, int type);
    public DragEndedDelegate dragEndedCallback;

    [SerializeField] private int type;
    [SerializeField] private float scaler = 0.9f;
    [SerializeField] private GameObject grabber;

    private bool isDragged = false;
    private Vector3 mouseDragStartPosition;
    private Vector3 spriteDragStartPosition;
    

    private void OnMouseDown()
    {
        isDragged = true;
        mouseDragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        grabber.transform.position = transform.position;
        spriteDragStartPosition = grabber.transform.position;
        grabber.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
        grabber.transform.localScale = new Vector3(scaler, scaler, 1);
        grabber.SetActive(true);
    }
    private void OnMouseDrag()
    {
        if (isDragged)
        {
            grabber.transform.localPosition = spriteDragStartPosition + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseDragStartPosition);
        }
    }

    private void OnMouseUp()
    {
        isDragged = false;
        grabber.SetActive(false);
        dragEndedCallback(new Vector2(grabber.transform.localPosition.x, grabber.transform.localPosition.y), type);
    }

}
