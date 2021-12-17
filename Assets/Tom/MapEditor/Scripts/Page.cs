using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    static int currentPage = 0;
    [SerializeField] private List<GameObject> pages;
    [SerializeField] private GameObject leftButton, rightButton;
    [SerializeField] private bool goup = false;
    private void OnMouseDown()
    {
        if (goup && currentPage + 1 < pages.Count)
        {
            FindObjectOfType<AudioManager>().PlaySound("Click");
            pages[currentPage].SetActive(false);
            currentPage++;
            pages[currentPage].SetActive(true);

            leftButton.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
            if(currentPage + 1 == pages.Count) rightButton.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 0.5f);
            //leftButton.SetActive(true);
            //if(currentPage + 1 == pages.Count) rightButton.SetActive(false);
        }
        else if(!goup && currentPage > 0)
        {
            FindObjectOfType<AudioManager>().PlaySound("Click");
            pages[currentPage].SetActive(false);
            currentPage--;
            pages[currentPage].SetActive(true);

            rightButton.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
            if (currentPage == 0) leftButton.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 0.5f);
            //rightButton.SetActive(true);
            //if (currentPage == 0) leftButton.SetActive(false);
        }
    }

    private void OnEnable()
    {
        //reset properties
        currentPage = 0;
        pages[currentPage].SetActive(true);
        for (int p = 1; p < pages.Count; ++p) pages[p].SetActive(false);
        rightButton.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 1);
        leftButton.GetComponent<SpriteRenderer>().color = new Vector4(1, 1, 1, 0.5f);
    }
}
