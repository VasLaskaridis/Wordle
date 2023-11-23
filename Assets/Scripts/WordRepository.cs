using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordRepository : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The text asset with all the words")]
    TextAsset wordlistAll = null;

    [SerializeField]
    [Tooltip("The text asset with common the words")]
    TextAsset wordlistCommon = null;

    List<string> wordsAll = null;
    List<string> wordsCommon = null;

    void Awake()
    {
        wordsAll = new List<string>(wordlistAll.text.Split(new char[]{' ', '\n', '\r'}, System.StringSplitOptions.RemoveEmptyEntries));
        wordsCommon = new List<string>(wordlistCommon.text.Split(new char[]{' ', '\n', '\r'}, System.StringSplitOptions.RemoveEmptyEntries));
    }

    public string GetRandomWord(){
        return wordsCommon[Random.Range(0, wordsCommon.Count)];
    }

    public bool CheckWordExists(string word){
        return wordsAll.Contains(word);
    }
}
