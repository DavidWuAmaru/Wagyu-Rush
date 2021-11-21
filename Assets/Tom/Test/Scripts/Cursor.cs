using UnityEngine;
using System.Collections;

public class cursor : MonoBehaviour
{
    Vector2 mouse;
    public Texture2D cursor1;
    // Use this for initialization
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        mouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
    }
    void OnMouseOver()
    {
        Cursor.SetCursor(cursor1, Vector2.zero, CursorMode.Auto);
    }
    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}