using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageController : MonoBehaviour
{
    [SerializeField] List<GameObject> pages;
    int currentPage = 0;
    public void NextPage()
    {
        pages[currentPage].SetActive(false);
        currentPage++;
        Debug.Assert(0 <= currentPage && currentPage < pages.Count, "Page index is out of bounds.");
        pages[currentPage].SetActive(true);
    }

    public void PreviousPage()
    {
        pages[currentPage].SetActive(false);
        currentPage--;
        Debug.Assert(0 <= currentPage && currentPage < pages.Count, "Page index is out of bounds.");
        pages[currentPage].SetActive(true);
    }

}
