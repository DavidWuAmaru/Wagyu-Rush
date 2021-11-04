using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharFunc : MonoBehaviour
{
    public delegate void CollectItemDelegate(GameObject srcGameObject, GameObject tarGameObject, bool isItem);
    public CollectItemDelegate collectItemCallback;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            collectItemCallback(this.gameObject, collision.gameObject, true);
        }
        else if(collision.gameObject.tag == "Destination")
        {
            collectItemCallback(this.gameObject, collision.gameObject, false);
        }
    }
}
