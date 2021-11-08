using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    static int currentPage = 0;
    [SerializeField] private List<GameObject> pages;
    [SerializeField] private bool goup = false;
    private void OnMouseDown()
    {
        FindObjectOfType<AudioManager>().PlaySound("Click");

        if (goup && currentPage + 1 < pages.Count)
        {
            pages[currentPage].SetActive(false);
            currentPage++;
            pages[currentPage].SetActive(true);
        }
        else if(!goup && currentPage > 0)
        {
            pages[currentPage].SetActive(false);
            currentPage--;
            pages[currentPage].SetActive(true);
        }
    }
}
