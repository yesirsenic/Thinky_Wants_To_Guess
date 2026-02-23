using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGameManager : MonoBehaviour
{
    [SerializeField]
    Text suggestWord;

    private void Start()
    {
        suggestWord.text = GameManager.Instance.suggestWord;
    }
}
