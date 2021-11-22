using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable2 : MonoBehaviour
{
    public delegate void DragEndedDelegate(GameObject block, Vector2 position, bool isDuplicating);
    public DragEndedDelegate dragEndedCallback;

    [SerializeField] private int type;
    [SerializeField] private float scaler = 0.9f;
    [SerializeField] private GameObject grabberPref;
    static private GameObject trashCan;
    static private Vector2 trashCanPos;
    static private GameObject grabber = null, grabber2 = null;
    private SpriteRenderer spriteRenderer1, spriteRenderer2;
    private Sprite initialSprite;

    private bool isHoldingControl = false;
    private bool isDragged = false;
    private Vector3 mouseDragStartPosition;
    private Vector3 spriteDragStartPosition;
    private const float draggingTransparency = 0.6f;

    private void Start()
    {
        spriteRenderer1 = transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer2 = transform.GetChild(1).GetComponent<SpriteRenderer>();
        initialSprite = spriteRenderer1.sprite;
        trashCan = GameObject.Find("trash_can");
        trashCanPos = new Vector2(trashCan.transform.position.x, trashCan.transform.position.y);
    }
    private void Awake()
    {    
        if (grabber == null)
        {
            grabber = Instantiate(grabberPref);
            grabber.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        if (grabber2 == null)
        {
            grabber2 = Instantiate(grabberPref);
            grabber2.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }

    private void Update()
    {
        if(isHoldingControl != Input.GetKey(KeyCode.LeftControl) && isDragged)
        {
            spriteRenderer1.color = new Vector4(1.0f, 1.0f, 1.0f, Input.GetKey(KeyCode.LeftControl) ? 1.0f : draggingTransparency);
            spriteRenderer2.color = new Vector4(1.0f, 1.0f, 1.0f, Input.GetKey(KeyCode.LeftControl) ? 1.0f : draggingTransparency);
        }
        isHoldingControl = Input.GetKey(KeyCode.LeftControl);
    }

    private void OnMouseDown()
    {
        if (tag == "Untagged") return;

        isDragged = true;
        mouseDragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        grabber.transform.position = transform.position;
        grabber2.transform.position = transform.position;
        spriteDragStartPosition = grabber.transform.position;
        grabber.GetComponent<SpriteRenderer>().sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        grabber.transform.localScale = transform.GetChild(0).transform.localScale * scaler;
        grabber.transform.eulerAngles = transform.GetChild(0).transform.eulerAngles;
        grabber2.GetComponent<SpriteRenderer>().sprite = transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
        grabber2.transform.localScale = transform.GetChild(1).transform.localScale * scaler;
        grabber2.transform.eulerAngles = transform.GetChild(1).transform.eulerAngles;

        grabber.GetComponent<SpriteRenderer>().color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        grabber2.GetComponent<SpriteRenderer>().color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        grabber.SetActive(true);
        grabber2.SetActive(true);

        if (!isHoldingControl)
        {
            spriteRenderer1.color = new Vector4(1.0f, 1.0f, 1.0f, draggingTransparency);
            spriteRenderer2.color = new Vector4(1.0f, 1.0f, 1.0f, draggingTransparency);
        }
    }
    private void OnMouseDrag()
    {
        if (isDragged)
        {
            grabber.transform.localPosition = spriteDragStartPosition + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseDragStartPosition);
            grabber2.transform.localPosition = spriteDragStartPosition + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseDragStartPosition);
            if (Vector2.Distance(trashCanPos, new Vector2(grabber.transform.localPosition.x, grabber.transform.localPosition.y)) <= 5.0f)
            {
                grabber.GetComponent<SpriteRenderer>().color = new Vector4(1.0f, 1.0f, 1.0f, draggingTransparency);
                grabber2.GetComponent<SpriteRenderer>().color = new Vector4(1.0f, 1.0f, 1.0f, draggingTransparency);
            }
            else
            {
                grabber.GetComponent<SpriteRenderer>().color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                grabber2.GetComponent<SpriteRenderer>().color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    private void OnMouseUp()
    {
        if (!isDragged) return;

        isDragged = false;
        grabber.SetActive(false);
        grabber2.SetActive(false);

        spriteRenderer1.color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        spriteRenderer2.color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        dragEndedCallback(this.gameObject, new Vector2(grabber.transform.localPosition.x, grabber.transform.localPosition.y), isHoldingControl);
    }

}
