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
        suggestWord.GetComponent<LocalizedSuggestWord>().SetWord(GameManager.Instance.suggestKey);
    }
}
