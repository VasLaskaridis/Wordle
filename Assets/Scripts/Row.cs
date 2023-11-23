using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Row : MonoBehaviour
{
    public Letter[] letters {get; private set;}

    // public string word{
    //     get{
    //         string word = "";
    //         for(int i = 0; i < letters.Length; i++){
    //             word += letters[i].letter;
    //         }
    //         return word;
    //     }
    // }

    private void Awake(){
        letters = GetComponentsInChildren<Letter>();
        Debug.Log(letters);
    }
}
