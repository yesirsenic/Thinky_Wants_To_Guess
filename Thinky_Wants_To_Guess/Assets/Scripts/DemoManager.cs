using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
    [SerializeField]
    GameObject demoPaenl;

    private void Start()
    {
        if(GameManager.Instance.is_Demo)
        {
            if(PlayerPrefs.GetInt("MaxStage") >=2)
            {
                demoPaenl.SetActive(true);
            }
        }
    }
}
