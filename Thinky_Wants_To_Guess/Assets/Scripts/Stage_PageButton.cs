using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_PageButton : MonoBehaviour
{
    [SerializeField]
    bool is_Next;

    [SerializeField]
    GameObject firstPage;

    [SerializeField]
    GameObject secondPage;

    [SerializeField]
    GameObject nextButton;

    [SerializeField]
    GameObject beforeButton;


    public void Next_Before_Button()
    {
        if(is_Next)
        {
            firstPage.SetActive(false);
            nextButton.SetActive(false);
            secondPage.SetActive(true);
            beforeButton.SetActive(true);
        }

        else
        {
            firstPage.SetActive(true);
            nextButton.SetActive(true);
            secondPage.SetActive(false);
            beforeButton.SetActive(false);
        }
    }
}
